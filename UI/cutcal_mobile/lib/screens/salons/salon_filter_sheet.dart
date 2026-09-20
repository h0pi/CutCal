import 'package:flutter/material.dart';

import '../../utils/app_theme.dart';

class SalonSortBy {
  static const nearest = 'Nearest';
  static const topRated = 'TopRated';
  static const priceLow = 'PriceLow';
}

/// Discover-screen filters. A null [sortBy] means "server default" (nearest when the
/// user's location is known, otherwise top rated).
class SalonFilters {
  final String? sortBy;
  final bool openNow;
  final double? minRating;
  final double? radiusKm;

  const SalonFilters({this.sortBy, this.openNow = false, this.minRating, this.radiusKm});

  bool get isActive => sortBy != null || openNow || minRating != null || radiusKm != null;

  String effectiveSort({required bool hasLocation}) => sortBy ?? (hasLocation ? SalonSortBy.nearest : SalonSortBy.topRated);

  Map<String, dynamic> toQuery() => {
        'sortBy': sortBy,
        'openNow': openNow ? true : null,
        'nowLocal': openNow ? DateTime.now().toIso8601String() : null,
        'minRating': minRating,
        'radiusKm': radiusKm,
      };
}

Future<SalonFilters?> showSalonFilterSheet(BuildContext context, SalonFilters current, {required bool hasLocation}) {
  return showModalBottomSheet<SalonFilters>(
    context: context,
    isScrollControlled: true,
    backgroundColor: Colors.white,
    shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
    builder: (context) => _FilterSheet(initial: current, hasLocation: hasLocation),
  );
}

class _FilterSheet extends StatefulWidget {
  final SalonFilters initial;
  final bool hasLocation;

  const _FilterSheet({required this.initial, required this.hasLocation});

  @override
  State<_FilterSheet> createState() => _FilterSheetState();
}

class _FilterSheetState extends State<_FilterSheet> {
  static const _ratingOptions = <(double?, String)>[(null, 'Any'), (4.0, '4.0+'), (4.5, '4.5+')];
  static const _radiusOptions = <(double?, String)>[(null, 'Any'), (5, '5 km'), (20, '20 km'), (50, '50 km')];

  late String _sort = widget.initial.effectiveSort(hasLocation: widget.hasLocation);
  late bool _openNow = widget.initial.openNow;
  late double? _minRating = widget.initial.minRating;
  late double? _radiusKm = widget.initial.radiusKm;

  String get _defaultSort => widget.hasLocation ? SalonSortBy.nearest : SalonSortBy.topRated;

  void _apply() {
    Navigator.of(context).pop(
      SalonFilters(
        sortBy: _sort == _defaultSort ? null : _sort,
        openNow: _openNow,
        minRating: _minRating,
        radiusKm: widget.hasLocation ? _radiusKm : null,
      ),
    );
  }

  void _reset() => Navigator.of(context).pop(const SalonFilters());

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: SingleChildScrollView(
        padding: const EdgeInsets.fromLTRB(20, 12, 20, 20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Center(
              child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey.shade300, borderRadius: BorderRadius.circular(2))),
            ),
            const SizedBox(height: 16),
            const Text('Filters', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
            const SizedBox(height: 18),
            _sectionTitle('Sort by'),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                _chip('Nearest', _sort == SalonSortBy.nearest, widget.hasLocation ? () => setState(() => _sort = SalonSortBy.nearest) : null),
                _chip('Top rated', _sort == SalonSortBy.topRated, () => setState(() => _sort = SalonSortBy.topRated)),
                _chip('Price: low to high', _sort == SalonSortBy.priceLow, () => setState(() => _sort = SalonSortBy.priceLow)),
              ],
            ),
            if (!widget.hasLocation)
              const Padding(
                padding: EdgeInsets.only(top: 6),
                child: Text('Turn on location to sort by distance.', style: TextStyle(color: AppColors.textSecondary, fontSize: 12)),
              ),
            const SizedBox(height: 18),
            _sectionTitle('Minimum rating'),
            Wrap(
              spacing: 8,
              children: _ratingOptions.map((o) => _chip(o.$2, _minRating == o.$1, () => setState(() => _minRating = o.$1))).toList(),
            ),
            const SizedBox(height: 18),
            _sectionTitle('Distance'),
            Wrap(
              spacing: 8,
              children: _radiusOptions
                  .map((o) => _chip(o.$2, _radiusKm == o.$1, widget.hasLocation ? () => setState(() => _radiusKm = o.$1) : null))
                  .toList(),
            ),
            const SizedBox(height: 12),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              value: _openNow,
              title: const Text('Open now', style: TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.w600)),
              onChanged: (v) => setState(() => _openNow = v),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(child: OutlinedButton(onPressed: _reset, child: const Text('Reset'))),
                const SizedBox(width: 12),
                Expanded(child: FilledButton(onPressed: _apply, child: const Text('Apply'))),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _sectionTitle(String text) => Padding(
        padding: const EdgeInsets.only(bottom: 8),
        child: Text(text.toUpperCase(), style: const TextStyle(color: AppColors.textSecondary, fontSize: 11, fontWeight: FontWeight.w700, letterSpacing: 0.4)),
      );

  Widget _chip(String label, bool selected, VoidCallback? onTap) {
    return ChoiceChip(label: Text(label), selected: selected, onSelected: onTap == null ? null : (_) => onTap());
  }
}
