import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import '../appointments/booking_screen.dart';
import '../../utils/image_url.dart';

// Indexed by the backend day-of-week value (0 = Sunday).
const _dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

// Plain Containers (not Card) are used for the info/hours/review sections, so they
// need their own subtle separation from the near-white scaffold background.
const _cardDecoration = BoxDecoration(
  color: Colors.white,
  borderRadius: BorderRadius.all(Radius.circular(16)),
  border: Border.fromBorderSide(BorderSide(color: Color(0xFFEDEAF5))),
  boxShadow: [BoxShadow(color: Color(0x14000000), blurRadius: 10, offset: Offset(0, 3))],
);

class SalonDetailScreen extends StatefulWidget {
  final int salonId;
  final double? initialDistanceKm;

  const SalonDetailScreen({super.key, required this.salonId, this.initialDistanceKm});

  @override
  State<SalonDetailScreen> createState() => _SalonDetailScreenState();
}

class _SalonDetailScreenState extends State<SalonDetailScreen> {
  SalonModel? _salon;
  List<SalonGalleryModel> _gallery = [];
  List<ReviewModel> _reviews = [];
  List<StaffModel> _staff = [];
  List<SalonServiceModel> _services = [];
  bool _isLoading = true;
  bool _isFavorite = false;
  int _carouselIndex = 0;
  bool _hoursExpanded = false;

  @override
  void initState() {
    super.initState();
    _load();
    _logView();
  }

  // Feeds the recommender; a failure must never get in the way of browsing.
  Future<void> _logView() async {
    try {
      await context.read<SalonProvider>().logView(widget.salonId);
    } on ApiClientException catch (e) {
      debugPrint('Could not log salon view: ${e.message}');
    }
  }

  Future<void> _load() async {
    final salonProvider = context.read<SalonProvider>();
    final reviewProvider = context.read<ReviewProvider>();
    final staffProvider = context.read<StaffProvider>();
    final serviceProvider = context.read<SalonServiceProvider>();
    final favoriteProvider = context.read<FavoriteProvider>();

    final salon = await salonProvider.getById(widget.salonId);
    final gallery = await salonProvider.getGallery(widget.salonId);
    final reviews = await reviewProvider.get(filter: {'salonId': widget.salonId, 'pageSize': 20});
    final staff = await staffProvider.get(filter: {'salonId': widget.salonId, 'isActive': true, 'pageSize': 50});
    final services = await serviceProvider.get(filter: {'salonId': widget.salonId, 'isActive': true, 'pageSize': 50});
    final favorites = await favoriteProvider.getMine();

    if (!mounted) return;
    setState(() {
      _salon = salon;
      _gallery = gallery;
      _reviews = reviews.items;
      _staff = staff.items;
      _services = services.items;
      _isFavorite = favorites.any((f) => f.salonId == widget.salonId);
      _isLoading = false;
    });
  }

  Future<void> _toggleFavorite() async {
    final provider = context.read<FavoriteProvider>();
    setState(() => _isFavorite = !_isFavorite);
    if (_isFavorite) {
      await provider.add(widget.salonId);
    } else {
      await provider.removeSalon(widget.salonId);
    }
  }

  String _formatTime(String? raw) {
    if (raw == null) return '';
    final parts = raw.split(':');
    if (parts.length < 2) return raw;
    return '${parts[0]}:${parts[1]}';
  }

  bool _isToday(int dayOfWeek) => dayOfWeek == DateTime.now().weekday % 7;

  @override
  Widget build(BuildContext context) {
    if (_isLoading || _salon == null) {
      return const Scaffold(body: LoadingIndicator());
    }

    final salon = _salon!;
    final images = _gallery.map((g) => g.imageUrl).toList();
    if (images.isEmpty && salon.profileImageUrl != null) images.add(salon.profileImageUrl!);
    final todayHours = salon.workingHours.where((wh) => _isToday(wh.dayOfWeek)).firstOrNull;

    return Scaffold(
      body: CustomScrollView(
        slivers: [
          SliverAppBar(
            expandedHeight: 240,
            pinned: true,
            backgroundColor: AppColors.primary,
            iconTheme: const IconThemeData(color: Colors.white),
            flexibleSpace: FlexibleSpaceBar(
              background: _buildHeroImages(images),
            ),
            actions: [
              Padding(
                padding: const EdgeInsets.only(right: 8),
                child: CircleAvatar(
                  backgroundColor: Colors.black26,
                  child: IconButton(
                    icon: Icon(_isFavorite ? Icons.favorite : Icons.favorite_border, color: _isFavorite ? Colors.redAccent : Colors.white),
                    onPressed: _toggleFavorite,
                  ),
                ),
              ),
            ],
          ),
          SliverToBoxAdapter(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 16, 16, 100),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Expanded(
                        child: Text(salon.name, style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                      ),
                      if (salon.avgRating >= 4.5)
                        Container(
                          margin: const EdgeInsets.only(left: 8, top: 4),
                          padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                          decoration: BoxDecoration(color: AppColors.primaryLight, borderRadius: BorderRadius.circular(20)),
                          child: const Text('Top rated', style: TextStyle(color: AppColors.primaryDark, fontSize: 11, fontWeight: FontWeight.bold)),
                        ),
                    ],
                  ),
                  const SizedBox(height: 6),
                  Row(
                    children: [
                      const Icon(Icons.star_rounded, color: AppColors.primary, size: 20),
                      const SizedBox(width: 4),
                      Text(salon.avgRating.toStringAsFixed(1), style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                      Text('  (${_reviews.length} reviews)', style: const TextStyle(color: AppColors.textSecondary, fontSize: 13)),
                    ],
                  ),
                  const SizedBox(height: 4),
                  Text(salon.address, style: const TextStyle(color: AppColors.textSecondary)),
                  const SizedBox(height: 12),
                  OutlinedButton.icon(
                    onPressed: () {},
                    icon: const Icon(Icons.directions_outlined, size: 18),
                    label: const Text('Directions'),
                    style: OutlinedButton.styleFrom(minimumSize: const Size(140, 40), padding: const EdgeInsets.symmetric(horizontal: 16)),
                  ),
                  const SizedBox(height: 16),
                  Row(
                    children: [
                      Expanded(
                        child: _statChip(
                          'TODAY',
                          todayHours == null
                              ? '—'
                              : (todayHours.isClosed ? 'Closed' : '${_formatTime(todayHours.openTime)} – ${_formatTime(todayHours.closeTime)}'),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: _statChip(
                          'DISTANCE',
                          widget.initialDistanceKm != null ? '${widget.initialDistanceKm!.toStringAsFixed(1)} mi' : '—',
                        ),
                      ),
                    ],
                  ),
                  if (_staff.isNotEmpty) ...[
                    const SizedBox(height: 24),
                    const Text('Stylists', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 17, color: AppColors.textPrimary)),
                    const SizedBox(height: 12),
                    SizedBox(
                      height: 84,
                      child: ListView.separated(
                        scrollDirection: Axis.horizontal,
                        itemCount: _staff.length,
                        separatorBuilder: (_, __) => const SizedBox(width: 16),
                        itemBuilder: (context, i) {
                          final s = _staff[i];
                          return SizedBox(
                            width: 68,
                            child: Column(
                              children: [
                                CircleAvatar(
                                  radius: 26,
                                  backgroundColor: AppColors.primaryLight,
                                  backgroundImage: s.profileImageUrl != null ? NetworkImage(resolveImageUrl(s.profileImageUrl!)) : null,
                                  child: s.profileImageUrl == null ? const Icon(Icons.person, color: AppColors.primary) : null,
                                ),
                                const SizedBox(height: 6),
                                Text(
                                  s.fullName ?? 'Staff',
                                  style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w600, color: AppColors.textPrimary),
                                  overflow: TextOverflow.ellipsis,
                                  textAlign: TextAlign.center,
                                ),
                                Text(
                                  s.role,
                                  style: const TextStyle(fontSize: 10, color: AppColors.textSecondary),
                                  overflow: TextOverflow.ellipsis,
                                  textAlign: TextAlign.center,
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                  const SizedBox(height: 24),
                  const Text('Services', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 17, color: AppColors.textPrimary)),
                  const SizedBox(height: 12),
                  if (_services.isEmpty)
                    const Padding(
                      padding: EdgeInsets.symmetric(vertical: 8),
                      child: Text('No services listed yet.', style: TextStyle(color: AppColors.textSecondary)),
                    )
                  else
                    ..._services.map((s) => Container(
                          margin: const EdgeInsets.only(bottom: 10),
                          padding: const EdgeInsets.all(14),
                          decoration: _cardDecoration,
                          child: Row(
                            children: [
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(s.name, style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
                                    const SizedBox(height: 2),
                                    Text(
                                      '${s.durationMinutes} min',
                                      style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
                                    ),
                                  ],
                                ),
                              ),
                              Text(
                                '\$${s.price.toStringAsFixed(0)}',
                                style: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.bold, fontSize: 15),
                              ),
                            ],
                          ),
                        )),
                  const SizedBox(height: 12),
                  _buildWorkingHoursCard(salon),
                  const SizedBox(height: 24),
                  Row(
                    children: [
                      Text('Reviews', style: Theme.of(context).textTheme.titleMedium),
                      const SizedBox(width: 8),
                      Text('(${_reviews.length})', style: const TextStyle(color: AppColors.textSecondary)),
                    ],
                  ),
                  const SizedBox(height: 8),
                  if (_reviews.isEmpty)
                    const Padding(
                      padding: EdgeInsets.symmetric(vertical: 12),
                      child: Text('No reviews yet.', style: TextStyle(color: AppColors.textSecondary)),
                    )
                  else
                    ..._reviews.map(_buildReviewCard),
                ],
              ),
            ),
          ),
        ],
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.fromLTRB(16, 8, 16, 16),
          child: SizedBox(
            width: double.infinity,
            height: 52,
            child: FilledButton(
              onPressed: () => Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => BookingScreen(salon: salon)),
              ),
              child: const Text('Book an appointment'),
            ),
          ),
        ),
      ),
    );
  }

  Widget _statChip(String label, String value) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      decoration: BoxDecoration(color: AppColors.surfaceMuted, borderRadius: BorderRadius.circular(14)),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: const TextStyle(color: AppColors.textSecondary, fontSize: 10, fontWeight: FontWeight.w700, letterSpacing: 0.4)),
          const SizedBox(height: 4),
          Text(value, style: const TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.bold, fontSize: 13)),
        ],
      ),
    );
  }

  Widget _buildHeroImages(List<String> images) {
    if (images.isEmpty) {
      return Container(
        color: AppColors.primary,
        alignment: Alignment.center,
        child: const Icon(Icons.storefront, size: 56, color: Colors.white70),
      );
    }

    return Stack(
      fit: StackFit.expand,
      children: [
        CarouselSlider(
          options: CarouselOptions(
            height: double.infinity,
            viewportFraction: 1,
            autoPlay: images.length > 1,
            onPageChanged: (index, _) => setState(() => _carouselIndex = index),
          ),
          items: images
              .map((url) => Image.network(
                    resolveImageUrl(url),
                    fit: BoxFit.cover,
                    width: double.infinity,
                    cacheWidth: 800,
                    errorBuilder: (context, error, stackTrace) => Container(
                      color: AppColors.primary,
                      child: const Icon(Icons.storefront, size: 56, color: Colors.white70),
                    ),
                  ))
              .toList(),
        ),
        // Gradient so the app bar's back/favorite buttons stay legible over bright photos.
        const Positioned(
          top: 0,
          left: 0,
          right: 0,
          height: 80,
          child: DecoratedBox(
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
                colors: [Colors.black45, Colors.transparent],
              ),
            ),
          ),
        ),
        if (images.length > 1)
          Positioned(
            bottom: 12,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: List.generate(images.length, (i) {
                final active = i == _carouselIndex;
                return AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  margin: const EdgeInsets.symmetric(horizontal: 3),
                  width: active ? 18 : 6,
                  height: 6,
                  decoration: BoxDecoration(
                    color: active ? AppColors.primaryLight : Colors.white54,
                    borderRadius: BorderRadius.circular(4),
                  ),
                );
              }),
            ),
          ),
      ],
    );
  }

  Widget _buildWorkingHoursCard(SalonModel salon) {
    // Show Monday first, Sunday last.
    final sortedHours = salon.workingHours.toList()..sort((a, b) => ((a.dayOfWeek + 6) % 7).compareTo((b.dayOfWeek + 6) % 7));
    final todayHours = sortedHours.where((wh) => _isToday(wh.dayOfWeek)).firstOrNull;

    return Container(
      width: double.infinity,
      decoration: _cardDecoration,
      clipBehavior: Clip.antiAlias,
      child: Column(
        children: [
          InkWell(
            onTap: sortedHours.isEmpty ? null : () => setState(() => _hoursExpanded = !_hoursExpanded),
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  const Icon(Icons.schedule, size: 18, color: AppColors.primary),
                  const SizedBox(width: 10),
                  const Expanded(child: Text('Working hours', style: TextStyle(fontWeight: FontWeight.bold, fontSize: 15, color: AppColors.textPrimary))),
                  if (todayHours != null)
                    Text(
                      todayHours.isClosed ? 'Closed today' : 'Today ${_formatTime(todayHours.openTime)}-${_formatTime(todayHours.closeTime)}',
                      style: const TextStyle(
                        color: AppColors.primaryDark,
                        fontWeight: FontWeight.w600,
                        fontSize: 12,
                      ),
                    ),
                  if (sortedHours.isNotEmpty) ...[
                    const SizedBox(width: 4),
                    AnimatedRotation(
                      turns: _hoursExpanded ? 0.5 : 0,
                      duration: const Duration(milliseconds: 200),
                      child: const Icon(Icons.keyboard_arrow_down, color: AppColors.textSecondary),
                    ),
                  ],
                ],
              ),
            ),
          ),
          AnimatedCrossFade(
            firstChild: const SizedBox(width: double.infinity, height: 0),
            secondChild: Padding(
              padding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
              child: Column(
                children: sortedHours.isEmpty
                    ? const [Text('Not specified', style: TextStyle(color: AppColors.textSecondary))]
                    : sortedHours.map((wh) {
                        final today = _isToday(wh.dayOfWeek);
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 5),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Text(
                                _dayNames[wh.dayOfWeek.clamp(0, 6)],
                                style: TextStyle(
                                  color: today ? AppColors.textPrimary : AppColors.textSecondary,
                                  fontWeight: today ? FontWeight.bold : FontWeight.normal,
                                ),
                              ),
                              Text(
                                wh.isClosed ? 'Closed' : '${_formatTime(wh.openTime)} - ${_formatTime(wh.closeTime)}',
                                style: TextStyle(
                                  color: wh.isClosed ? AppColors.textSecondary : (today ? AppColors.primaryDark : AppColors.textSecondary),
                                  fontWeight: today ? FontWeight.bold : FontWeight.normal,
                                ),
                              ),
                            ],
                          ),
                        );
                      }).toList(),
              ),
            ),
            crossFadeState: _hoursExpanded ? CrossFadeState.showSecond : CrossFadeState.showFirst,
            duration: const Duration(milliseconds: 200),
            sizeCurve: Curves.easeInOut,
          ),
        ],
      ),
    );
  }

  Widget _buildReviewCard(ReviewModel r) {
    final initial = (r.customerName ?? '?').trim().isNotEmpty ? r.customerName!.trim()[0].toUpperCase() : '?';
    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: _cardDecoration,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 16,
                backgroundColor: AppColors.primaryLight,
                child: Text(initial, style: const TextStyle(color: AppColors.primaryDark, fontWeight: FontWeight.bold)),
              ),
              const SizedBox(width: 10),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(r.customerName ?? 'Customer', style: const TextStyle(fontWeight: FontWeight.w600, color: AppColors.textPrimary)),
                    Row(
                      children: List.generate(
                        5,
                        (i) => Icon(i < r.rating ? Icons.star_rounded : Icons.star_border_rounded, size: 14, color: AppColors.starColor),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
          if ((r.comment ?? '').isNotEmpty) ...[
            const SizedBox(height: 10),
            Text(r.comment!, style: const TextStyle(color: AppColors.textSecondary, height: 1.3)),
          ],
          if ((r.salonReply ?? '').isNotEmpty) ...[
            const SizedBox(height: 10),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: AppColors.primaryLight,
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('Salon response', style: TextStyle(color: AppColors.primaryDark, fontSize: 12, fontWeight: FontWeight.bold)),
                  const SizedBox(height: 4),
                  Text(r.salonReply!, style: const TextStyle(color: AppColors.textPrimary)),
                ],
              ),
            ),
          ],
        ],
      ),
    );
  }
}
