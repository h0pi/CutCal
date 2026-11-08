import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:webview_flutter/webview_flutter.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';

const _returnUrlPrefix = 'https://cutcal.app/paypal/return';
const _cancelUrlPrefix = 'https://cutcal.app/paypal/cancel';

/// Pays an appointment through PayPal: the customer approves the payment on PayPal's own page and only then is it captured.
/// Returns true once the payment is captured, false if the customer backed out.
Future<bool> payWithPayPal(BuildContext context, int appointmentId, TipChoice tip) async {
  final paymentProvider = context.read<PaymentProvider>();
  final order = await paymentProvider.createOrder(appointmentId, tip);
  final approvalUrl = order['approvalUrl'] as String?;
  if (approvalUrl == null || approvalUrl.isEmpty) {
    throw ApiClientException('PayPal did not return an approval link. Please try again.');
  }

  if (!context.mounted) return false;
  final approvedToken = await Navigator.of(context).push<String?>(
    MaterialPageRoute(builder: (_) => PayPalWebViewScreen(approvalUrl: approvalUrl)),
  );
  if (approvedToken == null) return false;

  await paymentProvider.captureOrder(order['orderId'] as String, appointmentId);
  return true;
}

/// Shows PayPal's own hosted approval page in an in-app WebView. The customer logs in and
/// approves (or cancels) the payment there; we never see their PayPal credentials. PayPal then
/// navigates to the return/cancel URLs configured on the order (see PaymentService.CreateOrderAsync),
/// which this screen intercepts to report the outcome back to the caller instead of letting the
/// WebView actually try to load those (non-existent) pages.
class PayPalWebViewScreen extends StatefulWidget {
  final String approvalUrl;

  const PayPalWebViewScreen({super.key, required this.approvalUrl});

  @override
  State<PayPalWebViewScreen> createState() => _PayPalWebViewScreenState();
}

class _PayPalWebViewScreenState extends State<PayPalWebViewScreen> {
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
            if (request.url.startsWith(_returnUrlPrefix)) {
              final token = Uri.parse(request.url).queryParameters['token'];
              Navigator.of(context).pop(token);
              return NavigationDecision.prevent;
            }
            if (request.url.startsWith(_cancelUrlPrefix)) {
              Navigator.of(context).pop(null);
              return NavigationDecision.prevent;
            }
            return NavigationDecision.navigate;
          },
        ),
      )
      ..loadRequest(Uri.parse(widget.approvalUrl));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Approve payment'),
        leading: IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.of(context).pop(null)),
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
