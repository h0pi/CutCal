import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import 'salon_detail_screen.dart';

// Images are network-loaded; bounded via cacheWidth to keep decode cost low.
const bool kShowSalonImages = true;

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
  int _page = 0;
  bool _isLoading = false;
  bool _hasMore = true;
  double? _lat;
  double? _lng;

  @override
  void initState() {
    super.initState();
    _loadCategories();
    _determineLocation();
    _loadSalons(reset: true);
  }

  Future<void> _determineLocation() async {
    try {
      var permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
      }
      if (permission == LocationPermission.denied || permission == LocationPermission.deniedForever) return;
      if (!await Geolocator.isLocationServiceEnabled()) return;

      final position = await Geolocator.getCurrentPosition();
      if (!mounted) return;
      setState(() {
        _lat = position.latitude;
        _lng = position.longitude;
      });
      _loadSalons(reset: true);
    } catch (_) {
      // Distance just won't be shown; the rest of the screen still works.
    }
  }

  Future<void> _loadCategories() async {
    final result = await context.read<SalonCategoryProvider>().get();
    if (mounted) setState(() => _categories = result.items);
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
        'lat': _lat,
        'lng': _lng,
      });
      if (!mounted) return;
      setState(() {
        _salons.addAll(result.items);
        _hasMore = _salons.length < result.totalCount;
        _page++;
      });
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  String get _greeting {
    final hour = DateTime.now().hour;
    if (hour < 12) return 'GOOD MORNING';
    if (hour < 18) return 'GOOD AFTERNOON';
    return 'GOOD EVENING';
  }

  String get _initials {
    final first = AuthProvider.accessTokenDecoded?['FirstName']?.toString() ?? '';
    final last = AuthProvider.accessTokenDecoded?['LastName']?.toString() ?? '';
    final initials = '${first.isNotEmpty ? first[0] : ''}${last.isNotEmpty ? last[0] : ''}';
    return initials.isEmpty ? '?' : initials.toUpperCase();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: CustomScrollView(
        slivers: [
          SliverToBoxAdapter(child: _buildHeader()),
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(16, 16, 16, 8),
            sliver: SliverToBoxAdapter(
              child: SizedBox(
                height: 40,
                child: ListView(
                  scrollDirection: Axis.horizontal,
                  children: [
                    _categoryChip(null, 'All'),
                    ..._categories.map((c) => _categoryChip(c.id, c.name)),
                  ],
                ),
              ),
            ),
          ),
          SliverPadding(
            padding: const EdgeInsets.fromLTRB(16, 8, 16, 4),
            sliver: SliverToBoxAdapter(
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    _selectedCategoryId == null ? 'Top rated nearby' : 'Results',
                    style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                  ),
                  Text('${_salons.length} nearby', style: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.w600, fontSize: 13)),
                ],
              ),
            ),
          ),
          if (_salons.isEmpty && !_isLoading)
            const SliverFillRemaining(
              hasScrollBody: false,
              child: Center(child: Text('No salons found.', style: TextStyle(color: AppColors.textSecondary))),
            )
          else
            SliverPadding(
              padding: const EdgeInsets.fromLTRB(16, 4, 16, 16),
              sliver: SliverList.builder(
                itemCount: _salons.length + (_hasMore ? 1 : 0),
                itemBuilder: (context, index) {
                  if (index == _salons.length) {
                    _loadSalons();
                    return const Padding(padding: EdgeInsets.all(16), child: LoadingIndicator());
                  }
                  final salon = _salons[index];
                  return _SalonCard(salon: salon);
                },
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildHeader() {
    return Container(
      padding: const EdgeInsets.fromLTRB(20, 56, 20, 24),
      decoration: const BoxDecoration(
        color: AppColors.primary,
        borderRadius: BorderRadius.only(bottomLeft: Radius.circular(28), bottomRight: Radius.circular(28)),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(_greeting, style: TextStyle(color: Colors.white.withValues(alpha: 0.75), fontSize: 12, fontWeight: FontWeight.w600, letterSpacing: 0.5)),
                    const SizedBox(height: 4),
                    const Text('Find your next look', style: TextStyle(color: Colors.white, fontSize: 24, fontWeight: FontWeight.bold)),
                  ],
                ),
              ),
              CircleAvatar(
                radius: 20,
                backgroundColor: Colors.white.withValues(alpha: 0.2),
                child: Text(_initials, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
              ),
            ],
          ),
          const SizedBox(height: 20),
          Row(
            children: [
              Expanded(
                child: SizedBox(
                  height: 48,
                  child: TextField(
                    controller: _searchController,
                    decoration: const InputDecoration(
                      hintText: 'Salon, address, or service',
                      prefixIcon: Icon(Icons.search),
                      isCollapsed: false,
                    ),
                    onSubmitted: (_) => _loadSalons(reset: true),
                  ),
                ),
              ),
              const SizedBox(width: 10),
              Material(
                color: Colors.white.withValues(alpha: 0.2),
                borderRadius: BorderRadius.circular(24),
                child: InkWell(
                  borderRadius: BorderRadius.circular(24),
                  onTap: () {},
                  child: const SizedBox(width: 48, height: 48, child: Icon(Icons.tune, color: Colors.white)),
                ),
              ),
            ],
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

  const _SalonCard({required this.salon});

  bool? get _isOpenNow {
    if (salon.workingHours.isEmpty) return null;
    final backendDay = DateTime.now().weekday - 1;
    final today = salon.workingHours.where((wh) => wh.dayOfWeek == backendDay).firstOrNull;
    if (today == null || today.isClosed || today.openTime == null || today.closeTime == null) return false;

    final now = TimeOfDay.now();
    final open = _parseTime(today.openTime!);
    final close = _parseTime(today.closeTime!);
    if (open == null || close == null) return null;

    final nowMinutes = now.hour * 60 + now.minute;
    return nowMinutes >= (open.hour * 60 + open.minute) && nowMinutes <= (close.hour * 60 + close.minute);
  }

  String? get _closeTimeLabel {
    final backendDay = DateTime.now().weekday - 1;
    final today = salon.workingHours.where((wh) => wh.dayOfWeek == backendDay).firstOrNull;
    if (today == null || today.closeTime == null) return null;
    final parts = today.closeTime!.split(':');
    if (parts.length < 2) return null;
    final hour = int.tryParse(parts[0]) ?? 0;
    final suffix = hour >= 12 ? 'pm' : 'am';
    final hour12 = hour % 12 == 0 ? 12 : hour % 12;
    return '$hour12$suffix';
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
    final closeLabel = _closeTimeLabel;

    final subtitleParts = <String>[];
    if (salon.distanceKm != null) subtitleParts.add('${salon.distanceKm!.toStringAsFixed(1)} mi');
    if (salon.cityName != null) subtitleParts.add(salon.cityName!);
    if (isOpen == true && closeLabel != null) {
      subtitleParts.add('Open till $closeLabel');
    } else if (isOpen == false) {
      subtitleParts.add('Closed');
    }

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      clipBehavior: Clip.antiAlias,
      child: InkWell(
        onTap: () => Navigator.of(context).push(
          MaterialPageRoute(builder: (_) => SalonDetailScreen(salonId: salon.id, initialDistanceKm: salon.distanceKm)),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Stack(
              children: [
                SizedBox(
                  height: 140,
                  width: double.infinity,
                  child: (kShowSalonImages && salon.profileImageUrl != null)
                      ? Image.network(
                          salon.profileImageUrl!,
                          fit: BoxFit.cover,
                          cacheWidth: 400,
                          loadingBuilder: (context, child, progress) {
                            if (progress == null) return child;
                            return Container(
                              color: AppColors.primaryLight,
                              alignment: Alignment.center,
                              child: const CircularProgressIndicator(color: AppColors.primary, strokeWidth: 2),
                            );
                          },
                          errorBuilder: (context, error, stackTrace) => Container(
                            color: AppColors.primaryLight,
                            alignment: Alignment.center,
                            child: const Icon(Icons.storefront, size: 40, color: AppColors.primary),
                          ),
                        )
                      : Container(color: AppColors.primaryLight, child: const Icon(Icons.storefront, size: 40, color: AppColors.primary)),
                ),
                Positioned(top: 10, right: 10, child: RatingBadge(rating: salon.avgRating)),
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
                          style: const TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary),
                          overflow: TextOverflow.ellipsis,
                        ),
                      ),
                      if (salon.minServicePrice != null)
                        Text(
                          'from \$${salon.minServicePrice!.toStringAsFixed(0)}',
                          style: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.bold, fontSize: 14),
                        ),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Text(
                    subtitleParts.join(' · '),
                    style: const TextStyle(color: AppColors.textSecondary, fontSize: 13),
                    overflow: TextOverflow.ellipsis,
                  ),
                  if ((salon.salonCategoryName ?? '').isNotEmpty) ...[
                    const SizedBox(height: 10),
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                      decoration: BoxDecoration(color: AppColors.primaryLight, borderRadius: BorderRadius.circular(20)),
                      child: Text(
                        salon.salonCategoryName!,
                        style: const TextStyle(color: AppColors.primaryDark, fontSize: 12, fontWeight: FontWeight.w600),
                      ),
                    ),
                  ],
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
