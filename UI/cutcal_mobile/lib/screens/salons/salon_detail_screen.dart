import 'package:carousel_slider/carousel_slider.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';
import '../appointments/booking_screen.dart';

const _dayNames = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

class SalonDetailScreen extends StatefulWidget {
  final int salonId;

  const SalonDetailScreen({super.key, required this.salonId});

  @override
  State<SalonDetailScreen> createState() => _SalonDetailScreenState();
}

class _SalonDetailScreenState extends State<SalonDetailScreen> {
  SalonModel? _salon;
  List<SalonGalleryModel> _gallery = [];
  List<ReviewModel> _reviews = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final salonProvider = context.read<SalonProvider>();
    final reviewProvider = context.read<ReviewProvider>();

    final salon = await salonProvider.getById(widget.salonId);
    final gallery = await salonProvider.getGallery(widget.salonId);
    final reviews = await reviewProvider.get(filter: {'salonId': widget.salonId, 'pageSize': 20});

    setState(() {
      _salon = salon;
      _gallery = gallery;
      _reviews = reviews.items;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    if (_isLoading || _salon == null) {
      return const Scaffold(body: LoadingIndicator());
    }

    final salon = _salon!;
    return Scaffold(
      appBar: AppBar(title: Text(salon.name)),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          if (_gallery.isNotEmpty)
            CarouselSlider(
              options: CarouselOptions(height: 180, autoPlay: true, enlargeCenterPage: true),
              items: _gallery
                  .map((g) => ClipRRect(
                        borderRadius: BorderRadius.circular(12),
                        child: Image.network(g.imageUrl, fit: BoxFit.cover, width: double.infinity),
                      ))
                  .toList(),
            )
          else if (salon.profileImageUrl != null)
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: Image.network(salon.profileImageUrl!, height: 180, width: double.infinity, fit: BoxFit.cover),
            ),
          const SizedBox(height: 16),
          Row(
            children: [
              const Icon(Icons.star, color: Colors.amber),
              Text(' ${salon.avgRating.toStringAsFixed(1)}'),
              const SizedBox(width: 12),
              Expanded(child: Text(salon.salonCategoryName ?? '')),
            ],
          ),
          const SizedBox(height: 8),
          Text(salon.description ?? '', style: Theme.of(context).textTheme.bodyMedium),
          const SizedBox(height: 8),
          Row(children: [const Icon(Icons.location_on, size: 18), const SizedBox(width: 4), Expanded(child: Text(salon.address))]),
          if (salon.phone != null) Row(children: [const Icon(Icons.phone, size: 18), const SizedBox(width: 4), Text(salon.phone!)]),
          const SizedBox(height: 16),
          Text('Working hours', style: Theme.of(context).textTheme.titleMedium),
          ...salon.workingHours.map((wh) => Padding(
                padding: const EdgeInsets.symmetric(vertical: 2),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(_dayNames[wh.dayOfWeek.clamp(0, 6)]),
                    Text(wh.isClosed ? 'Closed' : '${wh.openTime} - ${wh.closeTime}'),
                  ],
                ),
              )),
          const SizedBox(height: 24),
          FilledButton.icon(
            icon: const Icon(Icons.calendar_month),
            label: const Text('Book Appointment'),
            onPressed: () => Navigator.of(context).push(
              MaterialPageRoute(builder: (_) => BookingScreen(salon: salon)),
            ),
          ),
          const SizedBox(height: 24),
          Text('Reviews (${_reviews.length})', style: Theme.of(context).textTheme.titleMedium),
          ..._reviews.map((r) => Card(
                child: ListTile(
                  title: Row(
                    children: List.generate(
                      5,
                      (i) => Icon(i < r.rating ? Icons.star : Icons.star_border, size: 16, color: Colors.amber),
                    ),
                  ),
                  subtitle: Text(r.comment ?? ''),
                ),
              )),
        ],
      ),
    );
  }
}
