import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';

const _pickedZoom = 16.0;

/// Full-screen map for choosing a salon's location: tap or drag the pin,
/// or type an address and pick one of the matches returned by the API.
class LocationPickerScreen extends StatefulWidget {
  final LatLng initial;
  final String? initialQuery;

  const LocationPickerScreen({super.key, required this.initial, this.initialQuery});

  @override
  State<LocationPickerScreen> createState() => _LocationPickerScreenState();
}

class _LocationPickerScreenState extends State<LocationPickerScreen> {
  late final TextEditingController _searchController = TextEditingController(text: widget.initialQuery);
  late LatLng _picked = widget.initial;
  GoogleMapController? _mapController;
  bool _isSearching = false;
  String? _error;

  Future<void> _search() async {
    final query = _searchController.text.trim();
    if (query.length < 3) {
      setState(() => _error = 'Enter at least 3 characters, e.g. "Ferhadija 1, Sarajevo".');
      return;
    }

    setState(() {
      _isSearching = true;
      _error = null;
    });
    try {
      final results = await context.read<GeocodingProvider>().search(query);
      if (!mounted) return;
      if (results.isEmpty) {
        setState(() => _error = 'No matches found. Try adding the city, e.g. "Ferhadija 1, Sarajevo".');
      } else if (results.length == 1) {
        _moveTo(results.first);
      } else {
        final chosen = await _chooseResult(results);
        if (chosen != null) _moveTo(chosen);
      }
    } on ApiClientException catch (e) {
      if (mounted) setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _isSearching = false);
    }
  }

  Future<GeocodeResultModel?> _chooseResult(List<GeocodeResultModel> results) {
    return showModalBottomSheet<GeocodeResultModel>(
      context: context,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      builder: (context) => SafeArea(
        child: ListView(
          shrinkWrap: true,
          padding: const EdgeInsets.symmetric(vertical: 12),
          children: [
            const Padding(
              padding: EdgeInsets.fromLTRB(20, 4, 20, 8),
              child: Text('Choose the correct place', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
            ),
            ...results.map((r) => ListTile(
                  leading: const Icon(Icons.place_outlined, color: AppColors.primary),
                  title: Text(r.displayName, maxLines: 2, overflow: TextOverflow.ellipsis, style: const TextStyle(fontSize: 14)),
                  onTap: () => Navigator.of(context).pop(r),
                )),
          ],
        ),
      ),
    );
  }

  void _moveTo(GeocodeResultModel result) {
    final target = LatLng(result.latitude, result.longitude);
    setState(() => _picked = target);
    _mapController?.animateCamera(CameraUpdate.newLatLngZoom(target, _pickedZoom));
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Salon location')),
      body: Stack(
        children: [
          GoogleMap(
            initialCameraPosition: CameraPosition(target: widget.initial, zoom: 14),
            onMapCreated: (controller) => _mapController = controller,
            onTap: (position) => setState(() => _picked = position),
            zoomControlsEnabled: false,
            markers: {
              Marker(
                markerId: const MarkerId('picked'),
                position: _picked,
                draggable: true,
                icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueViolet),
                onDragEnd: (position) => setState(() => _picked = position),
              ),
            },
          ),
          Positioned(
            top: 12,
            left: 16,
            right: 16,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Material(
                  elevation: 3,
                  borderRadius: BorderRadius.circular(28),
                  child: TextField(
                    controller: _searchController,
                    textInputAction: TextInputAction.search,
                    onSubmitted: (_) => _search(),
                    decoration: InputDecoration(
                      hintText: 'Search address or place',
                      prefixIcon: const Icon(Icons.search),
                      suffixIcon: _isSearching
                          ? const Padding(padding: EdgeInsets.all(12), child: SizedBox(width: 20, height: 20, child: CircularProgressIndicator(strokeWidth: 2)))
                          : IconButton(icon: const Icon(Icons.arrow_forward), onPressed: _search),
                    ),
                  ),
                ),
                if (_error != null)
                  Padding(
                    padding: const EdgeInsets.only(top: 8, left: 8),
                    child: Text(_error!, style: const TextStyle(color: AppColors.declinedText, fontSize: 12, fontWeight: FontWeight.w600)),
                  ),
              ],
            ),
          ),
        ],
      ),
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              const Text('Tap the map or drag the pin to fine-tune the position.', style: TextStyle(color: AppColors.textSecondary, fontSize: 12)),
              const SizedBox(height: 10),
              SizedBox(
                width: double.infinity,
                child: FilledButton(onPressed: () => Navigator.of(context).pop(_picked), child: const Text('Use this location')),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
