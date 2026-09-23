import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import 'paypal_webview_screen.dart';

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

  Future<void> _payWithCash() async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm booking',
      message: 'Book ${widget.service.name} with ${widget.staff.fullName ?? 'staff'} and pay \$${widget.service.price.toStringAsFixed(2)} in cash at the salon?',
      confirmLabel: 'Book & pay at salon',
      isDestructive: false,
    );
    if (!confirmed) return;

    setState(() => _isProcessing = true);
    try {
      await context.read<AppointmentProvider>().insert({
        'salonId': widget.salon.id,
        'staffId': widget.staff.id,
        'serviceId': widget.service.id,
        'scheduledAt': widget.scheduledAt.toIso8601String(),
        'paymentMethod': 'Cash',
      });
      if (mounted) {
        showSuccessSnackBar(context, 'Appointment booked for ${DateFormat('MMM d, y • HH:mm').format(widget.scheduledAt)}. Pay at the salon.');
        Navigator.of(context).popUntil((route) => route.isFirst);
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isProcessing = false);
    }
  }

  Future<void> _payWithPayPal() async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Pay with PayPal',
      message: 'Book ${widget.service.name} and pay \$${widget.service.price.toStringAsFixed(2)} now via PayPal sandbox?',
      confirmLabel: 'Pay now',
      isDestructive: false,
    );
    if (!confirmed) return;

    setState(() => _isProcessing = true);
    try {
      final appointment = await context.read<AppointmentProvider>().insert({
        'salonId': widget.salon.id,
        'staffId': widget.staff.id,
        'serviceId': widget.service.id,
        'scheduledAt': widget.scheduledAt.toIso8601String(),
        'paymentMethod': 'PayPal',
      });

      final paymentProvider = context.read<PaymentProvider>();
      final order = await paymentProvider.createOrder(appointment.id);
      final approvalUrl = order['approvalUrl'] as String?;
      if (approvalUrl == null || approvalUrl.isEmpty) {
        throw ApiClientException('PayPal did not return an approval link. Please try again.');
      }

      if (!mounted) return;
      final approvedToken = await Navigator.of(context).push<String?>(
        MaterialPageRoute(builder: (_) => PayPalWebViewScreen(approvalUrl: approvalUrl)),
      );

      if (approvedToken == null) {
        if (mounted) {
          showErrorSnackBar(context, 'Payment was not completed. Your appointment is booked but still unpaid — try again from your bookings.');
          Navigator.of(context).popUntil((route) => route.isFirst);
        }
        return;
      }

      await paymentProvider.captureOrder(order['orderId'] as String, appointment.id);

      if (mounted) {
        showSuccessSnackBar(context, 'Payment successful! Appointment booked for ${DateFormat('MMM d, y • HH:mm').format(widget.scheduledAt)}.');
        Navigator.of(context).popUntil((route) => route.isFirst);
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isProcessing = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Payment')),
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
                        '${widget.salon.name} · ${DateFormat('MMM d • HH:mm').format(widget.scheduledAt)} · ${widget.staff.fullName ?? ''}',
                        style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(16), border: Border.all(color: const Color(0xFFEDEAF5))),
                  child: Column(
                    children: [
                      _costRow('Subtotal', widget.service.price),
                      const Divider(height: 20),
                      _costRow('Total', widget.service.price, bold: true),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                const Text('Pay with', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15, color: AppColors.textPrimary)),
                const SizedBox(height: 12),
                _paymentTile(
                  badgeColor: const Color(0xFF16A34A),
                  badgeLabel: '\$',
                  title: 'Cash',
                  subtitle: 'Pay in person at the salon',
                  onTap: _payWithCash,
                ),
                const SizedBox(height: 10),
                _paymentTile(
                  badgeColor: const Color(0xFFFFC439),
                  badgeLabel: 'P',
                  title: 'PayPal',
                  subtitle: 'Sandbox checkout · buyer protection',
                  onTap: _payWithPayPal,
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

  Widget _costRow(String label, double amount, {bool bold = false}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: TextStyle(color: bold ? AppColors.textPrimary : AppColors.textSecondary, fontWeight: bold ? FontWeight.bold : FontWeight.normal)),
        Text(
          '\$${amount.toStringAsFixed(2)}',
          style: TextStyle(
            color: bold ? AppColors.primary : AppColors.textPrimary,
            fontWeight: FontWeight.bold,
            fontSize: bold ? 17 : 14,
          ),
        ),
      ],
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
