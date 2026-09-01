import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/app_theme.dart';
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
  final _searchController = TextEditingController();
  List<SalonModel> _salons = [];
  bool _isLoading = true;
  GoogleMapController? _mapController;
  LatLng _initialCenter = _fallbackCenter;
  SalonModel? _selectedSalon;

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

  Future<void> _recenter() async {
    try {
      final position = await Geolocator.getCurrentPosition();
      final target = LatLng(position.latitude, position.longitude);
      _mapController?.animateCamera(CameraUpdate.newLatLngZoom(target, 14));
    } catch (_) {
      // Ignore — button just won't move the camera this time.
    }
  }

  Future<void> _load({String? name}) async {
    setState(() => _isLoading = true);
    final salons = await context.read<SalonProvider>().get(filter: {
      'pageSize': 100,
      'name': name,
    });
    if (!mounted) return;
    setState(() {
      _salons = salons.items;
      _isLoading = false;
    });
  }

  Set<Marker> get _markers => _salons
      .map((salon) => Marker(
            markerId: MarkerId('salon-${salon.id}'),
            position: LatLng(salon.latitude, salon.longitude),
            icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueViolet),
            onTap: () => setState(() => _selectedSalon = salon),
          ))
      .toSet();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          GoogleMap(
            initialCameraPosition: CameraPosition(target: _initialCenter, zoom: 12),
            onMapCreated: (controller) => _mapController = controller,
            markers: _markers,
            myLocationEnabled: true,
            myLocationButtonEnabled: false,
            zoomControlsEnabled: false,
            onTap: (_) => setState(() => _selectedSalon = null),
          ),
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 0),
              child: Row(
                children: [
                  Expanded(
                    child: Material(
                      elevation: 3,
                      borderRadius: BorderRadius.circular(28),
                      child: TextField(
                        controller: _searchController,
                        decoration: const InputDecoration(hintText: 'Search this area', prefixIcon: Icon(Icons.search)),
                        onSubmitted: (v) => _load(name: v.isEmpty ? null : v),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Material(
                    elevation: 3,
                    shape: const CircleBorder(),
                    color: AppColors.primary,
                    child: InkWell(
                      customBorder: const CircleBorder(),
                      onTap: _recenter,
                      child: const SizedBox(width: 48, height: 48, child: Icon(Icons.my_location, color: Colors.white)),
                    ),
                  ),
                ],
              ),
            ),
          ),
          if (_isLoading) const Positioned(top: 80, left: 0, right: 0, child: Center(child: LoadingIndicator())),
          if (_selectedSalon != null)
            Positioned(
              left: 16,
              right: 16,
              bottom: 16,
              child: _SalonPreviewCard(
                salon: _selectedSalon!,
                onViewSalon: () => Navigator.of(context).push(
                  MaterialPageRoute(builder: (_) => SalonDetailScreen(salonId: _selectedSalon!.id)),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _SalonPreviewCard extends StatelessWidget {
  final SalonModel salon;
  final VoidCallback onViewSalon;

  const _SalonPreviewCard({required this.salon, required this.onViewSalon});

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: salon.profileImageUrl != null
                      ? Image.network(salon.profileImageUrl!, width: 56, height: 56, fit: BoxFit.cover, cacheWidth: 150)
                      : Container(width: 56, height: 56, color: AppColors.primaryLight, child: const Icon(Icons.storefront, color: AppColors.primary)),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          Expanded(child: Text(salon.name, style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary), overflow: TextOverflow.ellipsis)),
                          const Icon(Icons.star_rounded, color: AppColors.starColor, size: 16),
                          Text(salon.avgRating.toStringAsFixed(1), style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                        ],
                      ),
                      const SizedBox(height: 2),
                      Text(salon.address, style: const TextStyle(color: AppColors.textSecondary, fontSize: 12), overflow: TextOverflow.ellipsis),
                      if (salon.distanceKm != null)
                        Text('${salon.distanceKm!.toStringAsFixed(1)} mi away', style: const TextStyle(color: AppColors.primaryDark, fontSize: 12, fontWeight: FontWeight.w600)),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Expanded(
                  child: FilledButton.icon(
                    onPressed: () {},
                    icon: const Icon(Icons.directions, size: 18),
                    label: const Text('Directions'),
                    style: FilledButton.styleFrom(minimumSize: const Size.fromHeight(44)),
                  ),
                ),
                const SizedBox(width: 10),
                Expanded(
                  child: OutlinedButton(
                    onPressed: onViewSalon,
                    style: OutlinedButton.styleFrom(minimumSize: const Size.fromHeight(44)),
                    child: const Text('View salon'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
