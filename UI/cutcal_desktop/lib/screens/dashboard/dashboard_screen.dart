import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';

class DashboardScreen extends StatefulWidget {
  const DashboardScreen({super.key});

  @override
  State<DashboardScreen> createState() => _DashboardScreenState();
}

class _DashboardScreenState extends State<DashboardScreen> {
  List<AppointmentModel> _todayAppointments = [];
  List<AppointmentModel> _pending = [];
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    _pollTimer = Timer.periodic(const Duration(seconds: 30), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    final today = DateTime.now();
    final startOfDay = DateTime(today.year, today.month, today.day);
    final endOfDay = startOfDay.add(const Duration(days: 1));

    final todayResult = await context.read<AppointmentProvider>().get(filter: {
      'dateFrom': startOfDay.toIso8601String(),
      'dateTo': endOfDay.toIso8601String(),
      'pageSize': 100,
    });
    final pendingResult = await context.read<AppointmentProvider>().get(filter: {'status': 'Pending', 'pageSize': 50});

    if (!mounted) return;
    setState(() {
      _todayAppointments = todayResult.items;
      _pending = pendingResult.items;
      _isLoading = false;
    });
  }

  Future<void> _confirm(AppointmentModel appointment) async {
    await context.read<AppointmentProvider>().confirm(appointment.id);
    _load();
  }

  Future<void> _reject(AppointmentModel appointment) async {
    final reasonController = TextEditingController();
    final reason = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Reject appointment'),
        content: TextField(
          controller: reasonController,
          decoration: const InputDecoration(labelText: 'Reason', border: OutlineInputBorder()),
        ),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Back')),
          TextButton(
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            onPressed: () => Navigator.of(context).pop(reasonController.text),
            child: const Text('Reject'),
          ),
        ],
      ),
    );
    if (reason == null || reason.trim().isEmpty) return;

    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm rejection',
      message: 'Reject this appointment request? The customer will be notified.',
    );
    if (!confirmed) return;

    await context.read<AppointmentProvider>().cancel(appointment.id, reason);
    _load();
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading) return const LoadingIndicator();

    final revenueToday = _todayAppointments.where((a) => a.paymentStatus == 'Paid').fold<double>(0, (sum, a) => sum + a.price);
    final newCustomers = _todayAppointments.map((a) => a.customerId).toSet().length;

    return ListView(
      padding: const EdgeInsets.all(24),
      children: [
        Wrap(
          spacing: 16,
          runSpacing: 16,
          children: [
            _kpiCard('Appointments today', _todayAppointments.length.toString(), Icons.event),
            _kpiCard('Revenue today', '\$${revenueToday.toStringAsFixed(2)}', Icons.attach_money),
            _kpiCard('Customers today', newCustomers.toString(), Icons.person),
            _kpiCard('Pending requests', _pending.length.toString(), Icons.hourglass_top),
          ],
        ),
        const SizedBox(height: 32),
        Text('Pending requests', style: Theme.of(context).textTheme.titleLarge),
        const SizedBox(height: 8),
        if (_pending.isEmpty)
          const Text('No pending requests.')
        else
          ..._pending.map((a) => Card(
                child: ListTile(
                  title: Text('${a.salonName ?? ''} • ${a.serviceName ?? ''}'),
                  subtitle: Text(DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)),
                  trailing: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      FilledButton(onPressed: () => _confirm(a), child: const Text('Accept')),
                      const SizedBox(width: 8),
                      OutlinedButton(
                        style: OutlinedButton.styleFrom(foregroundColor: Colors.red),
                        onPressed: () => _reject(a),
                        child: const Text('Reject'),
                      ),
                    ],
                  ),
                ),
              )),
      ],
    );
  }

  Widget _kpiCard(String label, String value, IconData icon) {
    return SizedBox(
      width: 220,
      child: Card(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            children: [
              Icon(icon, size: 32, color: Colors.deepPurple),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(value, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
                    Text(label, style: const TextStyle(color: Colors.grey)),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
