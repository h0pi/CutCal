import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import 'appointment_detail_screen.dart';
import 'reschedule_sheet.dart';

class MyAppointmentsScreen extends StatefulWidget {
  const MyAppointmentsScreen({super.key});

  @override
  State<MyAppointmentsScreen> createState() => _MyAppointmentsScreenState();
}

class _MyAppointmentsScreenState extends State<MyAppointmentsScreen> {
  int _selectedTab = 0;
  List<AppointmentModel> _all = [];
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    // Status changes (confirm/cancel/complete) can come from someone else (a salon
    // manager on desktop), so this keeps the list current without a manual refresh.
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    final userId = context.read<AuthProvider>().userId;
    final result = await context.read<AppointmentProvider>().get(filter: {'customerId': userId, 'pageSize': 100});
    if (!mounted) return;
    setState(() {
      _all = result.items;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final upcoming = _all.where((a) => a.stateName == 'Pending' || a.stateName == 'Confirmed').toList();
    final history = _all.where((a) => a.stateName == 'Completed' || a.stateName == 'Cancelled').toList();
    final shown = _selectedTab == 0 ? upcoming : history;

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Appointments')),
      body: _isLoading
          ? const LoadingIndicator()
          : RefreshIndicator(
              onRefresh: _load,
              child: CustomScrollView(
                slivers: [
                  SliverPadding(
                    padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
                    sliver: SliverToBoxAdapter(
                      child: SegmentedTabs(
                        labels: const ['Upcoming', 'History'],
                        selectedIndex: _selectedTab,
                        onChanged: (i) => setState(() => _selectedTab = i),
                      ),
                    ),
                  ),
                  if (shown.isEmpty)
                    SliverFillRemaining(
                      hasScrollBody: false,
                      child: Center(
                        child: Text(
                          _selectedTab == 0 ? 'No upcoming appointments yet.' : 'No past appointments yet.',
                          style: const TextStyle(color: AppColors.textSecondary),
                        ),
                      ),
                    )
                  else
                    SliverPadding(
                      padding: const EdgeInsets.fromLTRB(16, 8, 16, 24),
                      sliver: SliverList.separated(
                        itemCount: shown.length,
                        separatorBuilder: (_, __) => const SizedBox(height: 12),
                        itemBuilder: (context, index) => _AppointmentCard(appointment: shown[index], onChanged: _load),
                      ),
                    ),
                ],
              ),
            ),
    );
  }
}

class _AppointmentCard extends StatelessWidget {
  final AppointmentModel appointment;
  final VoidCallback onChanged;

  const _AppointmentCard({required this.appointment, required this.onChanged});

  bool get _canReschedule => appointment.stateName == 'Pending' || appointment.stateName == 'Confirmed';
  bool get _canCancel => appointment.stateName == 'Pending' || appointment.stateName == 'Confirmed';
  bool get _canPay => appointment.paymentMethod == 'PayPal' && appointment.paymentStatus == 'Unpaid';

  Future<void> _openDetail(BuildContext context) async {
    await Navigator.of(context).push(
      MaterialPageRoute(builder: (_) => AppointmentDetailScreen(appointmentId: appointment.id)),
    );
    onChanged();
  }

  Future<void> _reschedule(BuildContext context) async {
    final success = await showRescheduleSheet(context, appointment);
    if (success) onChanged();
  }

  Future<void> _cancel(BuildContext context) async {
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
      await context.read<AppointmentProvider>().cancel(appointment.id, reason);
      if (context.mounted) {
        showSuccessSnackBar(context, 'Appointment cancelled.');
        onChanged();
      }
    } on ApiClientException catch (e) {
      if (context.mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _pay(BuildContext context) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm payment',
      message: 'Pay \$${appointment.price.toStringAsFixed(2)} via PayPal for this appointment?',
      confirmLabel: 'Pay now',
      isDestructive: false,
    );
    if (!confirmed) return;

    try {
      final paymentProvider = context.read<PaymentProvider>();
      final order = await paymentProvider.createOrder(appointment.id);
      // TODO: open order['approvalUrl'] in an in-app PayPal WebView/SDK flow and
      // return the resulting orderId here rather than capturing immediately.
      await paymentProvider.captureOrder(order['orderId'], appointment.id);
      if (context.mounted) {
        showSuccessSnackBar(context, 'Payment completed successfully.');
        onChanged();
      }
    } on ApiClientException catch (e) {
      if (context.mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    final a = appointment;
    return GestureDetector(
      onTap: () => _openDetail(context),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(18),
          border: Border.all(color: const Color(0xFFEDEAF4)),
          boxShadow: const [BoxShadow(color: Color(0x0A000000), blurRadius: 10, offset: Offset(0, 4))],
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Text(
                    a.serviceName ?? 'Appointment',
                    style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                  ),
                ),
                const SizedBox(width: 8),
                StatusBadge(status: a.stateName),
              ],
            ),
            const SizedBox(height: 4),
            Text(a.salonName ?? '', style: const TextStyle(color: AppColors.textSecondary, fontSize: 13)),
            const SizedBox(height: 10),
            Row(
              children: [
                const Icon(Icons.calendar_today_outlined, size: 14, color: AppColors.textSecondary),
                const SizedBox(width: 6),
                Text(DateFormat('MMM d, y • HH:mm').format(a.scheduledAt), style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                if (a.staffName != null) ...[
                  const SizedBox(width: 14),
                  const Icon(Icons.person_outline, size: 14, color: AppColors.textSecondary),
                  const SizedBox(width: 6),
                  Text(a.staffName!, style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                ],
              ],
            ),
            if (_canPay || _canReschedule || _canCancel) ...[
              const SizedBox(height: 14),
              Row(
                children: [
                  if (_canPay)
                    Expanded(
                      child: FilledButton(
                        style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(38), padding: EdgeInsets.zero),
                        onPressed: () => _pay(context),
                        child: Text('Pay \$${a.price.toStringAsFixed(2)}', style: const TextStyle(fontSize: 13)),
                      ),
                    ),
                  if (_canPay && (_canReschedule || _canCancel)) const SizedBox(width: 8),
                  if (_canReschedule)
                    Expanded(
                      child: OutlinedButton(
                        style: OutlinedButton.styleFrom(minimumSize: const Size.fromHeight(38), padding: EdgeInsets.zero),
                        onPressed: () => _reschedule(context),
                        child: const Text('Reschedule', style: TextStyle(fontSize: 13)),
                      ),
                    ),
                  if (_canReschedule && _canCancel) const SizedBox(width: 8),
                  if (_canCancel)
                    Expanded(
                      child: OutlinedButton(
                        style: OutlinedButton.styleFrom(
                          minimumSize: const Size.fromHeight(38),
                          padding: EdgeInsets.zero,
                          foregroundColor: AppColors.declinedText,
                          side: const BorderSide(color: Color(0xFFFCA5A5)),
                        ),
                        onPressed: () => _cancel(context),
                        child: const Text('Cancel', style: TextStyle(fontSize: 13)),
                      ),
                    ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}
