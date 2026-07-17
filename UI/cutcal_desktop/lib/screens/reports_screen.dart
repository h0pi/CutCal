import 'dart:io';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:path_provider/path_provider.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
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

  Future<void> _downloadAppointmentsPdf() async {
    final bytes = await context.read<ReportProvider>().downloadAppointmentsPdf(salonId: _salonId, dateFrom: _dateFrom, dateTo: _dateTo);
    await _saveAndNotify(bytes, 'appointments-report.pdf');
  }

  Future<void> _downloadServicesPdf() async {
    final bytes = await context.read<ReportProvider>().downloadServicesPdf(salonId: _salonId);
    await _saveAndNotify(bytes, 'services-report.pdf');
  }

  Future<void> _saveAndNotify(List<int> bytes, String fileName) async {
    // TODO: use the `printing` package to also offer a direct print dialog;
    // for now the report is saved to disk and can be opened/printed manually.
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
            ],
          ),
          const SizedBox(height: 24),
          if (_isLoading)
            const LoadingIndicator()
          else ...[
            // TODO: render an actual bar/line chart (e.g. fl_chart) instead of this placeholder summary.
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text('Summary', style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 8),
                    Text('Total appointments: ${appointmentsReport?['totalAppointments'] ?? '-'}'),
                    Text('Confirmed: ${appointmentsReport?['confirmedCount'] ?? '-'}  •  Cancelled: ${appointmentsReport?['cancelledCount'] ?? '-'}'),
                    Text('Total revenue: \$${appointmentsReport?['totalRevenue'] ?? 0}'),
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
