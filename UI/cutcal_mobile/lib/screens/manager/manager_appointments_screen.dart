import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/format.dart';
import '../../utils/utils_widgets.dart';

class ManagerAppointmentsScreen extends StatefulWidget {
  const ManagerAppointmentsScreen({super.key});

  @override
  State<ManagerAppointmentsScreen> createState() => _ManagerAppointmentsScreenState();
}

class _ManagerAppointmentsScreenState extends State<ManagerAppointmentsScreen> {
  static const _statuses = ['All', 'Pending', 'Confirmed', 'Completed', 'Cancelled'];
  static const _pageSize = 100; // the most the API returns per page
  String _status = 'All';
  List<AppointmentModel> _appointments = [];
  int _page = 0;
  int _totalCount = 0;
  bool _isLoading = true;
  bool _isLoadingMore = false;
  final _scrollController = ScrollController();
  String? _loadError;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _load(silent: true));
    _scrollController.addListener(() {
      if (_scrollController.position.extentAfter < 300) _loadMore();
    });
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    _scrollController.dispose();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    // Refreshing the first page would throw away the pages the manager has already scrolled to.
    if (silent && _page > 0) return;
    if (!silent) {
      setState(() {
        _isLoading = true;
        _loadError = null;
      });
    }
    try {
      // Auto-scoped server-side to salons this manager owns.
      final result = await context.read<AppointmentProvider>().get(filter: {
        'status': _status == 'All' ? null : _status,
        'page': 0,
        'pageSize': _pageSize,
      });
      if (!mounted) return;
      setState(() {
        _appointments = result.items;
        _totalCount = result.totalCount;
        _page = 0;
        _isLoading = false;
      });
    } on ApiClientException catch (e) {
      if (!mounted) return;
      if (silent) {
        debugPrint('Silent manager appointments refresh failed: ${e.message}');
        return;
      }
      setState(() {
        _loadError = e.message;
        _isLoading = false;
      });
    }
  }

  Future<void> _loadMore() async {
    if (_isLoadingMore || _appointments.length >= _totalCount) return;
    setState(() => _isLoadingMore = true);
    try {
      final result = await context.read<AppointmentProvider>().get(filter: {
        'status': _status == 'All' ? null : _status,
        'page': _page + 1,
        'pageSize': _pageSize,
      });
      if (!mounted) return;
      setState(() {
        _appointments = [..._appointments, ...result.items];
        _page++;
      });
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isLoadingMore = false);
    }
  }

  Future<void> _confirm(AppointmentModel a) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm appointment',
      message: 'Confirm this appointment for ${a.customerName ?? 'this customer'}?',
      confirmLabel: 'Confirm',
      isDestructive: false,
    );
    if (!confirmed) return;
    try {
      await context.read<AppointmentProvider>().confirm(a.id);
      if (mounted) showSuccessSnackBar(context, 'Appointment confirmed.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _complete(AppointmentModel a) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Mark as completed',
      message: 'Mark this appointment as completed?',
      confirmLabel: 'Mark completed',
      isDestructive: false,
    );
    if (!confirmed) return;
    try {
      await context.read<AppointmentProvider>().complete(a.id);
      if (mounted) showSuccessSnackBar(context, 'Appointment marked as completed.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _cancel(AppointmentModel a) async {
    final reason = await showCancelReasonDialog(context);
    if (reason == null) return;

    try {
      await context.read<AppointmentProvider>().cancel(a.id, reason);
      if (mounted) showSuccessSnackBar(context, 'Appointment cancelled.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Bookings')),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 8),
            child: FilterChipRow(
              labels: _statuses,
              selectedIndex: _statuses.indexOf(_status),
              onChanged: (i) {
                setState(() => _status = _statuses[i]);
                _load();
              },
            ),
          ),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : _loadError != null
                    ? ErrorState(message: _loadError!, onRetry: _load)
                    : _appointments.isEmpty
                        ? const Center(child: Text('No appointments here.', style: TextStyle(color: AppColors.textSecondary)))
                        : ListView.separated(
                        controller: _scrollController,
                        padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
                        itemCount: _appointments.length,
                        separatorBuilder: (_, __) => const SizedBox(height: 12),
                        itemBuilder: (context, index) {
                          final a = _appointments[index];
                          return Container(
                            padding: const EdgeInsets.all(14),
                            decoration: BoxDecoration(
                              color: Colors.white,
                              borderRadius: BorderRadius.circular(16),
                              border: Border.all(color: const Color(0xFFEDEAF4)),
                            ),
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Row(
                                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                  children: [
                                    Expanded(
                                      child: Text(
                                        '${a.customerName ?? 'Customer'} • ${a.serviceName ?? ''}',
                                        style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                                        overflow: TextOverflow.ellipsis,
                                      ),
                                    ),
                                    StatusBadge(status: a.stateName),
                                  ],
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  '${a.staffName ?? ''} • ${formatDateTime(a.scheduledAt, is24Hour: a.is24HourFormat)}',
                                  style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                                ),
                                if (a.cancellationReason != null)
                                  Padding(
                                    padding: const EdgeInsets.only(top: 4),
                                    child: Text('Reason: ${a.cancellationReason}', style: const TextStyle(color: AppColors.declinedText, fontSize: 12)),
                                  ),
                                if (a.stateName == 'Pending' || a.stateName == 'Confirmed') ...[
                                  const SizedBox(height: 10),
                                  Row(
                                    children: [
                                      if (a.stateName == 'Pending')
                                        Expanded(
                                          child: FilledButton(
                                            style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(38)),
                                            onPressed: () => _confirm(a),
                                            child: const Text('Confirm', style: TextStyle(fontSize: 13)),
                                          ),
                                        ),
                                      if (a.stateName == 'Confirmed')
                                        Expanded(
                                          child: FilledButton(
                                            style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(38)),
                                            onPressed: () => _complete(a),
                                            child: const Text('Complete', style: TextStyle(fontSize: 13)),
                                          ),
                                        ),
                                      const SizedBox(width: 8),
                                      Expanded(
                                        child: OutlinedButton(
                                          style: OutlinedButton.styleFrom(
                                            minimumSize: const Size.fromHeight(38),
                                            foregroundColor: AppColors.declinedText,
                                            side: const BorderSide(color: Color(0xFFFCA5A5)),
                                          ),
                                          onPressed: () => _cancel(a),
                                          child: const Text('Cancel', style: TextStyle(fontSize: 13)),
                                        ),
                                      ),
                                    ],
                                  ),
                                ],
                              ],
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
