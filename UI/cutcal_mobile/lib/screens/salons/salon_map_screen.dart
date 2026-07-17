import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';
import 'salon_detail_screen.dart';

// TODO: replace the placeholder list below with a real google_maps_flutter
// GoogleMap widget rendering a Marker per salon, once an API key is configured.
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

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final categories = await context.read<SalonCategoryProvider>().get();
    final salons = await context.read<SalonProvider>().get(filter: {
      'pageSize': 100,
      'categoryId': _selectedCategoryId,
    });
    setState(() {
      _categories = categories.items;
      _salons = salons.items;
      _isLoading = false;
    });
  }

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
          Expanded(
            child: Container(
              margin: const EdgeInsets.all(12),
              decoration: BoxDecoration(border: Border.all(color: Colors.grey.shade300), borderRadius: BorderRadius.circular(12)),
              child: _isLoading
                  ? const LoadingIndicator()
                  : Column(
                      children: [
                        const Padding(
                          padding: EdgeInsets.all(12),
                          child: Row(
                            children: [
                              Icon(Icons.map_outlined),
                              SizedBox(width: 8),
                              Expanded(child: Text('Map pins (Google Maps integration pending)')),
                            ],
                          ),
                        ),
                        Expanded(
                          child: ListView.builder(
                            itemCount: _salons.length,
                            itemBuilder: (context, index) {
                              final salon = _salons[index];
                              return ListTile(
                                leading: const Icon(Icons.location_pin, color: Colors.red),
                                title: Text(salon.name),
                                subtitle: Text('${salon.latitude.toStringAsFixed(4)}, ${salon.longitude.toStringAsFixed(4)}'),
                                onTap: () => Navigator.of(context).push(
                                  MaterialPageRoute(builder: (_) => SalonDetailScreen(salonId: salon.id)),
                                ),
                              );
                            },
                          ),
                        ),
                      ],
                    ),
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
