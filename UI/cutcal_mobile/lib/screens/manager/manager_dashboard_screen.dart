import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class ManagerDashboardScreen extends StatefulWidget {
  const ManagerDashboardScreen({super.key});

  @override
  State<ManagerDashboardScreen> createState() => _ManagerDashboardScreenState();
}

class _ManagerDashboardScreenState extends State<ManagerDashboardScreen> {
  List<SalonModel> _mySalons = [];
  List<AppointmentModel> _todayAppointments = [];
  List<AppointmentModel> _pendingAppointments = [];
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

    // The backend automatically scopes both of these to salons this manager owns.
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 20});
    final appointments = await context.read<AppointmentProvider>().get(filter: {'pageSize': 200});

    final now = DateTime.now();
    final today = appointments.items.where((a) =>
        a.scheduledAt.year == now.year && a.scheduledAt.month == now.month && a.scheduledAt.day == now.day);
    final pending = appointments.items.where((a) => a.stateName == 'Pending');

    if (!mounted) return;
    setState(() {
      _mySalons = salons.items;
      _todayAppointments = today.toList();
      _pendingAppointments = pending.toList();
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
    final reasonController = TextEditingController();
    String? errorText;
    final reason = await showDialog<String>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: const Text('Reject appointment'),
          content: TextField(
            controller: reasonController,
            autofocus: true,
            decoration: InputDecoration(labelText: 'Reason', border: const OutlineInputBorder(), errorText: errorText),
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
                  setDialogState(() => errorText = 'A reason is required.');
                  return;
                }
                Navigator.of(context).pop(reasonController.text.trim());
              },
              child: const Text('Reject'),
            ),
          ],
        ),
      ),
    );
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
    final salon = _mySalons.firstOrNull;

    return Scaffold(
      appBar: AppBar(title: const Text('My Salon')),
      body: _isLoading
          ? const LoadingIndicator()
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                if (salon != null)
                  Card(
                    child: ListTile(
                      leading: salon.profileImageUrl != null
                          ? CircleAvatar(backgroundImage: NetworkImage(salon.profileImageUrl!))
                          : const CircleAvatar(child: Icon(Icons.storefront)),
                      title: Text(salon.name),
                      subtitle: Text(salon.isApproved ? 'Approved' : 'Pending admin approval'),
                    ),
                  )
                else
                  const Card(child: ListTile(title: Text('No salon assigned to your account yet.'))),
                const SizedBox(height: 16),
                Row(
                  children: [
                    Expanded(child: _kpiCard('Today', _todayAppointments.length.toString(), Icons.today)),
                    const SizedBox(width: 12),
                    Expanded(child: _kpiCard('Pending', _pendingAppointments.length.toString(), Icons.hourglass_top)),
                  ],
                ),
                const SizedBox(height: 24),
                Text('Pending requests', style: Theme.of(context).textTheme.titleMedium),
                const SizedBox(height: 8),
                if (_pendingAppointments.isEmpty)
                  const Text('No pending requests.', style: TextStyle(color: Colors.grey))
                else
                  ..._pendingAppointments.map((a) => Card(
                        child: Padding(
                          padding: const EdgeInsets.all(12),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('${a.customerName ?? 'Customer'} • ${a.serviceName ?? ''}', style: const TextStyle(fontWeight: FontWeight.w600)),
                              Text(DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)),
                              const SizedBox(height: 8),
                              Row(
                                children: [
                                  Expanded(
                                    child: OutlinedButton(
                                      style: OutlinedButton.styleFrom(foregroundColor: Colors.red),
                                      onPressed: () => _reject(a),
                                      child: const Text('Reject'),
                                    ),
                                  ),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: FilledButton(onPressed: () => _confirm(a), child: const Text('Confirm')),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      )),
              ],
            ),
    );
  }

  Widget _kpiCard(String label, String value, IconData icon) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: Colors.deepPurple),
            const SizedBox(height: 8),
            Text(value, style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold)),
            Text(label, style: const TextStyle(color: Colors.grey)),
          ],
        ),
      ),
    );
  }
}
