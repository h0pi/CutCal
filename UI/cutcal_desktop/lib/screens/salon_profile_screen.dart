import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/auth_provider.dart';
import '../providers/entity_providers.dart';
import '../utils/api_client_exception.dart';
import '../utils/utils_widgets.dart';
import '../utils/image_url.dart';

class SalonProfileScreen extends StatefulWidget {
  const SalonProfileScreen({super.key});

  @override
  State<SalonProfileScreen> createState() => _SalonProfileScreenState();
}

class _SalonProfileScreenState extends State<SalonProfileScreen> {
  List<SalonModel> _salons = [];
  SalonModel? _selectedSalon;
  List<SalonGalleryModel> _gallery = [];
  final _nameController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _phoneController = TextEditingController();
  final _emailController = TextEditingController();
  final _addressController = TextEditingController();
  double? _latitude;
  double? _longitude;
  String? _locationLabel;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 100});
    setState(() => _salons = salons.items);
    if (_salons.isNotEmpty) await _selectSalon(_salons.first);
  }

  Future<void> _selectSalon(SalonModel salon) async {
    final gallery = await context.read<SalonProvider>().getGallery(salon.id);
    setState(() {
      _selectedSalon = salon;
      _nameController.text = salon.name;
      _descriptionController.text = salon.description ?? '';
      _phoneController.text = salon.phone ?? '';
      _emailController.text = salon.email ?? '';
      _addressController.text = salon.address;
      _latitude = salon.latitude;
      _longitude = salon.longitude;
      _locationLabel = null;
      _gallery = gallery;
    });
  }

  Future<void> _pickCoordinates() async {
    final salon = _selectedSalon;
    if (salon == null) return;

    final queryController = TextEditingController(text: '${_addressController.text.trim()}, ${salon.cityName ?? ''}'.replaceAll(RegExp(r',\s*$'), ''));
    List<GeocodeResultModel> results = [];
    GeocodeResultModel? chosen;
    String? errorText;
    var isSearching = false;

    Future<void> runSearch(StateSetter setDialogState) async {
      setDialogState(() {
        isSearching = true;
        errorText = null;
      });
      try {
        final found = await context.read<GeocodingProvider>().search(queryController.text.trim());
        setDialogState(() {
          results = found;
          chosen = found.firstOrNull;
          if (found.isEmpty) errorText = 'No matches found. Try adding the city.';
        });
      } on ApiClientException catch (e) {
        setDialogState(() => errorText = e.message);
      } finally {
        setDialogState(() => isSearching = false);
      }
    }

    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Row(
            children: [
              const Expanded(child: Text('Find salon location')),
              IconButton(icon: const Icon(Icons.close), onPressed: () => Navigator.of(context).pop(false)),
            ],
          ),
          content: SizedBox(
            width: 480,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                TextField(
                  controller: queryController,
                  decoration: InputDecoration(
                    labelText: 'Street and city',
                    hintText: 'Ferhadija 1, Sarajevo',
                    errorText: errorText,
                    suffixIcon: isSearching
                        ? const Padding(padding: EdgeInsets.all(12), child: SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2)))
                        : IconButton(icon: const Icon(Icons.search), onPressed: () => runSearch(setDialogState)),
                  ),
                  onSubmitted: (_) => runSearch(setDialogState),
                ),
                const SizedBox(height: 8),
                if (results.isNotEmpty)
                  Flexible(
                    child: SingleChildScrollView(
                      child: RadioGroup<GeocodeResultModel>(
                        groupValue: chosen,
                        onChanged: (v) => setDialogState(() => chosen = v),
                        child: Column(
                          children: results
                              .map((r) => RadioListTile<GeocodeResultModel>(
                                    value: r,
                                    dense: true,
                                    title: Text(r.displayName, maxLines: 2, overflow: TextOverflow.ellipsis),
                                  ))
                              .toList(),
                        ),
                      ),
                    ),
                  )
                else
                  const Text('Type an address and press Enter to search.', style: TextStyle(color: Colors.grey)),
              ],
            ),
          ),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
            FilledButton(onPressed: chosen == null ? null : () => Navigator.of(context).pop(true), child: const Text('Use this location')),
          ],
        ),
      ),
    );

    final picked = chosen;
    if (confirmed != true || picked == null || !mounted) return;
    setState(() {
      _latitude = picked.latitude;
      _longitude = picked.longitude;
      _locationLabel = picked.displayName;
    });
  }

  Future<void> _save() async {
    if (_selectedSalon == null) return;
    final salon = _selectedSalon!;
    await context.read<SalonProvider>().update(salon.id, {
      'name': _nameController.text,
      'salonCategoryId': salon.salonCategoryId,
      'description': _descriptionController.text,
      'address': _addressController.text,
      'cityId': salon.cityId,
      'latitude': _latitude ?? salon.latitude,
      'longitude': _longitude ?? salon.longitude,
      'phone': _phoneController.text,
      'email': _emailController.text,
      'profileImageUrl': salon.profileImageUrl,
      'autoConfirm': salon.autoConfirm,
      'workingHours': const [],
    });
    if (mounted) showSuccessSnackBar(context, 'Salon profile updated.');
  }

  Future<void> _addGalleryImage() async {
    final urlController = TextEditingController();
    final url = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Add gallery image'),
        content: TextField(controller: urlController, decoration: const InputDecoration(labelText: 'Image URL')),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(urlController.text), child: const Text('Add')),
        ],
      ),
    );
    if (url == null || url.trim().isEmpty || _selectedSalon == null) return;

    try {
      await context.read<SalonProvider>().addGalleryImage(_selectedSalon!.id, url.trim());
      if (!mounted) return;
      showSuccessSnackBar(context, 'Gallery image added.');
      await _selectSalon(_selectedSalon!);
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _toggleFeatured(bool value) async {
    final salon = _selectedSalon;
    if (salon == null) return;
    try {
      final updated = await context.read<SalonProvider>().setFeatured(salon.id, value);
      if (!mounted) return;
      setState(() {
        _selectedSalon = updated;
        _salons = _salons.map((s) => s.id == updated.id ? updated : s).toList();
      });
      showSuccessSnackBar(context, value ? 'Salon is now featured.' : 'Salon is no longer featured.');
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _removeGalleryImage(SalonGalleryModel image) async {
    if (_selectedSalon == null) return;
    final confirmed = await showConfirmationDialog(context, title: 'Remove image', message: 'Remove this image from the gallery?');
    if (!confirmed) return;

    try {
      await context.read<SalonProvider>().removeGalleryImage(_selectedSalon!.id, image.id);
      if (!mounted) return;
      showSuccessSnackBar(context, 'Gallery image removed.');
      await _selectSalon(_selectedSalon!);
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (_salons.isEmpty || _selectedSalon == null) return const LoadingIndicator();

    return Padding(
      padding: const EdgeInsets.all(24),
      child: ListView(
        children: [
          DropdownButtonFormField<SalonModel>(
            initialValue: _selectedSalon,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Salon', border: OutlineInputBorder()),
            items: _salons
                .map((s) => DropdownMenuItem(value: s, child: Text(s.name, overflow: TextOverflow.ellipsis)))
                .toList(),
            onChanged: (v) {
              if (v != null) _selectSalon(v);
            },
          ),
          if (context.watch<AuthProvider>().role == 'Admin') ...[
            const SizedBox(height: 8),
            SwitchListTile(
              contentPadding: EdgeInsets.zero,
              title: const Text('Featured salon'),
              subtitle: const Text('Shown as promoted on the mobile app\'s home page.'),
              value: _selectedSalon!.isFeatured,
              onChanged: _toggleFeatured,
            ),
          ],
          const SizedBox(height: 24),
          Row(
            children: [
              CircleAvatar(
                radius: 40,
                backgroundImage: _selectedSalon!.profileImageUrl != null ? NetworkImage(resolveImageUrl(_selectedSalon!.profileImageUrl!)) : null,
              ),
              const SizedBox(width: 16),
              OutlinedButton.icon(icon: const Icon(Icons.upload), label: const Text('Change photo'), onPressed: () {}),
            ],
          ),
          const SizedBox(height: 24),
          TextField(controller: _nameController, decoration: const InputDecoration(labelText: 'Name', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          TextField(controller: _descriptionController, maxLines: 3, decoration: const InputDecoration(labelText: 'Description', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          TextField(controller: _phoneController, decoration: const InputDecoration(labelText: 'Phone', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          TextField(controller: _emailController, decoration: const InputDecoration(labelText: 'Email', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          Row(
            children: [
              Expanded(
                child: TextField(controller: _addressController, decoration: const InputDecoration(labelText: 'Address', border: OutlineInputBorder())),
              ),
              const SizedBox(width: 8),
              OutlinedButton.icon(icon: const Icon(Icons.place_outlined), label: const Text('Find on map'), onPressed: _pickCoordinates),
            ],
          ),
          if (_locationLabel != null)
            Padding(
              padding: const EdgeInsets.only(top: 8),
              child: Text('New location: $_locationLabel (save to apply)', style: const TextStyle(color: Colors.grey, fontSize: 12)),
            ),
          const SizedBox(height: 24),
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text('Gallery', style: Theme.of(context).textTheme.titleMedium),
              OutlinedButton.icon(icon: const Icon(Icons.add_photo_alternate), label: const Text('Add image'), onPressed: _addGalleryImage),
            ],
          ),
          const SizedBox(height: 8),
          if (_gallery.isEmpty)
            const Padding(
              padding: EdgeInsets.symmetric(vertical: 8),
              child: Text('No gallery images yet.', style: TextStyle(color: Colors.grey)),
            )
          else
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: _gallery
                  .map((g) => Stack(
                        children: [
                          ClipRRect(
                            borderRadius: BorderRadius.circular(8),
                            child: Image.network(resolveImageUrl(g.imageUrl), width: 120, height: 90, fit: BoxFit.cover),
                          ),
                          Positioned(
                            top: 2,
                            right: 2,
                            child: Material(
                              color: Colors.black54,
                              shape: const CircleBorder(),
                              child: IconButton(
                                icon: const Icon(Icons.close, size: 16, color: Colors.white),
                                tooltip: 'Remove image',
                                constraints: const BoxConstraints(minWidth: 28, minHeight: 28),
                                padding: EdgeInsets.zero,
                                onPressed: () => _removeGalleryImage(g),
                              ),
                            ),
                          ),
                        ],
                      ))
                  .toList(),
            ),
          const SizedBox(height: 24),
          FilledButton(onPressed: _save, child: const Text('Save changes')),
        ],
      ),
    );
  }
}
