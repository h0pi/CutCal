import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import '../reviews/add_review_screen.dart';
import 'reschedule_sheet.dart';

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
    if (!mounted) return;
    setState(() {
      _appointment = appointment;
      _isLoading = false;
    });
  }

  Future<void> _cancel() async {
    final reason = await showCancelReasonDialog(context);
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

  Future<void> _reschedule() async {
    final success = await showRescheduleSheet(context, _appointment!);
    if (success) _load();
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

  static const _cardDecoration = BoxDecoration(
    color: Colors.white,
    borderRadius: BorderRadius.all(Radius.circular(18)),
    border: Border.fromBorderSide(BorderSide(color: Color(0xFFEDEAF4))),
  );

  (Color, Color) _heroColorsFor(String status) {
    switch (status) {
      case 'Pending':
        return (AppColors.pendingBg, AppColors.pendingText);
      case 'Confirmed':
        return (AppColors.confirmedBg, AppColors.confirmedText);
      case 'Completed':
        return (AppColors.completedBg, AppColors.completedText);
      case 'Cancelled':
        return (AppColors.declinedBg, AppColors.declinedText);
      default:
        return (AppColors.neutralBg, AppColors.neutralText);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading || _appointment == null) {
      return const Scaffold(backgroundColor: AppColors.background, body: LoadingIndicator());
    }

    final a = _appointment!;
    final canCancel = a.stateName == 'Pending' || a.stateName == 'Confirmed';
    final canReschedule = a.stateName == 'Pending' || a.stateName == 'Confirmed';
    final canReview = a.stateName == 'Completed' && !a.hasReview;
    final canPay = a.paymentMethod == 'PayPal' && a.paymentStatus == 'Unpaid';
    final (heroBg, heroText) = _heroColorsFor(a.stateName);

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Appointment'), backgroundColor: AppColors.background, elevation: 0),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(color: heroBg, borderRadius: BorderRadius.circular(20)),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Expanded(
                      child: Text(
                        a.serviceName ?? 'Appointment',
                        style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold, color: heroText),
                      ),
                    ),
                    StatusBadge(status: a.stateName),
                  ],
                ),
                const SizedBox(height: 6),
                Text(a.salonName ?? '', style: TextStyle(fontSize: 14, color: heroText.withValues(alpha: 0.85))),
                const SizedBox(height: 16),
                Row(
                  children: [
                    Icon(Icons.calendar_today_outlined, size: 15, color: heroText.withValues(alpha: 0.85)),
                    const SizedBox(width: 6),
                    Text(DateFormat('MMM d, y • HH:mm').format(a.scheduledAt), style: TextStyle(color: heroText.withValues(alpha: 0.85), fontSize: 13)),
                  ],
                ),
                if (a.staffName != null) ...[
                  const SizedBox(height: 6),
                  Row(
                    children: [
                      Icon(Icons.person_outline, size: 15, color: heroText.withValues(alpha: 0.85)),
                      const SizedBox(width: 6),
                      Text(a.staffName!, style: TextStyle(color: heroText.withValues(alpha: 0.85), fontSize: 13)),
                    ],
                  ),
                ],
              ],
            ),
          ),
          const SizedBox(height: 16),
          Container(
            padding: const EdgeInsets.all(16),
            decoration: _cardDecoration,
            child: Column(
              children: [
                _infoRow('Duration', '${a.durationMinutes} min'),
                const Divider(height: 20, color: Color(0xFFEDEAF4)),
                _infoRow('Price', '\$${a.price.toStringAsFixed(2)}'),
                const Divider(height: 20, color: Color(0xFFEDEAF4)),
                _infoRow('Payment', '${a.paymentMethod} • ${a.paymentStatus}'),
                if (a.cancellationReason != null) ...[
                  const Divider(height: 20, color: Color(0xFFEDEAF4)),
                  _infoRow('Cancellation reason', a.cancellationReason!),
                ],
              ],
            ),
          ),
          const SizedBox(height: 24),
          if (canPay)
            SizedBox(
              width: double.infinity,
              child: FilledButton(onPressed: _pay, child: Text('Pay \$${a.price.toStringAsFixed(2)}')),
            ),
          if (canReview) ...[
            if (canPay) const SizedBox(height: 10),
            SizedBox(
              width: double.infinity,
              child: FilledButton(
                onPressed: () async {
                  await Navigator.of(context).push(
                    MaterialPageRoute(builder: (_) => AddReviewScreen(appointmentId: a.id)),
                  );
                  _load();
                },
                child: const Text('Leave a review'),
              ),
            ),
          ],
          if (canReschedule) ...[
            if (canPay || canReview) const SizedBox(height: 10),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton(onPressed: _reschedule, child: const Text('Reschedule')),
            ),
          ],
          if (canCancel) ...[
            const SizedBox(height: 10),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton(
                style: OutlinedButton.styleFrom(foregroundColor: AppColors.declinedText, side: const BorderSide(color: Color(0xFFFCA5A5))),
                onPressed: _cancel,
                child: const Text('Cancel appointment'),
              ),
            ),
          ],
        ],
      ),
    );
  }

  Widget _infoRow(String label, String value) {
    return Row(
      children: [
        Expanded(child: Text(label, style: const TextStyle(color: AppColors.textSecondary, fontSize: 13))),
        Text(value, style: const TextStyle(color: AppColors.textPrimary, fontSize: 13, fontWeight: FontWeight.w600)),
      ],
    );
  }
}
