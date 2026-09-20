import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';

/// Colour-coded month calendar plus a grid of free/taken start times for the chosen day.
/// All availability comes from the backend, so it reflects working hours and existing bookings.
class AvailabilityPicker extends StatefulWidget {
  final int salonId;
  final int serviceId;
  final int staffId;
  final int? excludeAppointmentId;
  final ValueChanged<DateTime?> onChanged;

  const AvailabilityPicker({
    super.key,
    required this.salonId,
    required this.serviceId,
    required this.staffId,
    this.excludeAppointmentId,
    required this.onChanged,
  });

  @override
  State<AvailabilityPicker> createState() => _AvailabilityPickerState();
}

class _AvailabilityPickerState extends State<AvailabilityPicker> {
  static const _bookingHorizonDays = 60;
  static const _weekdayLabels = ['M', 'T', 'W', 'T', 'F', 'S', 'S'];

  late final DateTime _today;
  late DateTime _visibleMonth;
  final Map<String, AvailabilityDayModel> _days = {};
  DateTime? _selectedDate;
  DateTime? _selectedStart;
  List<AvailabilitySlotModel> _slots = [];
  bool _loadingDays = true;
  bool _loadingSlots = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    final now = DateTime.now();
    _today = DateTime(now.year, now.month, now.day);
    _visibleMonth = DateTime(_today.year, _today.month);
    _loadMonth(autoSelect: true);
  }

  @override
  void didUpdateWidget(covariant AvailabilityPicker oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.serviceId != widget.serviceId ||
        oldWidget.staffId != widget.staffId ||
        oldWidget.salonId != widget.salonId ||
        oldWidget.excludeAppointmentId != widget.excludeAppointmentId) {
      _days.clear();
      _slots = [];
      _selectedDate = null;
      _selectedStart = null;
      widget.onChanged(null);
      _loadMonth(autoSelect: true);
    }
  }

  String _key(DateTime d) => DateFormat('yyyy-MM-dd').format(d);

  int _daysBetween(DateTime a, DateTime b) => DateTime.utc(b.year, b.month, b.day).difference(DateTime.utc(a.year, a.month, a.day)).inDays;

  bool get _canGoBack => _visibleMonth.isAfter(DateTime(_today.year, _today.month));

  bool get _canGoForward {
    final horizonEnd = _today.add(const Duration(days: _bookingHorizonDays));
    return DateTime(_visibleMonth.year, _visibleMonth.month + 1).isBefore(horizonEnd.add(const Duration(days: 1)));
  }

  Future<void> _loadMonth({bool autoSelect = false}) async {
    final monthStart = _visibleMonth;
    final monthEnd = DateTime(monthStart.year, monthStart.month + 1, 0);
    final horizonEnd = _today.add(const Duration(days: _bookingHorizonDays));
    final from = monthStart.isBefore(_today) ? _today : monthStart;
    final to = monthEnd.isAfter(horizonEnd) ? horizonEnd : monthEnd;

    if (to.isBefore(from)) {
      setState(() => _loadingDays = false);
      return;
    }

    setState(() {
      _loadingDays = true;
      _error = null;
    });
    try {
      final days = await context.read<AvailabilityProvider>().getDays(
            salonId: widget.salonId,
            serviceId: widget.serviceId,
            staffId: widget.staffId,
            from: from,
            days: _daysBetween(from, to) + 1,
          );
      if (!mounted) return;
      setState(() {
        for (final d in days) {
          _days[_key(d.date)] = d;
        }
        _loadingDays = false;
      });
      if (autoSelect) {
        final firstBookable = days.where((d) => d.isBookable).firstOrNull;
        if (firstBookable != null) _selectDate(firstBookable.date);
      }
    } on ApiClientException catch (e) {
      if (!mounted) return;
      setState(() {
        _loadingDays = false;
        _error = e.message;
      });
    }
  }

  Future<void> _changeMonth(int delta) async {
    setState(() => _visibleMonth = DateTime(_visibleMonth.year, _visibleMonth.month + delta));
    await _loadMonth();
  }

  Future<void> _selectDate(DateTime date) async {
    setState(() {
      _selectedDate = date;
      _selectedStart = null;
      _slots = [];
      _loadingSlots = true;
      _error = null;
    });
    widget.onChanged(null);
    try {
      final slots = await context.read<AvailabilityProvider>().getSlots(
            salonId: widget.salonId,
            serviceId: widget.serviceId,
            staffId: widget.staffId,
            date: date,
            excludeAppointmentId: widget.excludeAppointmentId,
          );
      if (!mounted || _selectedDate != date) return;
      setState(() {
        _slots = slots;
        _loadingSlots = false;
      });
    } on ApiClientException catch (e) {
      if (!mounted) return;
      setState(() {
        _loadingSlots = false;
        _error = e.message;
      });
    }
  }

  void _selectSlot(AvailabilitySlotModel slot) {
    setState(() => _selectedStart = slot.startsAt);
    widget.onChanged(slot.startsAt);
  }

  (Color, Color) _colorsFor(String? status) {
    switch (status) {
      case AvailabilityDayModel.free:
        return (AppColors.completedText, AppColors.completedBg);
      case AvailabilityDayModel.limited:
        return (AppColors.pendingText, AppColors.pendingBg);
      case AvailabilityDayModel.full:
        return (AppColors.declinedText, AppColors.declinedBg);
      case AvailabilityDayModel.closed:
        return (AppColors.neutralText, AppColors.neutralBg);
      default:
        return (const Color(0xFFB8BCC6), Colors.transparent);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        _buildMonthHeader(),
        const SizedBox(height: 10),
        _buildLegend(),
        const SizedBox(height: 12),
        Row(
          children: _weekdayLabels
              .map((l) => Expanded(
                    child: Center(child: Text(l, style: const TextStyle(fontSize: 11, fontWeight: FontWeight.w700, color: AppColors.textSecondary))),
                  ))
              .toList(),
        ),
        const SizedBox(height: 8),
        _loadingDays && _days.isEmpty
            ? const SizedBox(height: 220, child: LoadingIndicator())
            : Opacity(opacity: _loadingDays ? 0.5 : 1, child: _buildCalendarGrid()),
        if (_error != null) ...[
          const SizedBox(height: 10),
          Text(_error!, style: const TextStyle(color: AppColors.declinedText, fontSize: 12)),
        ],
        const SizedBox(height: 20),
        _buildSlotsSection(),
      ],
    );
  }

  Widget _buildMonthHeader() {
    return Row(
      children: [
        Expanded(
          child: Text(
            DateFormat('MMMM yyyy').format(_visibleMonth),
            style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
          ),
        ),
        IconButton(
          visualDensity: VisualDensity.compact,
          icon: const Icon(Icons.chevron_left),
          onPressed: _canGoBack && !_loadingDays ? () => _changeMonth(-1) : null,
        ),
        IconButton(
          visualDensity: VisualDensity.compact,
          icon: const Icon(Icons.chevron_right),
          onPressed: _canGoForward && !_loadingDays ? () => _changeMonth(1) : null,
        ),
      ],
    );
  }

  Widget _buildLegend() {
    Widget item(String label, String status) {
      final (text, bg) = _colorsFor(status);
      return Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            width: 12,
            height: 12,
            decoration: BoxDecoration(color: bg, borderRadius: BorderRadius.circular(4), border: Border.all(color: text.withValues(alpha: 0.5))),
          ),
          const SizedBox(width: 4),
          Text(label, style: const TextStyle(fontSize: 11, color: AppColors.textSecondary)),
        ],
      );
    }

    return Wrap(
      spacing: 14,
      runSpacing: 6,
      children: [
        item('Free', AvailabilityDayModel.free),
        item('Few left', AvailabilityDayModel.limited),
        item('Fully booked', AvailabilityDayModel.full),
        item('Closed', AvailabilityDayModel.closed),
      ],
    );
  }

  Widget _buildCalendarGrid() {
    final daysInMonth = DateTime(_visibleMonth.year, _visibleMonth.month + 1, 0).day;
    final leadingBlanks = _visibleMonth.weekday - 1;

    final cells = <Widget>[
      for (var i = 0; i < leadingBlanks; i++) const SizedBox.shrink(),
      for (var day = 1; day <= daysInMonth; day++) _buildDayCell(DateTime(_visibleMonth.year, _visibleMonth.month, day)),
    ];

    return GridView.count(
      crossAxisCount: 7,
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      mainAxisSpacing: 6,
      crossAxisSpacing: 6,
      children: cells,
    );
  }

  Widget _buildDayCell(DateTime date) {
    final info = _days[_key(date)];
    final (textColor, bgColor) = _colorsFor(info?.status);
    final selected = _selectedDate != null && _key(_selectedDate!) == _key(date);

    return GestureDetector(
      onTap: info != null && info.isBookable ? () => _selectDate(date) : null,
      child: Container(
        decoration: BoxDecoration(
          color: bgColor,
          borderRadius: BorderRadius.circular(12),
          border: selected ? Border.all(color: AppColors.primary, width: 2.5) : null,
        ),
        alignment: Alignment.center,
        child: Text(
          '${date.day}',
          style: TextStyle(
            fontSize: 14,
            fontWeight: selected ? FontWeight.w800 : FontWeight.w600,
            color: textColor,
            decoration: info != null && info.status == AvailabilityDayModel.full ? TextDecoration.lineThrough : null,
          ),
        ),
      ),
    );
  }

  Widget _buildSlotsSection() {
    if (_selectedDate == null) {
      return const Text('Pick a green or amber day to see available times.', style: TextStyle(color: AppColors.textSecondary, fontSize: 13));
    }

    final free = _slots.where((s) => s.isAvailable).length;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          '${DateFormat('EEE, MMM d').format(_selectedDate!).toUpperCase()}${_loadingSlots ? '' : ' · $free FREE'}',
          style: const TextStyle(color: AppColors.textSecondary, fontSize: 11, fontWeight: FontWeight.w700, letterSpacing: 0.4),
        ),
        const SizedBox(height: 10),
        if (_loadingSlots)
          const SizedBox(height: 80, child: LoadingIndicator())
        else if (_slots.isEmpty)
          const Text('No times available on this day.', style: TextStyle(color: AppColors.textSecondary, fontSize: 13))
        else
          GridView.builder(
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: 4, mainAxisSpacing: 8, crossAxisSpacing: 8, childAspectRatio: 2.1),
            itemCount: _slots.length,
            itemBuilder: (context, i) => _buildSlotTile(_slots[i]),
          ),
      ],
    );
  }

  Widget _buildSlotTile(AvailabilitySlotModel slot) {
    final selected = _selectedStart == slot.startsAt;
    final label = DateFormat('HH:mm').format(slot.startsAt);

    return GestureDetector(
      onTap: slot.isAvailable ? () => _selectSlot(slot) : null,
      child: Container(
        decoration: BoxDecoration(
          color: selected ? AppColors.primary : (slot.isAvailable ? Colors.white : AppColors.surfaceMuted),
          borderRadius: BorderRadius.circular(10),
          border: selected || !slot.isAvailable ? null : Border.all(color: const Color(0xFFE5E1EF)),
        ),
        alignment: Alignment.center,
        child: Text(
          label,
          style: TextStyle(
            fontSize: 13,
            fontWeight: FontWeight.w600,
            color: selected ? Colors.white : (slot.isAvailable ? AppColors.textPrimary : const Color(0xFFB8BCC6)),
            decoration: slot.isAvailable ? null : TextDecoration.lineThrough,
          ),
        ),
      ),
    );
  }
}
