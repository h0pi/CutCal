import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

const _dayNames = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  List<SalonModel> _salons = [];
  SalonModel? _selectedSalon;
  late List<SalonWorkingHoursModel> _hours;
  bool _autoConfirm = false;
  String _currency = 'USD';
  bool _is24HourFormat = true;

  @override
  void initState() {
    super.initState();
    _hours = List.generate(7, (i) => SalonWorkingHoursModel(dayOfWeek: i, openTime: '09:00', closeTime: '20:00'));
    _load();
  }

  Future<void> _load() async {
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 100});
    setState(() {
      _salons = salons.items;
      if (_salons.isNotEmpty) _selectSalon(_salons.first);
    });
  }

  void _selectSalon(SalonModel salon) {
    setState(() {
      _selectedSalon = salon;
      _autoConfirm = salon.autoConfirm;
      _hours = List.generate(7, (i) {
        final existing = salon.workingHours.where((wh) => wh.dayOfWeek == i).toList();
        return existing.isNotEmpty ? existing.first : SalonWorkingHoursModel(dayOfWeek: i, openTime: '09:00', closeTime: '20:00');
      });
    });
  }

  Future<void> _pickTime(int dayIndex, {required bool isOpen}) async {
    final time = await showTimePicker(context: context, initialTime: const TimeOfDay(hour: 9, minute: 0));
    if (time == null) return;
    final formatted = '${time.hour.toString().padLeft(2, '0')}:${time.minute.toString().padLeft(2, '0')}';
    setState(() {
      final wh = _hours[dayIndex];
      _hours[dayIndex] = SalonWorkingHoursModel(
        dayOfWeek: wh.dayOfWeek,
        openTime: isOpen ? formatted : wh.openTime,
        closeTime: isOpen ? wh.closeTime : formatted,
        isClosed: wh.isClosed,
      );
    });
  }

  Future<void> _save() async {
    if (_selectedSalon == null) return;
    final salon = _selectedSalon!;
    await context.read<SalonProvider>().update(salon.id, {
      'name': salon.name,
      'salonCategoryId': salon.salonCategoryId,
      'description': salon.description,
      'address': salon.address,
      'cityId': salon.cityId,
      'latitude': salon.latitude,
      'longitude': salon.longitude,
      'phone': salon.phone,
      'email': salon.email,
      'profileImageUrl': salon.profileImageUrl,
      'autoConfirm': _autoConfirm,
      'workingHours': _hours.map((h) => h.toJson()).toList(),
    });
    if (mounted) showSuccessSnackBar(context, 'Settings saved.');
  }

  @override
  Widget build(BuildContext context) {
    if (_salons.isEmpty) return const LoadingIndicator();

    return Padding(
      padding: const EdgeInsets.all(24),
      child: ListView(
        children: [
          DropdownButtonFormField<SalonModel>(
            initialValue: _selectedSalon,
            decoration: const InputDecoration(labelText: 'Salon', border: OutlineInputBorder()),
            items: _salons.map((s) => DropdownMenuItem(value: s, child: Text(s.name))).toList(),
            onChanged: (v) {
              if (v != null) _selectSalon(v);
            },
          ),
          const SizedBox(height: 24),
          Text('Working hours', style: Theme.of(context).textTheme.titleMedium),
          ...List.generate(7, (i) {
            final wh = _hours[i];
            return Row(
              children: [
                SizedBox(width: 120, child: Text(_dayNames[i])),
                Checkbox(
                  value: wh.isClosed,
                  onChanged: (v) => setState(() => _hours[i] = SalonWorkingHoursModel(
                        dayOfWeek: i,
                        openTime: wh.openTime,
                        closeTime: wh.closeTime,
                        isClosed: v ?? false,
                      )),
                ),
                const Text('Closed'),
                const SizedBox(width: 16),
                if (!wh.isClosed) ...[
                  TextButton(onPressed: () => _pickTime(i, isOpen: true), child: Text('Open: ${wh.openTime}')),
                  TextButton(onPressed: () => _pickTime(i, isOpen: false), child: Text('Close: ${wh.closeTime}')),
                ],
              ],
            );
          }),
          const SizedBox(height: 24),
          SwitchListTile(
            value: _autoConfirm,
            title: const Text('Auto-confirm new bookings'),
            subtitle: const Text('When on, appointments skip the Pending state and are Confirmed immediately.'),
            onChanged: (v) => setState(() => _autoConfirm = v),
          ),
          const SizedBox(height: 16),
          Text('Display preferences', style: Theme.of(context).textTheme.titleMedium),
          DropdownButtonFormField<String>(
            initialValue: _currency,
            decoration: const InputDecoration(labelText: 'Currency'),
            items: const [
              DropdownMenuItem(value: 'USD', child: Text('USD')),
              DropdownMenuItem(value: 'EUR', child: Text('EUR')),
              DropdownMenuItem(value: 'BAM', child: Text('BAM')),
            ],
            onChanged: (v) => setState(() => _currency = v ?? 'USD'),
          ),
          SwitchListTile(
            value: _is24HourFormat,
            title: const Text('Use 24-hour time format'),
            onChanged: (v) => setState(() => _is24HourFormat = v),
          ),
          const SizedBox(height: 24),
          FilledButton(onPressed: _save, child: const Text('Save settings')),
        ],
      ),
    );
  }
}
