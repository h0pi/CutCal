import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import 'salon_detail_screen.dart';

class SalonListScreen extends StatefulWidget {
  const SalonListScreen({super.key});

  @override
  State<SalonListScreen> createState() => _SalonListScreenState();
}

class _SalonListScreenState extends State<SalonListScreen> {
  final _searchController = TextEditingController();
  List<SalonCategoryModel> _categories = [];
  int? _selectedCategoryId;
  List<SalonModel> _salons = [];
  Set<int> _favoriteSalonIds = {};
  int _page = 0;
  bool _isLoading = false;
  bool _hasMore = true;

  @override
  void initState() {
    super.initState();
    _loadCategories();
    _loadFavorites();
    _loadSalons(reset: true);
  }

  Future<void> _loadCategories() async {
    final result = await context.read<SalonCategoryProvider>().get();
    if (mounted) setState(() => _categories = result.items);
  }

  Future<void> _loadFavorites() async {
    final favorites = await context.read<FavoriteProvider>().getMine();
    if (mounted) setState(() => _favoriteSalonIds = favorites.map((f) => f.salonId).toSet());
  }

  Future<void> _toggleFavorite(SalonModel salon) async {
    final isFavorite = _favoriteSalonIds.contains(salon.id);
    setState(() {
      if (isFavorite) {
        _favoriteSalonIds.remove(salon.id);
      } else {
        _favoriteSalonIds.add(salon.id);
      }
    });
    final provider = context.read<FavoriteProvider>();
    if (isFavorite) {
      await provider.removeSalon(salon.id);
    } else {
      await provider.add(salon.id);
    }
  }

  Future<void> _loadSalons({bool reset = false}) async {
    if (_isLoading) return;
    setState(() => _isLoading = true);

    if (reset) {
      _page = 0;
      _salons = [];
      _hasMore = true;
    }

    try {
      final result = await context.read<SalonProvider>().get(filter: {
        'page': _page,
        'pageSize': 10,
        'name': _searchController.text.isEmpty ? null : _searchController.text,
        'categoryId': _selectedCategoryId,
      });
      setState(() {
        _salons.addAll(result.items);
        _hasMore = _salons.length < result.totalCount;
        _page++;
      });
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Salons', style: TextStyle(fontWeight: FontWeight.bold)),
        actions: [
          IconButton(icon: const Icon(Icons.tune), onPressed: () {}),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 12),
            child: TextField(
              controller: _searchController,
              decoration: const InputDecoration(
                hintText: 'Search salons...',
                prefixIcon: Icon(Icons.search),
              ),
              onSubmitted: (_) => _loadSalons(reset: true),
            ),
          ),
          const SizedBox(height: 12),
          SizedBox(
            height: 40,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 8),
              children: [
                _categoryChip(null, 'All'),
                ..._categories.map((c) => _categoryChip(c.id, c.name)),
              ],
            ),
          ),
          const SizedBox(height: 8),
          Expanded(
            child: _salons.isEmpty && !_isLoading
                ? const Center(child: Text('No salons found.', style: TextStyle(color: Colors.white70)))
                : ListView.builder(
                    padding: const EdgeInsets.fromLTRB(12, 4, 12, 12),
                    itemCount: _salons.length + (_hasMore ? 1 : 0),
                    itemBuilder: (context, index) {
                      if (index == _salons.length) {
                        _loadSalons();
                        return const Padding(padding: EdgeInsets.all(16), child: LoadingIndicator());
                      }
                      final salon = _salons[index];
                      return _SalonCard(
                        salon: salon,
                        isFavorite: _favoriteSalonIds.contains(salon.id),
                        onToggleFavorite: () => _toggleFavorite(salon),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }

  Widget _categoryChip(int? id, String label) {
    final selected = _selectedCategoryId == id;
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 4),
      child: ChoiceChip(
        label: Text(label),
        selected: selected,
        onSelected: (_) {
          setState(() => _selectedCategoryId = id);
          _loadSalons(reset: true);
        },
      ),
    );
  }
}

class _SalonCard extends StatelessWidget {
  final SalonModel salon;
  final bool isFavorite;
  final VoidCallback onToggleFavorite;

  const _SalonCard({required this.salon, required this.isFavorite, required this.onToggleFavorite});

  bool? get _isOpenNow {
    if (salon.workingHours.isEmpty) return null;
    final backendDay = DateTime.now().weekday - 1; // Dart Mon=1..Sun=7 -> backend Mon=0..Sun=6
    final today = salon.workingHours.where((wh) => wh.dayOfWeek == backendDay).firstOrNull;
    if (today == null || today.isClosed || today.openTime == null || today.closeTime == null) return false;

    final now = TimeOfDay.now();
    final open = _parseTime(today.openTime!);
    final close = _parseTime(today.closeTime!);
    if (open == null || close == null) return null;

    final nowMinutes = now.hour * 60 + now.minute;
    return nowMinutes >= (open.hour * 60 + open.minute) && nowMinutes <= (close.hour * 60 + close.minute);
  }

  TimeOfDay? _parseTime(String value) {
    final parts = value.split(':');
    if (parts.length < 2) return null;
    final hour = int.tryParse(parts[0]);
    final minute = int.tryParse(parts[1]);
    if (hour == null || minute == null) return null;
    return TimeOfDay(hour: hour, minute: minute);
  }

  @override
  Widget build(BuildContext context) {
    final isOpen = _isOpenNow;

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: () => Navigator.of(context).push(
          MaterialPageRoute(builder: (_) => SalonDetailScreen(salonId: salon.id)),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                SizedBox(
                  height: 150,
                  width: double.infinity,
                  child: salon.profileImageUrl != null
                      ? Image.network(
                          salon.profileImageUrl!,
                          fit: BoxFit.cover,
                          loadingBuilder: (context, child, progress) {
                            if (progress == null) return child;
                            return Container(
                              color: AppColors.primary,
                              alignment: Alignment.center,
                              child: const CircularProgressIndicator(color: AppColors.accent, strokeWidth: 2),
                            );
                          },
                          errorBuilder: (context, error, stackTrace) => Container(
                            color: AppColors.primary,
                            alignment: Alignment.center,
                            child: const Icon(Icons.storefront, size: 40, color: AppColors.accent),
                          ),
                        )
                      : Container(color: AppColors.primary, child: const Icon(Icons.storefront, size: 40, color: AppColors.accent)),
                ),
                if (salon.avgRating >= 4.5)
                  Positioned(
                    top: 10,
                    left: 10,
                    child: Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                      decoration: BoxDecoration(color: AppColors.accent, borderRadius: BorderRadius.circular(20)),
                      child: const Text('Top rated', style: TextStyle(color: AppColors.primary, fontSize: 11, fontWeight: FontWeight.bold)),
                    ),
                  ),
                Positioned(
                  top: 8,
                  right: 8,
                  child: GestureDetector(
                    onTap: onToggleFavorite,
                    child: CircleAvatar(
                      radius: 16,
                      backgroundColor: AppColors.primary.withValues(alpha: 0.75),
                      child: Icon(
                        isFavorite ? Icons.favorite : Icons.favorite_border,
                        color: isFavorite ? AppColors.accent : Colors.white,
                        size: 18,
                      ),
                    ),
                  ),
                ),
              ],
            ),
            Padding(
              padding: const EdgeInsets.fromLTRB(14, 12, 14, 14),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(
                        child: Text(
                          salon.name,
                          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: Colors.white),
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      const SizedBox(width: 8),
                      Row(
                        children: [
                          const Icon(Icons.star_rounded, color: AppColors.accent, size: 18),
                          const SizedBox(width: 2),
                          Text(salon.avgRating.toStringAsFixed(1), style: const TextStyle(fontWeight: FontWeight.bold, color: Colors.white)),
                        ],
                      ),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Text(
                    salon.salonCategoryName ?? '',
                    style: const TextStyle(color: Colors.white70, fontSize: 13),
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 10),
                  Row(
                    children: [
                      if (salon.distanceKm != null) ...[
                        _infoPill(Icons.place_outlined, '${salon.distanceKm!.toStringAsFixed(1)} km', AppColors.accent),
                        const SizedBox(width: 8),
                      ],
                      if (isOpen != null)
                        _infoPill(
                          isOpen ? Icons.check_circle_outline : Icons.schedule,
                          isOpen ? 'Open now' : 'Closed',
                          isOpen ? Colors.greenAccent.shade200 : Colors.white60,
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _infoPill(IconData icon, String label, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(color: Colors.white.withValues(alpha: 0.08), borderRadius: BorderRadius.circular(20)),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(icon, size: 14, color: color),
          const SizedBox(width: 4),
          Text(label, style: TextStyle(color: color, fontSize: 12, fontWeight: FontWeight.w600)),
        ],
      ),
    );
  }
}
