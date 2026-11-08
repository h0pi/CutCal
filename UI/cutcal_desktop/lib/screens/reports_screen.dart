import 'dart:io';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:path_provider/path_provider.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/api_client_exception.dart';
import '../utils/format.dart';
import '../utils/utils_widgets.dart';

class ReportsScreen extends StatefulWidget {
  const ReportsScreen({super.key});

  @override
  State<ReportsScreen> createState() => _ReportsScreenState();
}

class _ReportsScreenState extends State<ReportsScreen> {
  List<SalonModel> _salons = [];
  int? _salonId;
  DateTime? _dateFrom;
  DateTime? _dateTo;
  Map<String, dynamic>? _summary;
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 100});
    setState(() => _salons = salons.items);
    await _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final summary = await context.read<ReportProvider>().getSummary(salonId: _salonId, dateFrom: _dateFrom, dateTo: _dateTo);
    setState(() {
      _summary = summary;
      _isLoading = false;
    });
  }

  Future<void> _pickDate({required bool isFrom}) async {
    final date = await showDatePicker(context: context, initialDate: DateTime.now(), firstDate: DateTime(2020), lastDate: DateTime(2030));
    if (date == null) return;
    setState(() => isFrom ? _dateFrom = date : _dateTo = date);
    _load();
  }

  void _applyPreset(String preset) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    setState(() {
      switch (preset) {
        case 'Today':
          _dateFrom = today;
          _dateTo = today;
          break;
        case 'Week':
          _dateFrom = today.subtract(Duration(days: today.weekday - 1));
          _dateTo = today;
          break;
        case 'Month':
          _dateFrom = DateTime(now.year, now.month, 1);
          _dateTo = today;
          break;
      }
    });
    _load();
  }

  Future<void> _downloadAppointmentsPdf() async {
    try {
      final bytes = await context.read<ReportProvider>().downloadAppointmentsPdf(salonId: _salonId, dateFrom: _dateFrom, dateTo: _dateTo);
      await _saveAndNotify(bytes, 'appointments-report.pdf');
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _downloadServicesPdf() async {
    try {
      final bytes = await context.read<ReportProvider>().downloadServicesPdf(salonId: _salonId, dateFrom: _dateFrom, dateTo: _dateTo);
      await _saveAndNotify(bytes, 'services-report.pdf');
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _saveAndNotify(List<int> bytes, String fileName) async {
    // The report is saved to disk and can be opened and printed from there.
    final dir = await getDownloadsDirectory() ?? await getApplicationDocumentsDirectory();
    final file = File('${dir.path}${Platform.pathSeparator}$fileName');
    await file.writeAsBytes(bytes);
    if (mounted) showSuccessSnackBar(context, 'Saved to ${file.path}');
  }

  @override
  Widget build(BuildContext context) {
    final appointmentsReport = _summary?['appointmentsReport'] as Map<String, dynamic>?;
    final servicesReport = _summary?['servicesReport'] as Map<String, dynamic>?;

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
                child: DropdownButtonFormField<int?>(
                  initialValue: _salonId,
                  isExpanded: true,
                  decoration: const InputDecoration(labelText: 'Salon', border: OutlineInputBorder(), isDense: true),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('All salons')),
                    ..._salons.map((s) => DropdownMenuItem(value: s.id, child: Text(s.name, overflow: TextOverflow.ellipsis))),
                  ],
                  onChanged: (v) {
                    setState(() => _salonId = v);
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
              for (final preset in const ['Today', 'Week', 'Month'])
                OutlinedButton(onPressed: () => _applyPreset(preset), child: Text(preset)),
            ],
          ),
          const SizedBox(height: 24),
          if (_isLoading)
            const LoadingIndicator()
          else ...[
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Summary', style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 8),
                    Text('Total appointments: ${appointmentsReport?['totalAppointments'] ?? '-'}'),
                    Text(
                      'Confirmed: ${appointmentsReport?['confirmedCount'] ?? '-'}  •  '
                      'Completed: ${appointmentsReport?['completedCount'] ?? '-'}  •  '
                      'Cancelled: ${appointmentsReport?['cancelledCount'] ?? '-'}',
                    ),
                    Text('Total revenue: ${formatMoney((appointmentsReport?['totalRevenue'] as num?) ?? 0, appointmentsReport?['currency'] as String?)}'),
                    Text('Average duration: ${(appointmentsReport?['averageDurationMinutes'] as num?)?.toStringAsFixed(0) ?? '-'} min'),
                    Text('Busiest day: ${appointmentsReport?['busiestDayOfWeek'] ?? '-'}'),
                    Text('Most popular service: ${servicesReport?['mostPopularService'] ?? '-'}'),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),
            Wrap(
              spacing: 12,
              children: [
                FilledButton.icon(
                  icon: const Icon(Icons.picture_as_pdf),
                  label: const Text('Download PDF - Appointments'),
                  onPressed: _downloadAppointmentsPdf,
                ),
                FilledButton.icon(
                  icon: const Icon(Icons.picture_as_pdf),
                  label: const Text('Download PDF - Services'),
                  onPressed: _downloadServicesPdf,
                ),
              ],
            ),
          ],
        ],
      ),
    );
  }
}
