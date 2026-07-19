import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';
import 'salon_detail_screen.dart';

// Default camera fallback (Sarajevo) used when location permission is denied
// or unavailable, so the map still opens somewhere sensible.
const _fallbackCenter = LatLng(43.8563, 18.4131);

class SalonMapScreen extends StatefulWidget {
  const SalonMapScreen({super.key});

  @override
  State<SalonMapScreen> createState() => _SalonMapScreenState();
}

class _SalonMapScreenState extends State<SalonMapScreen> {
  List<SalonCategoryModel> _categories = [];
  int? _selectedCategoryId;
  List<SalonModel> _salons = [];
  bool _isLoading = true;
  GoogleMapController? _mapController;
  LatLng _initialCenter = _fallbackCenter;

  @override
  void initState() {
    super.initState();
    _determineInitialCenter();
    _load();
  }

  Future<void> _determineInitialCenter() async {
    try {
      var permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
      }
      if (permission == LocationPermission.denied || permission == LocationPermission.deniedForever) {
        return; // keep fallback center
      }
      final serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) return;

      final position = await Geolocator.getCurrentPosition();
      if (!mounted) return;
      setState(() => _initialCenter = LatLng(position.latitude, position.longitude));
      _mapController?.animateCamera(CameraUpdate.newLatLng(_initialCenter));
    } catch (_) {
      // Keep the fallback center; the map is still usable without live location.
    }
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final categories = await context.read<SalonCategoryProvider>().get();
    final salons = await context.read<SalonProvider>().get(filter: {
      'pageSize': 100,
      'categoryId': _selectedCategoryId,
    });
    if (!mounted) return;
    setState(() {
      _categories = categories.items;
      _salons = salons.items;
      _isLoading = false;
    });
  }

  Set<Marker> get _markers => _salons
      .map((salon) => Marker(
            markerId: MarkerId('salon-${salon.id}'),
            position: LatLng(salon.latitude, salon.longitude),
            infoWindow: InfoWindow(
              title: salon.name,
              snippet: '${salon.salonCategoryName ?? ''} • ★ ${salon.avgRating.toStringAsFixed(1)}',
              onTap: () => Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => SalonDetailScreen(salonId: salon.id)),
              ),
            ),
          ))
      .toSet();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Salon Map')),
      body: Column(
        children: [
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
            child: Stack(
              children: [
                GoogleMap(
                  initialCameraPosition: CameraPosition(target: _initialCenter, zoom: 12),
                  onMapCreated: (controller) => _mapController = controller,
                  markers: _markers,
                  myLocationEnabled: true,
                  myLocationButtonEnabled: true,
                  zoomControlsEnabled: false,
                ),
                if (_isLoading) const Positioned(top: 16, left: 0, right: 0, child: Center(child: LoadingIndicator())),
              ],
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
          _load();
        },
      ),
    );
  }
}
