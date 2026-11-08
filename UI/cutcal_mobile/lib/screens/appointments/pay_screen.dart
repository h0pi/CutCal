import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/format.dart';
import '../../utils/utils_widgets.dart';
import 'pay_appointment.dart';

/// Where a confirmed appointment is paid: the customer chooses a tip and sees the price, the tax and the total exactly as the server
/// works them out, which is also what is then charged. Pops with true once the payment went through.
class PayScreen extends StatefulWidget {
  final AppointmentModel appointment;

  const PayScreen({super.key, required this.appointment});

  @override
  State<PayScreen> createState() => _PayScreenState();
}

class _PayScreenState extends State<PayScreen> {
  static const _tipPercents = [0.0, 10.0, 15.0, 20.0];

  TipChoice _tip = const TipChoice.percent(0);
  Map<String, dynamic>? _quote;
  String? _error;
  bool _isPaying = false;

  @override
  void initState() {
    super.initState();
    _loadQuote();
  }

  Future<void> _loadQuote() async {
    try {
      final quote = await context.read<PaymentProvider>().getQuote(widget.appointment.id, _tip);
      if (!mounted) return;
      setState(() {
        _quote = quote;
        _error = null;
      });
    } on ApiClientException catch (e) {
      if (mounted) setState(() => _error = e.message);
    }
  }

  void _chooseTip(TipChoice tip) {
    setState(() => _tip = tip);
    _loadQuote();
  }

  Future<void> _chooseCustomTip() async {
    final controller = TextEditingController();
    final text = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Tip amount'),
        content: TextField(
          controller: controller,
          autofocus: true,
          keyboardType: const TextInputType.numberWithOptions(decimal: true),
          decoration: InputDecoration(labelText: 'Amount in ${widget.appointment.currency}'),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(controller.text), child: const Text('Apply')),
        ],
      ),
    );
    final amount = double.tryParse(text?.replaceAll(',', '.') ?? '');
    if (amount != null) _chooseTip(TipChoice.amount(amount));
  }

  Future<void> _pay() async {
    setState(() => _isPaying = true);
    try {
      final paid = await payAppointment(context, widget.appointment, _tip);
      if (!mounted) return;
      if (paid) {
        Navigator.of(context).pop(true);
      } else {
        showErrorSnackBar(context, 'Payment was not completed.');
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isPaying = false);
    }
  }

  bool _isSelected(double percent) => _tip.percent == percent;

  @override
  Widget build(BuildContext context) {
    final a = widget.appointment;
    final quote = _quote;
    String money(String key) => formatMoney((quote![key] as num), quote['currency'] as String);

    return Scaffold(
      appBar: AppBar(title: const Text('Pay')),
      body: _isPaying
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
                      Text(a.serviceName ?? '', style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16, color: AppColors.textPrimary)),
                      const SizedBox(height: 4),
                      Text(
                        '${a.salonName ?? ''} · ${formatDateTime(a.scheduledAt, is24Hour: a.is24HourFormat, datePattern: 'MMM d')} · ${a.staffName ?? ''}',
                        style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 24),
                const Text('Tip for your stylist', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15, color: AppColors.textPrimary)),
                const SizedBox(height: 12),
                Wrap(
                  spacing: 8,
                  children: [
                    for (final percent in _tipPercents)
                      ChoiceChip(
                        label: Text(percent == 0 ? 'No tip' : '${percent.round()}%'),
                        selected: _isSelected(percent),
                        onSelected: (_) => _chooseTip(TipChoice.percent(percent)),
                      ),
                    ChoiceChip(
                      label: Text(_tip.amount == null ? 'Custom' : 'Custom: ${formatMoney(_tip.amount!, a.currency)}'),
                      selected: _tip.amount != null,
                      onSelected: (_) => _chooseCustomTip(),
                    ),
                  ],
                ),
                const SizedBox(height: 24),
                if (_error != null)
                  Text(_error!, style: const TextStyle(color: AppColors.declinedText))
                else if (quote == null)
                  const Center(child: CircularProgressIndicator(color: AppColors.primary))
                else
                  Container(
                    padding: const EdgeInsets.all(16),
                    decoration: BoxDecoration(color: Colors.white, borderRadius: BorderRadius.circular(16), border: Border.all(color: const Color(0xFFEDEAF5))),
                    child: Column(
                      children: [
                        _costRow('Price', money('subtotal')),
                        const SizedBox(height: 8),
                        _costRow('Tax (${((quote['taxRate'] as num) * 100).round()}%)', money('tax')),
                        const SizedBox(height: 8),
                        _costRow('Tip', money('tip')),
                        const Divider(height: 20),
                        _costRow('Total', money('total'), bold: true),
                      ],
                    ),
                  ),
                const SizedBox(height: 24),
                FilledButton(
                  onPressed: quote == null || _error != null ? null : _pay,
                  child: Text(quote == null ? 'Pay' : 'Pay ${money('total')} by ${a.paymentMethod == 'Stripe' ? 'card' : 'PayPal'}'),
                ),
              ],
            ),
    );
  }

  Widget _costRow(String label, String value, {bool bold = false}) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: TextStyle(color: bold ? AppColors.textPrimary : AppColors.textSecondary, fontWeight: bold ? FontWeight.bold : FontWeight.normal)),
        Text(
          value,
          style: TextStyle(
            color: bold ? AppColors.primary : AppColors.textPrimary,
            fontWeight: FontWeight.bold,
            fontSize: bold ? 17 : 14,
          ),
        ),
      ],
    );
  }
}
