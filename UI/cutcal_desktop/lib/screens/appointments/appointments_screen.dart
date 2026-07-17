import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';

class AppointmentsScreen extends StatefulWidget {
  const AppointmentsScreen({super.key});

  @override
  State<AppointmentsScreen> createState() => _AppointmentsScreenState();
}

class _AppointmentsScreenState extends State<AppointmentsScreen> {
  final _customerNameController = TextEditingController();
  String? _status;
  DateTime? _dateFrom;
  DateTime? _dateTo;
  List<AppointmentModel> _appointments = [];
  int _page = 0;
  int _totalCount = 0;
  static const _pageSize = 10;
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    // Appointments can change from the mobile side (customer books/cancels) at any
    // time, so this keeps the table current without the manager having to refresh.
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    final result = await context.read<AppointmentProvider>().get(filter: {
      'page': _page,
      'pageSize': _pageSize,
      'status': _status,
      'dateFrom': _dateFrom?.toIso8601String(),
      'dateTo': _dateTo?.toIso8601String(),
    });
    var items = result.items;
    if (_customerNameController.text.isNotEmpty) {
      final query = _customerNameController.text.toLowerCase();
      items = items.where((a) => (a.customerName ?? '').toLowerCase().contains(query)).toList();
    }
    if (!mounted) return;
    setState(() {
      _appointments = items;
      _totalCount = result.totalCount;
      _isLoading = false;
    });
  }

  Future<void> _pickDate({required bool isFrom}) async {
    final date = await showDatePicker(context: context, initialDate: DateTime.now(), firstDate: DateTime(2020), lastDate: DateTime(2030));
    if (date == null) return;
    setState(() => isFrom ? _dateFrom = date : _dateTo = date);
    _load();
  }

  Future<void> _changeStatus(AppointmentModel appointment, String action) async {
    final provider = context.read<AppointmentProvider>();
    if (action == 'Confirm') {
      await provider.confirm(appointment.id);
    } else if (action == 'Complete') {
      await provider.complete(appointment.id);
    } else if (action == 'Cancel') {
      final reasonController = TextEditingController();
      final reason = await showDialog<String>(
        context: context,
        builder: (context) => AlertDialog(
          title: const Text('Cancel appointment'),
          content: TextField(controller: reasonController, decoration: const InputDecoration(labelText: 'Reason')),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Back')),
            TextButton(
              style: TextButton.styleFrom(foregroundColor: Colors.red),
              onPressed: () => Navigator.of(context).pop(reasonController.text),
              child: const Text('Confirm cancel'),
            ),
          ],
        ),
      );
      if (reason == null || reason.trim().isEmpty) return;
      final confirmed = await showConfirmationDialog(context, title: 'Cancel appointment', message: 'Are you sure?');
      if (!confirmed) return;
      await provider.cancel(appointment.id, reason);
    }
    _load();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Wrap(
            spacing: 12,
            runSpacing: 12,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              SizedBox(
                width: 220,
                child: TextField(
                  controller: _customerNameController,
                  decoration: const InputDecoration(labelText: 'Customer name', border: OutlineInputBorder(), isDense: true),
                  onSubmitted: (_) => _load(),
                ),
              ),
              SizedBox(
                width: 180,
                child: DropdownButtonFormField<String?>(
                  initialValue: _status,
                  decoration: const InputDecoration(labelText: 'Status', border: OutlineInputBorder(), isDense: true),
                  items: const [
                    DropdownMenuItem(value: null, child: Text('All')),
                    DropdownMenuItem(value: 'Pending', child: Text('Pending')),
                    DropdownMenuItem(value: 'Confirmed', child: Text('Confirmed')),
                    DropdownMenuItem(value: 'Completed', child: Text('Completed')),
                    DropdownMenuItem(value: 'Cancelled', child: Text('Cancelled')),
                  ],
                  onChanged: (v) {
                    setState(() => _status = v);
                    _load();
                  },
                ),
              ),
              OutlinedButton.icon(
                icon: const Icon(Icons.date_range),
                label: Text(_dateFrom == null ? 'From date' : DateFormat('MMM d').format(_dateFrom!)),
                onPressed: () => _pickDate(isFrom: true),
              ),
              OutlinedButton.icon(
                icon: const Icon(Icons.date_range),
                label: Text(_dateTo == null ? 'To date' : DateFormat('MMM d').format(_dateTo!)),
                onPressed: () => _pickDate(isFrom: false),
              ),
              FilledButton.icon(icon: const Icon(Icons.search), label: const Text('Search'), onPressed: _load),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : SingleChildScrollView(
                    child: DataTable(
                      columns: const [
                        DataColumn(label: Text('Customer')),
                        DataColumn(label: Text('Salon')),
                        DataColumn(label: Text('Service')),
                        DataColumn(label: Text('Date')),
                        DataColumn(label: Text('Status')),
                        DataColumn(label: Text('Price')),
                        DataColumn(label: Text('Actions')),
                      ],
                      rows: _appointments
                          .map((a) => DataRow(cells: [
                                DataCell(Text(a.customerName ?? '')),
                                DataCell(Text(a.salonName ?? '')),
                                DataCell(Text(a.serviceName ?? '')),
                                DataCell(Text(DateFormat('MMM d, HH:mm').format(a.scheduledAt))),
                                DataCell(StatusBadge(status: a.stateName)),
                                DataCell(Text('\$${a.price.toStringAsFixed(2)}')),
                                DataCell(_actionsFor(a)),
                              ]))
                          .toList(),
                    ),
                  ),
          ),
          Row(
            mainAxisAlignment: MainAxisAlignment.end,
            children: [
              IconButton(
                icon: const Icon(Icons.chevron_left),
                onPressed: _page > 0 ? () { setState(() => _page--); _load(); } : null,
              ),
              Text('Page ${_page + 1} of ${(_totalCount / _pageSize).ceil().clamp(1, 999)}'),
              IconButton(
                icon: const Icon(Icons.chevron_right),
                onPressed: (_page + 1) * _pageSize < _totalCount ? () { setState(() => _page++); _load(); } : null,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _actionsFor(AppointmentModel a) {
    return PopupMenuButton<String>(
      onSelected: (action) => _changeStatus(a, action),
      itemBuilder: (context) => [
        if (a.stateName == 'Pending') const PopupMenuItem(value: 'Confirm', child: Text('Confirm')),
        if (a.stateName == 'Confirmed') const PopupMenuItem(value: 'Complete', child: Text('Complete')),
        if (a.stateName == 'Pending' || a.stateName == 'Confirmed') const PopupMenuItem(value: 'Cancel', child: Text('Cancel')),
      ],
    );
  }
}
