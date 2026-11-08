import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/format.dart';
import '../../utils/utils_widgets.dart';

/// The last step of booking: nothing is paid here. The customer picks how they will pay, and the payment itself
/// (with the tip, the tax and the total) happens once the salon has confirmed the appointment.
class PaymentScreen extends StatefulWidget {
  final SalonModel salon;
  final SalonServiceModel service;
  final StaffModel staff;
  final DateTime scheduledAt;

  const PaymentScreen({
    super.key,
    required this.salon,
    required this.service,
    required this.staff,
    required this.scheduledAt,
  });

  @override
  State<PaymentScreen> createState() => _PaymentScreenState();
}

class _PaymentScreenState extends State<PaymentScreen> {
  bool _isProcessing = false;

  Future<void> _book({required String method, required String methodName}) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm booking',
      message: 'Book ${widget.service.name}? You will pay by $methodName after the salon confirms your appointment.',
      confirmLabel: 'Book',
      isDestructive: false,
    );
    if (!confirmed || !mounted) return;

    setState(() => _isProcessing = true);
    try {
      await context.read<AppointmentProvider>().insert({
        'salonId': widget.salon.id,
        'staffId': widget.staff.id,
        'serviceId': widget.service.id,
        'scheduledAt': widget.scheduledAt.toIso8601String(),
        'paymentMethod': method,
      });
      if (!mounted) return;
      showSuccessSnackBar(
        context,
        'Appointment booked for ${formatDateTime(widget.scheduledAt, is24Hour: widget.salon.is24HourFormat)}. You can pay once the salon confirms it.',
      );
      Navigator.of(context).popUntil((route) => route.isFirst);
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isProcessing = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Confirm booking')),
      body: _isProcessing
          ? const LoadingIndicator()
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(color: AppColors.surfaceMuted, borderRadius: BorderRadius.circular(16)),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(widget.service.name, style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16, color: AppColors.textPrimary)),
                      const SizedBox(height: 4),
                      Text(
                        '${widget.salon.name} · ${formatDateTime(widget.scheduledAt, is24Hour: widget.salon.is24HourFormat, datePattern: 'MMM d')} · ${widget.staff.fullName ?? ''}',
                        style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(16), border: Border.all(color: const Color(0xFFEDEAF5))),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      const Text('Price', style: TextStyle(color: AppColors.textSecondary)),
                      Text(
                        formatMoney(widget.service.price, widget.salon.currency),
                        style: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.bold, fontSize: 17),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 8),
                const Text(
                  'Tax and an optional tip are added when you pay.',
                  style: TextStyle(color: AppColors.textSecondary, fontSize: 12),
                ),
                const SizedBox(height: 24),
                const Text('Pay later with', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15, color: AppColors.textPrimary)),
                const SizedBox(height: 12),
                _paymentTile(
                  badgeColor: const Color(0xFFFFC439),
                  badgeLabel: 'P',
                  title: 'PayPal',
                  subtitle: 'Sandbox checkout · buyer protection',
                  onTap: () => _book(method: 'PayPal', methodName: 'PayPal'),
                ),
                const SizedBox(height: 10),
                _paymentTile(
                  badgeColor: const Color(0xFF635BFF),
                  badgeLabel: 'S',
                  title: 'Card',
                  subtitle: 'Secure checkout by Stripe',
                  onTap: () => _book(method: 'Stripe', methodName: 'card'),
                ),
                const SizedBox(height: 16),
                const Text(
                  'No booking fee. Card details are never stored by CutCal.',
                  textAlign: TextAlign.center,
                  style: TextStyle(color: AppColors.textSecondary, fontSize: 11),
                ),
              ],
            ),
    );
  }

  Widget _paymentTile({
    required Color badgeColor,
    required String badgeLabel,
    required String title,
    required String subtitle,
    required VoidCallback onTap,
  }) {
    return InkWell(
      borderRadius: BorderRadius.circular(14),
      onTap: onTap,
      child: Container(
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(14), border: Border.all(color: const Color(0xFFEDEAF5))),
        child: Row(
          children: [
            Container(
              width: 36,
              height: 36,
              decoration: BoxDecoration(color: badgeColor, borderRadius: BorderRadius.circular(8)),
              alignment: Alignment.center,
              child: Text(badgeLabel, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
                  Text(subtitle, style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                ],
              ),
            ),
            const Icon(Icons.chevron_right, color: AppColors.textSecondary),
          ],
        ),
      ),
    );
  }
}
