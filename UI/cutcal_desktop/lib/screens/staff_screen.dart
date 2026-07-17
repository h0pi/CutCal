import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

class StaffScreen extends StatefulWidget {
  const StaffScreen({super.key});

  @override
  State<StaffScreen> createState() => _StaffScreenState();
}

class _StaffScreenState extends State<StaffScreen> {
  final _nameController = TextEditingController();
  List<SalonModel> _salons = [];
  int? _salonId;
  List<StaffModel> _staff = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 100});
    setState(() => _salons = salons.items);
    await _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<StaffProvider>().get(filter: {
      'salonId': _salonId,
      'name': _nameController.text.isEmpty ? null : _nameController.text,
      'pageSize': 100,
    });
    setState(() {
      _staff = result.items;
      _isLoading = false;
    });
  }

  Future<void> _openAddForm() async {
    final userIdController = TextEditingController();
    final roleController = TextEditingController(text: 'Stylist');
    int? salonId = _salonId ?? _salons.firstOrNull?.id;
    List<SalonServiceModel> availableServices = [];
    final selectedServiceIds = <int>{};

    if (salonId != null) {
      final services = await context.read<SalonServiceProvider>().get(filter: {'salonId': salonId, 'pageSize': 100});
      availableServices = services.items;
    }

    if (!mounted) return;

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: const Text('Add staff member'),
          content: SizedBox(
            width: 420,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                DropdownButtonFormField<int>(
                  initialValue: salonId,
                  isExpanded: true,
                  decoration: const InputDecoration(labelText: 'Salon'),
                  items: _salons
                      .map((s) => DropdownMenuItem(value: s.id, child: Text(s.name, overflow: TextOverflow.ellipsis)))
                      .toList(),
                  onChanged: (v) async {
                    setDialogState(() => salonId = v);
                    if (v != null) {
                      final services = await context.read<SalonServiceProvider>().get(filter: {'salonId': v, 'pageSize': 100});
                      setDialogState(() => availableServices = services.items);
                    }
                  },
                ),
                TextField(
                  controller: userIdController,
                  decoration: const InputDecoration(labelText: 'User ID (existing account)'),
                  keyboardType: TextInputType.number,
                ),
                TextField(controller: roleController, decoration: const InputDecoration(labelText: 'Role / title')),
                const SizedBox(height: 8),
                Align(alignment: Alignment.centerLeft, child: Text('Services performed', style: Theme.of(context).textTheme.labelLarge)),
                Wrap(
                  spacing: 6,
                  children: availableServices
                      .map((s) => FilterChip(
                            label: Text(s.name),
                            selected: selectedServiceIds.contains(s.id),
                            onSelected: (v) => setDialogState(() => v ? selectedServiceIds.add(s.id) : selectedServiceIds.remove(s.id)),
                          ))
                      .toList(),
                ),
              ],
            ),
          ),
          actions: [
            TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
            FilledButton(onPressed: () => Navigator.of(context).pop(true), child: const Text('Save')),
          ],
        ),
      ),
    );

    if (result != true || salonId == null) return;

    await context.read<StaffProvider>().insert({
      'salonId': salonId,
      'userId': int.tryParse(userIdController.text) ?? 0,
      'role': roleController.text,
      'serviceIds': selectedServiceIds.toList(),
    });
    if (mounted) showSuccessSnackBar(context, 'Staff member added.');
    _load();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Wrap(
            spacing: 12,
            runSpacing: 12,
            crossAxisAlignment: WrapCrossAlignment.center,
            children: [
              SizedBox(
                width: 220,
                child: DropdownButtonFormField<int?>(
                  initialValue: _salonId,
                  isExpanded: true,
                  decoration: const InputDecoration(labelText: 'Salon', border: OutlineInputBorder(), isDense: true),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('All salons')),
                    ..._salons.map((s) => DropdownMenuItem(value: s.id, child: Text(s.name, overflow: TextOverflow.ellipsis))),
                  ],
                  onChanged: (v) {
                    setState(() => _salonId = v);
                    _load();
                  },
                ),
              ),
              SizedBox(
                width: 220,
                child: TextField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: 'Search by name', border: OutlineInputBorder(), isDense: true),
                  onSubmitted: (_) => _load(),
                ),
              ),
              FilledButton.icon(icon: const Icon(Icons.add), label: const Text('Add staff'), onPressed: _openAddForm),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : ListView.builder(
                    itemCount: _staff.length,
                    itemBuilder: (context, index) {
                      final s = _staff[index];
                      return Card(
                        child: ListTile(
                          leading: CircleAvatar(backgroundImage: s.profileImageUrl != null ? NetworkImage(s.profileImageUrl!) : null),
                          title: Text(s.fullName ?? 'Staff #${s.id}'),
                          subtitle: Text('${s.role} • ${s.serviceIds.length} service(s)'),
                          trailing: Icon(s.isActive ? Icons.check_circle : Icons.pause_circle, color: s.isActive ? Colors.green : Colors.grey),
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
