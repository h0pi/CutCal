import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/app_theme.dart';
import 'payment_screen.dart';

class BookingScreen extends StatefulWidget {
  final SalonModel salon;

  const BookingScreen({super.key, required this.salon});

  @override
  State<BookingScreen> createState() => _BookingScreenState();
}

class _BookingScreenState extends State<BookingScreen> {
  static const _stepTitles = ['Select a service', 'Choose your stylist', 'Pick date & time'];

  int _step = 0;
  List<SalonServiceModel> _services = [];
  List<StaffModel> _staff = [];
  SalonServiceModel? _selectedService;
  StaffModel? _selectedStaff;
  DateTime _selectedDate = DateTime.now().add(const Duration(days: 1));
  TimeOfDay? _selectedTime;

  @override
  void initState() {
    super.initState();
    _loadServices();
  }

  Future<void> _loadServices() async {
    final result = await context.read<SalonServiceProvider>().get(filter: {'salonId': widget.salon.id, 'isActive': true});
    if (mounted) setState(() => _services = result.items);
  }

  Future<void> _loadStaff() async {
    final result = await context.read<StaffProvider>().get(filter: {'salonId': widget.salon.id, 'isActive': true});
    if (mounted) {
      setState(() => _staff = result.items.where((s) => s.serviceIds.contains(_selectedService!.id)).toList());
    }
  }

  bool get _canContinue => switch (_step) {
        0 => _selectedService != null,
        1 => _selectedStaff != null,
        2 => _selectedTime != null,
        _ => false,
      };

  void _onContinue() {
    if (!_canContinue) return;
    if (_step < 2) {
      setState(() => _step++);
      return;
    }

    final scheduledAt = DateTime(
      _selectedDate.year,
      _selectedDate.month,
      _selectedDate.day,
      _selectedTime!.hour,
      _selectedTime!.minute,
    );

    Navigator.of(context).push(
      MaterialPageRoute(
        builder: (_) => PaymentScreen(
          salon: widget.salon,
          service: _selectedService!,
          staff: _selectedStaff!,
          scheduledAt: scheduledAt,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(_stepTitles[_step], style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold)),
            Text('Step ${_step + 1} of 3 · ${widget.salon.name}', style: const TextStyle(fontSize: 12, color: AppColors.textSecondary, fontWeight: FontWeight.normal)),
          ],
        ),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 16),
            child: Row(
              children: List.generate(3, (i) {
                final filled = i <= _step;
                return Expanded(
                  child: Container(
                    margin: EdgeInsets.only(right: i < 2 ? 6 : 0),
                    height: 4,
                    decoration: BoxDecoration(
                      color: filled ? AppColors.primary : AppColors.primaryLight,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                );
              }),
            ),
          ),
          Expanded(child: _buildStepContent()),
        ],
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _canContinue ? _onContinue : null,
              child: Text(_step == 2 ? (_selectedTime == null ? 'Pick a time to continue' : 'Continue') : 'Continue'),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildStepContent() {
    switch (_step) {
      case 0:
        return ListView.builder(
          padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
          itemCount: _services.length,
          itemBuilder: (context, i) {
            final s = _services[i];
            final selected = _selectedService?.id == s.id;
            return _SelectableRow(
              selected: selected,
              onTap: () {
                setState(() {
                  _selectedService = s;
                  _selectedStaff = null;
                });
                _loadStaff();
              },
              title: s.name,
              subtitle: '${s.durationMinutes} min · ${s.description ?? ''}',
              trailing: '\$${s.price.toStringAsFixed(0)}',
            );
          },
        );
      case 1:
        if (_selectedService == null) {
          return const Center(child: Text('Select a service first.', style: TextStyle(color: AppColors.textSecondary)));
        }
        if (_staff.isEmpty) {
          return const Padding(
            padding: EdgeInsets.all(16),
            child: Text(
              'No staff members are currently assigned to this service. Please choose a different service, or check back later.',
              style: TextStyle(color: Colors.red),
            ),
          );
        }
        return ListView.builder(
          padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
          itemCount: _staff.length,
          itemBuilder: (context, i) {
            final s = _staff[i];
            final selected = _selectedStaff?.id == s.id;
            return _SelectableRow(
              selected: selected,
              onTap: () => setState(() => _selectedStaff = s),
              leading: CircleAvatar(
                radius: 18,
                backgroundColor: AppColors.primaryLight,
                backgroundImage: s.profileImageUrl != null ? NetworkImage(s.profileImageUrl!) : null,
                child: s.profileImageUrl == null ? const Icon(Icons.person, size: 18, color: AppColors.primary) : null,
              ),
              title: s.fullName ?? 'Staff #${s.id}',
              subtitle: s.role,
            );
          },
        );
      case 2:
        return _buildDateTimeStep();
      default:
        return const SizedBox.shrink();
    }
  }

  Widget _buildDateTimeStep() {
    final days = List.generate(14, (i) => DateTime.now().add(Duration(days: i + 1)));
    final times = <TimeOfDay>[
      for (var h = 9; h <= 18; h++) ...[TimeOfDay(hour: h, minute: 0), TimeOfDay(hour: h, minute: 30)],
    ];

    return ListView(
      padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
      children: [
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
        const SizedBox(height: 20),
        const Text('AVAILABLE TIMES', style: TextStyle(color: AppColors.textSecondary, fontSize: 11, fontWeight: FontWeight.w700, letterSpacing: 0.4)),
        const SizedBox(height: 10),
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
                child: Text(
                  t.format(context),
                  style: TextStyle(color: selected ? Colors.white : AppColors.textPrimary, fontWeight: FontWeight.w600, fontSize: 13),
                ),
              ),
            );
          },
        ),
        const SizedBox(height: 20),
        Container(
          padding: const EdgeInsets.all(16),
          decoration: BoxDecoration(color: AppColors.surfaceMuted, borderRadius: BorderRadius.circular(16)),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text('Your booking', style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
              const SizedBox(height: 10),
              _summaryRow('Service', _selectedService?.name ?? '-'),
              _summaryRow('Stylist', _selectedStaff?.fullName ?? '-'),
              _summaryRow('When', '${DateFormat('MMM d').format(_selectedDate)} · ${_selectedTime?.format(context) ?? 'pick a time'}'),
              _summaryRow('Duration', '${_selectedService?.durationMinutes ?? 0} min'),
              _summaryRow('Price', '\$${(_selectedService?.price ?? 0).toStringAsFixed(2)}'),
            ],
          ),
        ),
      ],
    );
  }

  Widget _summaryRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(label, style: const TextStyle(color: AppColors.textSecondary, fontSize: 13)),
          Text(value, style: const TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.w600, fontSize: 13)),
        ],
      ),
    );
  }
}

class _SelectableRow extends StatelessWidget {
  final bool selected;
  final VoidCallback onTap;
  final Widget? leading;
  final String title;
  final String subtitle;
  final String? trailing;

  const _SelectableRow({
    required this.selected,
    required this.onTap,
    this.leading,
    required this.title,
    required this.subtitle,
    this.trailing,
  });

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.only(bottom: 10),
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: selected ? AppColors.primaryLight : Colors.white,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: selected ? AppColors.primary : const Color(0xFFEDEAF5), width: selected ? 1.5 : 1),
        ),
        child: Row(
          children: [
            if (leading != null) ...[leading!, const SizedBox(width: 12)],
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(title, style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
                  const SizedBox(height: 2),
                  Text(subtitle, style: const TextStyle(color: AppColors.textSecondary, fontSize: 12), overflow: TextOverflow.ellipsis),
                ],
              ),
            ),
            if (trailing != null)
              Text(trailing!, style: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.bold)),
          ],
        ),
      ),
    );
  }
}
