import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import '../notifications/notifications_screen.dart';

class ManagerDashboardScreen extends StatefulWidget {
  const ManagerDashboardScreen({super.key});

  @override
  State<ManagerDashboardScreen> createState() => _ManagerDashboardScreenState();
}

class _ManagerDashboardScreenState extends State<ManagerDashboardScreen> {
  SalonModel? _salon;
  List<AppointmentModel> _todayAppointments = [];
  List<AppointmentModel> _pendingAppointments = [];
  List<double> _dailyRevenue = List.filled(7, 0);
  double _weekRevenue = 0;
  double? _revenueChangePct;
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    _pollTimer = Timer.periodic(const Duration(seconds: 20), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);

    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final weekStart = today.subtract(const Duration(days: 6));
    final prevWeekStart = weekStart.subtract(const Duration(days: 7));

    // The backend automatically scopes both of these to salons this manager owns.
    // The four calls are independent, so they're kicked off together instead of
    // one after another — this runs every 20s via the poll timer, so it matters.
    final salonsFuture = context.read<SalonProvider>().get(filter: {'pageSize': 20});
    final appointmentsFuture = context.read<AppointmentProvider>().get(filter: {'pageSize': 200});
    final thisWeekReportFuture = context.read<ReportProvider>().getSummary(dateFrom: weekStart, dateTo: now);
    final prevWeekReportFuture = context.read<ReportProvider>().getSummary(dateFrom: prevWeekStart, dateTo: weekStart);

    final salons = await salonsFuture;
    final appointments = await appointmentsFuture;
    final thisWeekReport = await thisWeekReportFuture;
    final prevWeekReport = await prevWeekReportFuture;

    final pending = appointments.items.where((a) => a.stateName == 'Pending');
    final todays = appointments.items.where((a) =>
        a.scheduledAt.year == now.year && a.scheduledAt.month == now.month && a.scheduledAt.day == now.day);

    final dailyRevenue = List<double>.filled(7, 0);
    final thisWeekAppointments = (thisWeekReport['appointmentsReport']?['appointments'] as List?) ?? [];
    for (final json in thisWeekAppointments) {
      if (json['paymentStatus'] != 'Paid') continue;
      final scheduledAt = DateTime.parse(json['scheduledAt']);
      final dayIndex = DateTime(scheduledAt.year, scheduledAt.month, scheduledAt.day).difference(weekStart).inDays;
      if (dayIndex < 0 || dayIndex > 6) continue;
      dailyRevenue[dayIndex] += (json['price'] as num).toDouble();
    }

    final weekRevenue = (thisWeekReport['appointmentsReport']?['totalRevenue'] as num?)?.toDouble() ?? 0;
    final prevWeekRevenue = (prevWeekReport['appointmentsReport']?['totalRevenue'] as num?)?.toDouble() ?? 0;
    final changePct = prevWeekRevenue == 0 ? null : ((weekRevenue - prevWeekRevenue) / prevWeekRevenue) * 100;

    if (!mounted) return;
    setState(() {
      _salon = salons.items.firstOrNull;
      _todayAppointments = todays.toList()..sort((a, b) => a.scheduledAt.compareTo(b.scheduledAt));
      _pendingAppointments = pending.toList();
      _dailyRevenue = dailyRevenue;
      _weekRevenue = weekRevenue;
      _revenueChangePct = changePct;
      _isLoading = false;
    });
  }

  Future<void> _confirm(AppointmentModel a) async {
    try {
      await context.read<AppointmentProvider>().confirm(a.id);
      if (mounted) showSuccessSnackBar(context, 'Appointment confirmed.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _reject(AppointmentModel a) async {
    final reason = await showCancelReasonDialog(context);
    if (reason == null) return;
    try {
      await context.read<AppointmentProvider>().cancel(a.id, reason);
      if (mounted) showSuccessSnackBar(context, 'Appointment rejected.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Home')),
      body: _isLoading
          ? const LoadingIndicator()
          : RefreshIndicator(
              onRefresh: _load,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  _buildHero(context),
                  const SizedBox(height: 16),
                  _buildRevenueCard(),
                  const SizedBox(height: 24),
                  Text('Needs approval', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  if (_pendingAppointments.isEmpty)
                    const Text('No pending requests.', style: TextStyle(color: AppColors.textSecondary))
                  else
                    ..._pendingAppointments.map((a) => _buildPendingCard(a)),
                  const SizedBox(height: 24),
                  Text("Today's schedule", style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 8),
                  if (_todayAppointments.isEmpty)
                    const Text('No appointments scheduled for today.', style: TextStyle(color: AppColors.textSecondary))
                  else
                    ..._todayAppointments.map((a) => _buildScheduleRow(a)),
                ],
              ),
            ),
    );
  }

  Widget _buildHero(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(20),
      decoration: BoxDecoration(color: AppColors.businessHero, borderRadius: BorderRadius.circular(20)),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Expanded(
                child: Text(
                  _salon?.name ?? 'Your salon',
                  style: const TextStyle(color: Colors.white, fontSize: 18, fontWeight: FontWeight.bold),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              GestureDetector(
                onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NotificationsScreen())),
                child: Stack(
                  clipBehavior: Clip.none,
                  children: [
                    const Icon(Icons.notifications_outlined, color: Colors.white),
                    if (_pendingAppointments.isNotEmpty)
                      Positioned(
                        right: -2,
                        top: -2,
                        child: Container(
                          width: 9,
                          height: 9,
                          decoration: const BoxDecoration(color: Color(0xFFF87171), shape: BoxShape.circle),
                        ),
                      ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),
          Row(
            children: [
              Expanded(child: _heroStat('${_todayAppointments.length + _pendingAppointments.length}', 'This week')),
              Container(width: 1, height: 36, color: Colors.white24),
              Expanded(child: _heroStat('${_pendingAppointments.length}', 'Awaiting approval')),
            ],
          ),
        ],
      ),
    );
  }

  Widget _heroStat(String value, String label) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(value, style: const TextStyle(color: Colors.white, fontSize: 22, fontWeight: FontWeight.bold)),
        const SizedBox(height: 2),
        Text(label, style: const TextStyle(color: Colors.white70, fontSize: 12)),
      ],
    );
  }

  Widget _buildRevenueCard() {
    final maxRevenue = _dailyRevenue.reduce((a, b) => a > b ? a : b);
    final today = DateTime.now();

    return Container(
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: const Color(0xFFEDEAF4)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text('Revenue this week', style: TextStyle(color: AppColors.textSecondary, fontSize: 13)),
                    const SizedBox(height: 4),
                    Text('\$${_weekRevenue.toStringAsFixed(2)}', style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                  ],
                ),
              ),
              if (_revenueChangePct != null)
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                  decoration: BoxDecoration(
                    color: _revenueChangePct! >= 0 ? AppColors.completedBg : AppColors.declinedBg,
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Text(
                    '${_revenueChangePct! >= 0 ? '+' : ''}${_revenueChangePct!.toStringAsFixed(0)}%',
                    style: TextStyle(
                      color: _revenueChangePct! >= 0 ? AppColors.completedText : AppColors.declinedText,
                      fontWeight: FontWeight.bold,
                      fontSize: 12,
                    ),
                  ),
                ),
            ],
          ),
          const SizedBox(height: 20),
          SizedBox(
            height: 90,
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.end,
              children: List.generate(7, (i) {
                final day = DateTime.now().subtract(Duration(days: 6 - i));
                final isToday = day.year == today.year && day.month == today.month && day.day == today.day;
                final heightFraction = maxRevenue == 0 ? 0.06 : (0.1 + 0.9 * (_dailyRevenue[i] / maxRevenue));
                return Expanded(
                  child: Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 4),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.end,
                      children: [
                        Container(
                          height: 60 * heightFraction,
                          decoration: BoxDecoration(
                            color: isToday ? AppColors.primary : AppColors.primaryLight,
                            borderRadius: BorderRadius.circular(6),
                          ),
                        ),
                        const SizedBox(height: 6),
                        Text(
                          DateFormat('E').format(day).substring(0, 1),
                          style: TextStyle(
                            fontSize: 11,
                            fontWeight: isToday ? FontWeight.bold : FontWeight.normal,
                            color: isToday ? AppColors.primary : AppColors.textSecondary,
                          ),
                        ),
                      ],
                    ),
                  ),
                );
              }),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildPendingCard(AppointmentModel a) {
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(16),
        border: Border.all(color: const Color(0xFFEDEAF4)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('${a.customerName ?? 'Customer'} • ${a.serviceName ?? ''}', style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
          const SizedBox(height: 2),
          Text(DateFormat('MMM d, y • HH:mm').format(a.scheduledAt), style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
          const SizedBox(height: 10),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  style: OutlinedButton.styleFrom(
                    minimumSize: const Size.fromHeight(36),
                    foregroundColor: AppColors.declinedText,
                    side: const BorderSide(color: Color(0xFFFCA5A5)),
                  ),
                  onPressed: () => _reject(a),
                  child: const Text('Reject', style: TextStyle(fontSize: 13)),
                ),
              ),
              const SizedBox(width: 8),
              Expanded(
                child: FilledButton(
                  style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(36)),
                  onPressed: () => _confirm(a),
                  child: const Text('Confirm', style: TextStyle(fontSize: 13)),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildScheduleRow(AppointmentModel a) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(14),
        border: Border.all(color: const Color(0xFFEDEAF4)),
      ),
      child: Row(
        children: [
          Text(DateFormat('HH:mm').format(a.scheduledAt), style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(a.customerName ?? 'Customer', style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary, fontSize: 13)),
                Text(a.serviceName ?? '', style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
              ],
            ),
          ),
          StatusBadge(status: a.stateName),
        ],
      ),
    );
  }
}
