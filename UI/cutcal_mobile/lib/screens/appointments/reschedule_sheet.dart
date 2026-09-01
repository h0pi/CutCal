import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';

/// Shows the "Reschedule" bottom sheet for [appointment]. Returns true if the
/// appointment was successfully rescheduled (caller should refresh its data).
Future<bool> showRescheduleSheet(BuildContext context, AppointmentModel appointment) async {
  final result = await showModalBottomSheet<bool>(
    context: context,
    isScrollControlled: true,
    backgroundColor: Colors.white,
    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
    builder: (context) => _RescheduleSheet(appointment: appointment),
  );
  return result ?? false;
}

class _RescheduleSheet extends StatefulWidget {
  final AppointmentModel appointment;

  const _RescheduleSheet({required this.appointment});

  @override
  State<_RescheduleSheet> createState() => _RescheduleSheetState();
}

class _RescheduleSheetState extends State<_RescheduleSheet> {
  late DateTime _selectedDate;
  TimeOfDay? _selectedTime;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    _selectedDate = widget.appointment.scheduledAt;
  }

  Future<void> _confirm() async {
    if (_selectedTime == null) return;
    final newScheduledAt = DateTime(_selectedDate.year, _selectedDate.month, _selectedDate.day, _selectedTime!.hour, _selectedTime!.minute);

    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm new time',
      message: 'Move this appointment to ${DateFormat('MMM d, y • HH:mm').format(newScheduledAt)}?',
      confirmLabel: 'Confirm new time',
      isDestructive: false,
    );
    if (!confirmed) return;

    setState(() => _isSaving = true);
    try {
      await context.read<AppointmentProvider>().reschedule(widget.appointment.id, newScheduledAt);
      if (mounted) {
        showSuccessSnackBar(context, 'Appointment moved to ${DateFormat('MMM d, y • HH:mm').format(newScheduledAt)}.');
        Navigator.of(context).pop(true);
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final days = List.generate(14, (i) => DateTime.now().add(Duration(days: i + 1)));
    final times = <TimeOfDay>[
      for (var h = 9; h <= 18; h++) ...[TimeOfDay(hour: h, minute: 0), TimeOfDay(hour: h, minute: 30)],
    ];

    return Padding(
      padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
      child: SafeArea(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(20, 12, 20, 20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Center(
                child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey.shade300, borderRadius: BorderRadius.circular(2))),
              ),
              const SizedBox(height: 16),
              const Text('Reschedule', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
              Text(
                '${widget.appointment.serviceName ?? ''} · currently ${DateFormat('MMM d, y • HH:mm').format(widget.appointment.scheduledAt)}',
                style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
              ),
              const SizedBox(height: 20),
              SizedBox(
                height: 72,
                child: ListView.separated(
                  scrollDirection: Axis.horizontal,
                  itemCount: days.length,
                  separatorBuilder: (_, __) => const SizedBox(width: 8),
                  itemBuilder: (context, i) {
                    final day = days[i];
                    final selected = day.year == _selectedDate.year && day.month == _selectedDate.month && day.day == _selectedDate.day;
                    return GestureDetector(
                      onTap: () => setState(() {
                        _selectedDate = day;
                        _selectedTime = null;
                      }),
                      child: Container(
                        width: 56,
                        decoration: BoxDecoration(
                          color: selected ? AppColors.primary : Colors.white,
                          borderRadius: BorderRadius.circular(14),
                          border: selected ? null : Border.all(color: const Color(0xFFE5E1EF)),
                        ),
                        alignment: Alignment.center,
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Text(DateFormat('E').format(day), style: TextStyle(fontSize: 11, color: selected ? Colors.white70 : AppColors.textSecondary)),
                            const SizedBox(height: 4),
                            Text('${day.day}', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: selected ? Colors.white : AppColors.textPrimary)),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
              const SizedBox(height: 16),
              GridView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 3, mainAxisSpacing: 10, crossAxisSpacing: 10, childAspectRatio: 2.2),
                itemCount: times.length,
                itemBuilder: (context, i) {
                  final t = times[i];
                  final selected = _selectedTime == t;
                  return GestureDetector(
                    onTap: () => setState(() => _selectedTime = t),
                    child: Container(
                      decoration: BoxDecoration(
                        color: selected ? AppColors.primary : Colors.white,
                        borderRadius: BorderRadius.circular(12),
                        border: selected ? null : Border.all(color: const Color(0xFFE5E1EF)),
                      ),
                      alignment: Alignment.center,
                      child: Text(t.format(context), style: TextStyle(color: selected ? Colors.white : AppColors.textPrimary, fontWeight: FontWeight.w600, fontSize: 13)),
                    ),
                  );
                },
              ),
              const SizedBox(height: 20),
              SizedBox(
                width: double.infinity,
                child: FilledButton(
                  onPressed: (_selectedTime == null || _isSaving) ? null : _confirm,
                  child: _isSaving ? const SizedBox(height: 20, width: 20, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white)) : const Text('Confirm new time'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
