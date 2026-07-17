import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

class ServicesScreen extends StatefulWidget {
  const ServicesScreen({super.key});

  @override
  State<ServicesScreen> createState() => _ServicesScreenState();
}

class _ServicesScreenState extends State<ServicesScreen> {
  final _nameController = TextEditingController();
  List<SalonModel> _salons = [];
  int? _salonId;
  List<SalonServiceModel> _services = [];
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
    final result = await context.read<SalonServiceProvider>().get(filter: {
      'salonId': _salonId,
      'name': _nameController.text.isEmpty ? null : _nameController.text,
      'pageSize': 100,
    });
    setState(() {
      _services = result.items;
      _isLoading = false;
    });
  }

  Future<void> _openForm({SalonServiceModel? service}) async {
    final nameController = TextEditingController(text: service?.name);
    final descController = TextEditingController(text: service?.description);
    final durationController = TextEditingController(text: service?.durationMinutes.toString() ?? '30');
    final priceController = TextEditingController(text: service?.price.toString() ?? '0');
    bool isActive = service?.isActive ?? true;
    int? salonId = service?.salonId ?? _salonId ?? _salons.firstOrNull?.id;

    List<StaffModel> availableStaff = [];
    final selectedStaffIds = <int>{};

    Future<void> loadStaffForSalon(int? salon, void Function(void Function()) setDialogState) async {
      if (salon == null) return;
      final result = await context.read<StaffProvider>().get(filter: {'salonId': salon, 'isActive': true, 'pageSize': 100});
      selectedStaffIds
        ..clear()
        ..addAll(service == null ? const [] : result.items.where((s) => s.serviceIds.contains(service.id)).map((s) => s.id));
      setDialogState(() => availableStaff = result.items);
    }

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) {
          if (availableStaff.isEmpty && salonId != null) {
            // Kick off the initial staff load once, on first build.
            Future.microtask(() => loadStaffForSalon(salonId, setDialogState));
          }
          return AlertDialog(
            title: Text(service == null ? 'Add service' : 'Edit service'),
            content: SizedBox(
              width: 400,
              child: SingleChildScrollView(
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
                      onChanged: (v) {
                        setDialogState(() {
                          salonId = v;
                          availableStaff = [];
                        });
                        loadStaffForSalon(v, setDialogState);
                      },
                    ),
                    TextField(controller: nameController, decoration: const InputDecoration(labelText: 'Name')),
                    TextField(controller: descController, decoration: const InputDecoration(labelText: 'Description')),
                    TextField(
                      controller: durationController,
                      decoration: const InputDecoration(labelText: 'Duration (minutes)'),
                      keyboardType: TextInputType.number,
                    ),
                    TextField(
                      controller: priceController,
                      decoration: const InputDecoration(labelText: 'Price'),
                      keyboardType: TextInputType.number,
                    ),
                    SwitchListTile(
                      value: isActive,
                      title: const Text('Active'),
                      onChanged: (v) => setDialogState(() => isActive = v),
                    ),
                    const Align(alignment: Alignment.centerLeft, child: Padding(padding: EdgeInsets.only(top: 8), child: Text('Staff who perform this service'))),
                    if (availableStaff.isEmpty)
                      const Padding(
                        padding: EdgeInsets.symmetric(vertical: 8),
                        child: Text('No staff at this salon yet. Add staff first from the Staff screen.', style: TextStyle(color: Colors.grey)),
                      )
                    else
                      ...availableStaff.map((s) => CheckboxListTile(
                            dense: true,
                            value: selectedStaffIds.contains(s.id),
                            title: Text(s.fullName ?? 'Staff #${s.id}'),
                            onChanged: (v) => setDialogState(() {
                              if (v == true) {
                                selectedStaffIds.add(s.id);
                              } else {
                                selectedStaffIds.remove(s.id);
                              }
                            }),
                          )),
                  ],
                ),
              ),
            ),
            actions: [
              TextButton(onPressed: () => Navigator.of(context).pop(false), child: const Text('Cancel')),
              FilledButton(onPressed: () => Navigator.of(context).pop(true), child: const Text('Save')),
            ],
          );
        },
      ),
    );

    if (result != true || salonId == null) return;

    final request = {
      'salonId': salonId,
      'name': nameController.text,
      'description': descController.text,
      'durationMinutes': int.tryParse(durationController.text) ?? 30,
      'price': double.tryParse(priceController.text) ?? 0,
      'isActive': isActive,
    };

    int serviceId;
    if (service == null) {
      final created = await context.read<SalonServiceProvider>().insert(request);
      serviceId = created.id;
    } else {
      await context.read<SalonServiceProvider>().update(service.id, request);
      serviceId = service.id;
    }

    // Reconcile staff <-> service assignments to match what was checked in the dialog.
    final staffProvider = context.read<StaffProvider>();
    for (final staff in availableStaff) {
      final shouldHave = selectedStaffIds.contains(staff.id);
      final currentlyHas = staff.serviceIds.contains(serviceId);
      if (shouldHave == currentlyHas) continue;

      final updatedServiceIds = shouldHave
          ? [...staff.serviceIds, serviceId]
          : staff.serviceIds.where((id) => id != serviceId).toList();

      await staffProvider.update(staff.id, {
        'role': staff.role,
        'bio': staff.bio,
        'profileImageUrl': staff.profileImageUrl,
        'isActive': staff.isActive,
        'serviceIds': updatedServiceIds,
      });
    }

    if (mounted) showSuccessSnackBar(context, service == null ? 'Service created.' : 'Service updated.');
    _load();
  }

  Future<void> _delete(SalonServiceModel service) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Delete service',
      message: 'Delete "${service.name}"? This cannot be undone.',
    );
    if (!confirmed) return;
    await context.read<SalonServiceProvider>().remove(service.id);
    if (mounted) showSuccessSnackBar(context, 'Service deleted.');
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
              FilledButton.icon(icon: const Icon(Icons.add), label: const Text('Add service'), onPressed: () => _openForm()),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : SingleChildScrollView(
                    child: DataTable(
                      columns: const [
                        DataColumn(label: Text('Name')),
                        DataColumn(label: Text('Duration')),
                        DataColumn(label: Text('Price')),
                        DataColumn(label: Text('Active')),
                        DataColumn(label: Text('Actions')),
                      ],
                      rows: _services
                          .map((s) => DataRow(cells: [
                                DataCell(Text(s.name)),
                                DataCell(Text('${s.durationMinutes} min')),
                                DataCell(Text('\$${s.price.toStringAsFixed(2)}')),
                                DataCell(Icon(s.isActive ? Icons.check_circle : Icons.cancel, color: s.isActive ? Colors.green : Colors.grey)),
                                DataCell(Row(children: [
                                  IconButton(icon: const Icon(Icons.edit), onPressed: () => _openForm(service: s)),
                                  IconButton(icon: const Icon(Icons.delete, color: Colors.red), onPressed: () => _delete(s)),
                                ])),
                              ]))
                          .toList(),
                    ),
                  ),
          ),
        ],
      ),
    );
  }
}
