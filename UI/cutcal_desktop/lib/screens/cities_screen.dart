import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/api_client_exception.dart';
import '../utils/utils_widgets.dart';

class CitiesScreen extends StatefulWidget {
  const CitiesScreen({super.key});

  @override
  State<CitiesScreen> createState() => _CitiesScreenState();
}

class _CitiesScreenState extends State<CitiesScreen> {
  final _searchController = TextEditingController();
  List<CityModel> _cities = [];
  List<CountryModel> _countries = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<CityProvider>().get(filter: {
      'name': _searchController.text.isEmpty ? null : _searchController.text,
      'pageSize': 100,
    });
    final countries = await context.read<CountryProvider>().get(filter: {'pageSize': 100});
    setState(() {
      _cities = result.items;
      _countries = countries.items;
      _isLoading = false;
    });
  }

  Future<CountryModel?> _addCountry(StateSetter setDialogState) async {
    final nameController = TextEditingController();
    final name = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Add country'),
        content: TextField(controller: nameController, autofocus: true, decoration: const InputDecoration(labelText: 'Country name')),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(nameController.text), child: const Text('Add')),
        ],
      ),
    );
    if (name == null || name.trim().isEmpty) return null;

    try {
      final created = await context.read<CountryProvider>().insert({'name': name.trim()});
      setDialogState(() => _countries = [..._countries, created]);
      return created;
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
      return null;
    }
  }

  Future<void> _openForm({CityModel? city}) async {
    final nameController = TextEditingController(text: city?.name);
    int? countryId = city?.countryId ?? _countries.firstOrNull?.id;

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text(city == null ? 'Add city' : 'Edit city'),
          content: SizedBox(
            width: 360,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextField(controller: nameController, decoration: const InputDecoration(labelText: 'Name')),
                const SizedBox(height: 16),
                Row(
                  children: [
                    Expanded(
                      child: DropdownButtonFormField<int>(
                        initialValue: countryId,
                        isExpanded: true,
                        decoration: const InputDecoration(labelText: 'Country'),
                        items: _countries.map((c) => DropdownMenuItem(value: c.id, child: Text(c.name))).toList(),
                        onChanged: (v) => setDialogState(() => countryId = v),
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.add_circle_outline),
                      tooltip: 'Add new country',
                      onPressed: () async {
                        final created = await _addCountry(setDialogState);
                        if (created != null) setDialogState(() => countryId = created.id);
                      },
                    ),
                  ],
                ),
              ],
            ),
          ),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
            FilledButton(
              onPressed: countryId == null ? null : () => Navigator.of(context).pop(true),
              child: const Text('Save'),
            ),
          ],
        ),
      ),
    );
    if (result != true || nameController.text.isEmpty || countryId == null) return;

    final request = {'name': nameController.text, 'countryId': countryId};
    if (city == null) {
      await context.read<CityProvider>().insert(request);
    } else {
      await context.read<CityProvider>().update(city.id, request);
    }
    if (mounted) showSuccessSnackBar(context, city == null ? 'City created.' : 'City updated.');
    _load();
  }

  Future<void> _delete(CityModel city) async {
    final confirmed = await showConfirmationDialog(context, title: 'Delete city', message: 'Delete "${city.name}"?');
    if (!confirmed) return;
    await context.read<CityProvider>().remove(city.id);
    if (mounted) showSuccessSnackBar(context, 'City deleted.');
    _load();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              SizedBox(
                width: 260,
                child: TextField(
                  controller: _searchController,
                  decoration: const InputDecoration(labelText: 'Search', border: OutlineInputBorder(), isDense: true),
                  onSubmitted: (_) => _load(),
                ),
              ),
              const SizedBox(width: 12),
              FilledButton.icon(icon: const Icon(Icons.add), label: const Text('Add city'), onPressed: () => _openForm()),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : ListView.builder(
                    itemCount: _cities.length,
                    itemBuilder: (context, index) {
                      final c = _cities[index];
                      return ListTile(
                        title: Text(c.name),
                        subtitle: Text(c.countryName ?? 'Unknown'),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            IconButton(icon: const Icon(Icons.edit), onPressed: () => _openForm(city: c)),
                            IconButton(icon: const Icon(Icons.delete, color: Colors.red), onPressed: () => _delete(c)),
                          ],
                        ),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }
}
