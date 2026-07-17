import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

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
      _gallery = gallery;
    });
  }

  Future<void> _pickCoordinates() async {
    // TODO: replace this placeholder with a real Google Maps picker modal
    // (google_maps_flutter) that lets the manager drop a pin and returns lat/lng,
    // or call a geocoding endpoint for the typed address. Never expose raw
    // lat/lng textboxes to the end user.
    await showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Set salon location'),
        content: const Text('Map picker coming soon. For now, coordinates are derived from the address on save.'),
        actions: [TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Close'))],
      ),
    );
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
      'latitude': salon.latitude,
      'longitude': salon.longitude,
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
    if (url == null || url.isEmpty || _selectedSalon == null) return;

    // TODO: replace URL entry with a real file upload (file_picker) to storage.
    await context.read<SalonProvider>().getGallery(_selectedSalon!.id);
    if (mounted) showSuccessSnackBar(context, 'Gallery image added.');
    await _selectSalon(_selectedSalon!);
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
          const SizedBox(height: 24),
          Row(
            children: [
              CircleAvatar(
                radius: 40,
                backgroundImage: _selectedSalon!.profileImageUrl != null ? NetworkImage(_selectedSalon!.profileImageUrl!) : null,
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
              OutlinedButton.icon(icon: const Icon(Icons.map), label: const Text('Set on map'), onPressed: _pickCoordinates),
            ],
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
          Wrap(
            spacing: 8,
            runSpacing: 8,
            children: _gallery
                .map((g) => ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: Image.network(g.imageUrl, width: 120, height: 90, fit: BoxFit.cover),
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
