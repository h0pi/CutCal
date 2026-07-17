import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';
import '../reviews/add_review_screen.dart';

class AppointmentDetailScreen extends StatefulWidget {
  final int appointmentId;

  const AppointmentDetailScreen({super.key, required this.appointmentId});

  @override
  State<AppointmentDetailScreen> createState() => _AppointmentDetailScreenState();
}

class _AppointmentDetailScreenState extends State<AppointmentDetailScreen> {
  AppointmentModel? _appointment;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final appointment = await context.read<AppointmentProvider>().getById(widget.appointmentId);
    setState(() {
      _appointment = appointment;
      _isLoading = false;
    });
  }

  Future<void> _cancel() async {
    final reasonController = TextEditingController();
    String? errorText;

    final reason = await showDialog<String>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: const Text('Cancel appointment'),
          content: TextField(
            controller: reasonController,
            autofocus: true,
            decoration: InputDecoration(
              labelText: 'Reason for cancellation',
              border: const OutlineInputBorder(),
              errorText: errorText,
            ),
            maxLines: 2,
            onChanged: (_) {
              if (errorText != null) setDialogState(() => errorText = null);
            },
          ),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Back')),
            TextButton(
              style: TextButton.styleFrom(foregroundColor: Colors.red),
              onPressed: () {
                if (reasonController.text.trim().isEmpty) {
                  setDialogState(() => errorText = 'Please tell us why you are cancelling.');
                  return;
                }
                Navigator.of(context).pop(reasonController.text.trim());
              },
              child: const Text('Cancel appointment'),
            ),
          ],
        ),
      ),
    );
    if (reason == null) return;

    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm cancellation',
      message: 'Are you sure you want to cancel this appointment? This cannot be undone.',
      confirmLabel: 'Yes, cancel',
    );
    if (!confirmed) return;

    try {
      await context.read<AppointmentProvider>().cancel(widget.appointmentId, reason);
      if (mounted) {
        showSuccessSnackBar(context, 'Appointment cancelled.');
        _load();
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _pay() async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm payment',
      message: 'Pay \$${_appointment!.price.toStringAsFixed(2)} via PayPal for this appointment?',
      confirmLabel: 'Pay now',
      isDestructive: false,
    );
    if (!confirmed) return;

    try {
      final paymentProvider = context.read<PaymentProvider>();
      final order = await paymentProvider.createOrder(widget.appointmentId);
      // TODO: open order['approvalUrl'] in an in-app PayPal WebView/SDK flow and
      // return the resulting orderId here rather than capturing immediately.
      await paymentProvider.captureOrder(order['orderId'], widget.appointmentId);
      if (mounted) {
        showSuccessSnackBar(context, 'Payment completed successfully.');
        _load();
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading || _appointment == null) {
      return const Scaffold(body: LoadingIndicator());
    }

    final a = _appointment!;
    final canCancel = a.stateName == 'Pending' || a.stateName == 'Confirmed';
    final canReview = a.stateName == 'Completed' && !a.hasReview;
    final canPay = a.paymentMethod == 'PayPal' && a.paymentStatus == 'Unpaid';

    return Scaffold(
      appBar: AppBar(title: const Text('Appointment')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Row(mainAxisAlignment: MainAxisAlignment.spaceBetween, children: [
            Text(a.salonName ?? '', style: Theme.of(context).textTheme.titleLarge),
            StatusBadge(status: a.stateName),
          ]),
          const SizedBox(height: 12),
          _infoRow('Service', a.serviceName ?? '-'),
          _infoRow('Staff', a.staffName ?? '-'),
          _infoRow('Date & time', DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)),
          _infoRow('Duration', '${a.durationMinutes} min'),
          _infoRow('Price', '\$${a.price.toStringAsFixed(2)}'),
          _infoRow('Payment', '${a.paymentMethod} • ${a.paymentStatus}'),
          if (a.cancellationReason != null) _infoRow('Cancellation reason', a.cancellationReason!),
          const SizedBox(height: 24),
          if (canPay)
            FilledButton.icon(icon: const Icon(Icons.payment), label: const Text('Pay'), onPressed: _pay)
          else if (a.paymentMethod == 'PayPal')
            Tooltip(
              message: a.paymentStatus == 'Paid' ? 'Already paid' : 'Payment unavailable in this state',
              child: const FilledButton(onPressed: null, child: Text('Pay')),
            ),
          const SizedBox(height: 8),
          if (canReview)
            OutlinedButton.icon(
              icon: const Icon(Icons.rate_review),
              label: const Text('Leave a review'),
              onPressed: () async {
                await Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => AddReviewScreen(appointmentId: a.id)),
                );
                _load();
              },
            ),
          const SizedBox(height: 8),
          if (canCancel)
            OutlinedButton.icon(
              style: OutlinedButton.styleFrom(foregroundColor: Colors.red),
              icon: const Icon(Icons.cancel),
              label: const Text('Cancel appointment'),
              onPressed: _cancel,
            )
          else
            Tooltip(
              message: 'This appointment can no longer be cancelled.',
              child: OutlinedButton(onPressed: null, child: const Text('Cancel appointment')),
            ),
        ],
      ),
    );
  }

  Widget _infoRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        children: [
          SizedBox(width: 130, child: Text(label, style: const TextStyle(color: Colors.grey))),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }
}
