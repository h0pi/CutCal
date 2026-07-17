import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

class SalonCategoriesScreen extends StatefulWidget {
  const SalonCategoriesScreen({super.key});

  @override
  State<SalonCategoriesScreen> createState() => _SalonCategoriesScreenState();
}

class _SalonCategoriesScreenState extends State<SalonCategoriesScreen> {
  final _searchController = TextEditingController();
  List<SalonCategoryModel> _categories = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<SalonCategoryProvider>().get(filter: {
      'name': _searchController.text.isEmpty ? null : _searchController.text,
      'pageSize': 100,
    });
    setState(() {
      _categories = result.items;
      _isLoading = false;
    });
  }

  Future<void> _openForm({SalonCategoryModel? category}) async {
    final controller = TextEditingController(text: category?.name);
    final result = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(category == null ? 'Add category' : 'Edit category'),
        content: TextField(controller: controller, decoration: const InputDecoration(labelText: 'Name')),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(true), child: const Text('Save')),
        ],
      ),
    );
    if (result != true || controller.text.isEmpty) return;

    if (category == null) {
      await context.read<SalonCategoryProvider>().insert({'name': controller.text});
    } else {
      await context.read<SalonCategoryProvider>().update(category.id, {'name': controller.text});
    }
    if (mounted) showSuccessSnackBar(context, category == null ? 'Category created.' : 'Category updated.');
    _load();
  }

  Future<void> _delete(SalonCategoryModel category) async {
    final confirmed = await showConfirmationDialog(context, title: 'Delete category', message: 'Delete "${category.name}"?');
    if (!confirmed) return;
    await context.read<SalonCategoryProvider>().remove(category.id);
    if (mounted) showSuccessSnackBar(context, 'Category deleted.');
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
              FilledButton.icon(icon: const Icon(Icons.add), label: const Text('Add category'), onPressed: () => _openForm()),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : ListView.builder(
                    itemCount: _categories.length,
                    itemBuilder: (context, index) {
                      final c = _categories[index];
                      return ListTile(
                        title: Text(c.name),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            IconButton(icon: const Icon(Icons.edit), onPressed: () => _openForm(category: c)),
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
