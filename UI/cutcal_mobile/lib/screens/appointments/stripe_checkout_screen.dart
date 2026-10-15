import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:webview_flutter/webview_flutter.dart';

import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';

const _successUrlPrefix = 'https://cutcal.app/stripe/success';
const _cancelUrlPrefix = 'https://cutcal.app/stripe/cancel';

/// Pays an appointment by card: the customer enters the card on Stripe's own hosted page, so CutCal never sees it.
/// Returns true once the server has confirmed with Stripe that the payment went through, false if the customer backed out.
Future<bool> payWithStripe(BuildContext context, int appointmentId) async {
  final paymentProvider = context.read<PaymentProvider>();
  final session = await paymentProvider.createStripeSession(appointmentId);
  final checkoutUrl = session['checkoutUrl'] as String?;
  if (checkoutUrl == null || checkoutUrl.isEmpty) {
    throw ApiClientException('Stripe did not return a checkout link. Please try again.');
  }

  if (!context.mounted) return false;
  final completed = await Navigator.of(context).push<bool>(
    MaterialPageRoute(builder: (_) => StripeCheckoutScreen(checkoutUrl: checkoutUrl)),
  );
  if (completed != true) return false;

  await paymentProvider.confirmStripePayment(appointmentId);
  return true;
}

/// Shows Stripe's hosted checkout page in an in-app WebView and intercepts the success/cancel redirect configured on the
/// session (see PaymentService), so the WebView never tries to load those pages.
class StripeCheckoutScreen extends StatefulWidget {
  final String checkoutUrl;

  const StripeCheckoutScreen({super.key, required this.checkoutUrl});

  @override
  State<StripeCheckoutScreen> createState() => _StripeCheckoutScreenState();
}

class _StripeCheckoutScreenState extends State<StripeCheckoutScreen> {
  late final WebViewController _controller;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageStarted: (_) => setState(() => _isLoading = true),
          onPageFinished: (_) => setState(() => _isLoading = false),
          onNavigationRequest: (request) {
            if (request.url.startsWith(_successUrlPrefix)) {
              Navigator.of(context).pop(true);
              return NavigationDecision.prevent;
            }
            if (request.url.startsWith(_cancelUrlPrefix)) {
              Navigator.of(context).pop(false);
              return NavigationDecision.prevent;
            }
            return NavigationDecision.navigate;
          },
        ),
      )
      ..loadRequest(Uri.parse(widget.checkoutUrl));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Pay by card'),
        leading: IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.of(context).pop(false)),
      ),
      body: Stack(
        children: [
          WebViewWidget(controller: _controller),
          if (_isLoading) const Center(child: CircularProgressIndicator(color: AppColors.primary)),
        ],
      ),
    );
  }
}
