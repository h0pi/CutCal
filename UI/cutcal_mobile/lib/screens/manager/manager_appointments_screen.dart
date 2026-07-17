import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class ManagerAppointmentsScreen extends StatefulWidget {
  const ManagerAppointmentsScreen({super.key});

  @override
  State<ManagerAppointmentsScreen> createState() => _ManagerAppointmentsScreenState();
}

class _ManagerAppointmentsScreenState extends State<ManagerAppointmentsScreen> {
  static const _statuses = ['All', 'Pending', 'Confirmed', 'Completed', 'Cancelled'];
  String _status = 'All';
  List<AppointmentModel> _appointments = [];
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    // Auto-scoped server-side to salons this manager owns.
    final result = await context.read<AppointmentProvider>().get(filter: {
      'status': _status == 'All' ? null : _status,
      'pageSize': 200,
    });
    if (!mounted) return;
    setState(() {
      _appointments = result.items;
      _isLoading = false;
    });
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
              child: const Text('Cancel appointment'),
            ),
          ],
        ),
      ),
    );
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
      appBar: AppBar(title: const Text('Salon Appointments')),
      body: Column(
        children: [
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 8),
              children: _statuses
                  .map((s) => Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 4),
                        child: ChoiceChip(
                          label: Text(s),
                          selected: _status == s,
                          onSelected: (_) {
                            setState(() => _status = s);
                            _load();
                          },
                        ),
                      ))
                  .toList(),
            ),
          ),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : _appointments.isEmpty
                    ? const Center(child: Text('No appointments here.'))
                    : ListView.builder(
                        itemCount: _appointments.length,
                        itemBuilder: (context, index) {
                          final a = _appointments[index];
                          return Card(
                            margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
                            child: Padding(
                              padding: const EdgeInsets.all(12),
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: [
                                      Expanded(
                                        child: Text(
                                          '${a.customerName ?? 'Customer'} • ${a.serviceName ?? ''}',
                                          style: const TextStyle(fontWeight: FontWeight.w600),
                                          overflow: TextOverflow.ellipsis,
                                        ),
                                      ),
                                      StatusBadge(status: a.stateName),
                                    ],
                                  ),
                                  Text('${a.staffName ?? ''} • ${DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)}'),
                                  if (a.cancellationReason != null)
                                    Padding(
                                      padding: const EdgeInsets.only(top: 4),
                                      child: Text('Reason: ${a.cancellationReason}', style: const TextStyle(color: Colors.red, fontSize: 12)),
                                    ),
                                  if (a.stateName == 'Pending' || a.stateName == 'Confirmed') ...[
                                    const SizedBox(height: 8),
                                    Row(
                                      children: [
                                        if (a.stateName == 'Pending')
                                          Expanded(
                                            child: FilledButton(onPressed: () => _confirm(a), child: const Text('Confirm')),
                                          ),
                                        if (a.stateName == 'Confirmed')
                                          Expanded(
                                            child: FilledButton(onPressed: () => _complete(a), child: const Text('Complete')),
                                          ),
                                        const SizedBox(width: 8),
                                        Expanded(
                                          child: OutlinedButton(
                                            style: OutlinedButton.styleFrom(foregroundColor: Colors.red),
                                            onPressed: () => _cancel(a),
                                            child: const Text('Cancel'),
                                          ),
                                        ),
                                      ],
                                    ),
                                  ],
                                ],
                              ),
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
