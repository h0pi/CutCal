import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class BookingScreen extends StatefulWidget {
  final SalonModel salon;

  const BookingScreen({super.key, required this.salon});

  @override
  State<BookingScreen> createState() => _BookingScreenState();
}

class _BookingScreenState extends State<BookingScreen> {
  int _step = 0;
  List<SalonServiceModel> _services = [];
  List<StaffModel> _staff = [];
  SalonServiceModel? _selectedService;
  StaffModel? _selectedStaff;
  DateTime? _selectedDate;
  TimeOfDay? _selectedTime;
  String _paymentMethod = 'Cash';
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadServices();
  }

  Future<void> _loadServices() async {
    final result = await context.read<SalonServiceProvider>().get(filter: {'salonId': widget.salon.id, 'isActive': true});
    setState(() => _services = result.items);
  }

  Future<void> _loadStaff() async {
    final result = await context.read<StaffProvider>().get(filter: {'salonId': widget.salon.id, 'isActive': true});
    setState(() => _staff = result.items.where((s) => s.serviceIds.contains(_selectedService!.id)).toList());
  }

  Future<void> _submit() async {
    if (_selectedDate == null || _selectedTime == null) return;

    final confirmed = await showConfirmationDialog(
      context,
      title: 'Confirm booking',
      message:
          'Book ${_selectedService!.name} with ${_selectedStaff!.fullName ?? 'staff'} on ${_selectedDate!.toLocal().toString().split(' ').first} at ${_selectedTime!.format(context)}?',
      confirmLabel: 'Book',
      isDestructive: false,
    );
    if (!confirmed) return;

    final scheduledAt = DateTime(
      _selectedDate!.year,
      _selectedDate!.month,
      _selectedDate!.day,
      _selectedTime!.hour,
      _selectedTime!.minute,
    );

    setState(() => _isLoading = true);
    try {
      await context.read<AppointmentProvider>().insert({
        'salonId': widget.salon.id,
        'staffId': _selectedStaff!.id,
        'serviceId': _selectedService!.id,
        'scheduledAt': scheduledAt.toIso8601String(),
        'paymentMethod': _paymentMethod,
      });
      if (mounted) {
        showSuccessSnackBar(
          context,
          'Appointment booked for ${scheduledAt.toLocal().toString().split(' ').first} at ${_selectedTime!.format(context)}',
        );
        Navigator.of(context).popUntil((route) => route.isFirst);
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Book at ${widget.salon.name}')),
      body: Stepper(
        currentStep: _step,
        controlsBuilder: (context, details) => const SizedBox.shrink(),
        steps: [
          Step(
            title: const Text('Service'),
            isActive: _step >= 0,
            content: Column(
              children: _services
                  .map((s) => RadioListTile<SalonServiceModel>(
                        value: s,
                        groupValue: _selectedService,
                        title: Text('${s.name}  •  \$${s.price.toStringAsFixed(2)}'),
                        subtitle: Text('${s.durationMinutes} min'),
                        onChanged: (v) {
                          setState(() {
                            _selectedService = v;
                            _selectedStaff = null;
                          });
                          _loadStaff();
                        },
                      ))
                  .toList(),
            ),
          ),
          Step(
            title: const Text('Staff'),
            isActive: _step >= 1,
            content: _selectedService == null
                ? const Text('Select a service first.')
                : _staff.isEmpty
                    ? const Padding(
                        padding: EdgeInsets.symmetric(vertical: 8),
                        child: Text(
                          'No staff members are currently assigned to this service. '
                          'Please choose a different service, or check back later.',
                          style: TextStyle(color: Colors.red),
                        ),
                      )
                    : Column(
                        children: _staff
                            .map((s) => RadioListTile<StaffModel>(
                                  value: s,
                                  groupValue: _selectedStaff,
                                  title: Text(s.fullName ?? 'Staff #${s.id}'),
                                  subtitle: Text(s.role),
                                  onChanged: (v) => setState(() => _selectedStaff = v),
                                ))
                            .toList(),
                      ),
          ),
          Step(
            title: const Text('Date & Time'),
            isActive: _step >= 2,
            content: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ListTile(
                  leading: const Icon(Icons.calendar_today),
                  title: Text(_selectedDate == null ? 'Pick a date' : _selectedDate!.toLocal().toString().split(' ').first),
                  onTap: () async {
                    final date = await showDatePicker(
                      context: context,
                      initialDate: DateTime.now().add(const Duration(days: 1)),
                      firstDate: DateTime.now(),
                      lastDate: DateTime.now().add(const Duration(days: 90)),
                    );
                    if (date != null) setState(() => _selectedDate = date);
                  },
                ),
                ListTile(
                  leading: const Icon(Icons.access_time),
                  title: Text(_selectedTime == null ? 'Pick a time' : _selectedTime!.format(context)),
                  onTap: () async {
                    final time = await showTimePicker(context: context, initialTime: const TimeOfDay(hour: 10, minute: 0));
                    if (time != null) setState(() => _selectedTime = time);
                  },
                ),
              ],
            ),
          ),
          Step(
            title: const Text('Review & Confirm'),
            isActive: _step >= 3,
            content: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text('Service: ${_selectedService?.name ?? '-'}'),
                Text('Staff: ${_selectedStaff?.fullName ?? '-'}'),
                Text('Date: ${_selectedDate?.toLocal().toString().split(' ').first ?? '-'}'),
                Text('Time: ${_selectedTime?.format(context) ?? '-'}'),
                const SizedBox(height: 12),
                const Text('Payment method'),
                RadioListTile<String>(
                  value: 'Cash',
                  groupValue: _paymentMethod,
                  title: const Text('Cash'),
                  onChanged: (v) => setState(() => _paymentMethod = v!),
                ),
                RadioListTile<String>(
                  value: 'PayPal',
                  groupValue: _paymentMethod,
                  title: const Text('PayPal'),
                  onChanged: (v) => setState(() => _paymentMethod = v!),
                ),
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  child: FilledButton(
                    onPressed: _isLoading ? null : _submit,
                    child: _isLoading ? const LoadingIndicator() : const Text('Confirm booking'),
                  ),
                ),
              ],
            ),
          ),
        ],
        onStepContinue: () {
          final canContinue = switch (_step) {
            0 => _selectedService != null,
            1 => _selectedStaff != null,
            2 => _selectedDate != null && _selectedTime != null,
            _ => true,
          };
          if (canContinue && _step < 3) setState(() => _step++);
        },
        onStepCancel: () {
          if (_step > 0) setState(() => _step--);
        },
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(12),
          child: Row(
            children: [
              if (_step > 0)
                Expanded(child: OutlinedButton(onPressed: () => setState(() => _step--), child: const Text('Back'))),
              if (_step > 0) const SizedBox(width: 12),
              if (_step < 3)
                Expanded(
                  child: FilledButton(
                    onPressed: () {
                      final canContinue = switch (_step) {
                        0 => _selectedService != null,
                        1 => _selectedStaff != null,
                        2 => _selectedDate != null && _selectedTime != null,
                        _ => true,
                      };
                      if (canContinue) setState(() => _step++);
                    },
                    child: const Text('Next'),
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
