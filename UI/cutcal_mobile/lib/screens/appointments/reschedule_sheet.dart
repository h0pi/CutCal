import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import 'availability_picker.dart';

/// Shows the "Reschedule" bottom sheet for [appointment]. Returns true if the
/// appointment was successfully rescheduled (caller should refresh its data).
Future<bool> showRescheduleSheet(BuildContext context, AppointmentModel appointment) async {
  final result = await showModalBottomSheet<bool>(
    context: context,
    isScrollControlled: true,
    backgroundColor: Colors.white,
    constraints: BoxConstraints(maxHeight: MediaQuery.of(context).size.height * 0.92),
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
  DateTime? _selectedStart;
  bool _isSaving = false;

  Future<void> _confirm() async {
    final newScheduledAt = _selectedStart;
    if (newScheduledAt == null) return;

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
    final a = widget.appointment;

    return Padding(
      padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
      child: SafeArea(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Center(
                    child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey.shade300, borderRadius: BorderRadius.circular(2))),
                  ),
                  const SizedBox(height: 16),
                  const Text('Reschedule', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                  Text(
                    '${a.serviceName ?? ''} · currently ${DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)}',
                    style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                  ),
                  const SizedBox(height: 16),
                ],
              ),
            ),
            Flexible(
              child: SingleChildScrollView(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: AvailabilityPicker(
                  salonId: a.salonId,
                  serviceId: a.serviceId,
                  staffId: a.staffId,
                  excludeAppointmentId: a.id,
                  onChanged: (start) => setState(() => _selectedStart = start),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 16),
              child: SizedBox(
                width: double.infinity,
                child: FilledButton(
                  onPressed: (_selectedStart == null || _isSaving) ? null : _confirm,
                  child: _isSaving
                      ? const SizedBox(height: 20, width: 20, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                      : const Text('Confirm new time'),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
