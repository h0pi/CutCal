import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';

class ManagerServicesScreen extends StatefulWidget {
  const ManagerServicesScreen({super.key});

  @override
  State<ManagerServicesScreen> createState() => _ManagerServicesScreenState();
}

class _ManagerServicesScreenState extends State<ManagerServicesScreen> {
  List<SalonModel> _salons = [];
  List<SalonServiceModel> _services = [];
  bool _isLoading = true;
  String? _loadError;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });
    try {
      // Both endpoints are auto-scoped server-side to salons this manager owns,
      // and independent of each other, so they're fetched concurrently.
      final salonsFuture = context.read<SalonProvider>().get(filter: {'pageSize': 20});
      final servicesFuture = context.read<SalonServiceProvider>().get(filter: {'pageSize': 200});

      final salons = await salonsFuture;
      final services = await servicesFuture;
      if (!mounted) return;
      setState(() {
        _salons = salons.items;
        _services = services.items;
        _isLoading = false;
      });
    } on ApiClientException catch (e) {
      if (!mounted) return;
      setState(() {
        _loadError = e.message;
        _isLoading = false;
      });
    }
  }

  Future<void> _openForm({SalonServiceModel? service}) async {
    final salonId = service?.salonId ?? _salons.firstOrNull?.id;
    if (salonId == null) {
      showErrorSnackBar(context, 'Set up your salon first from the Salon tab.');
      return;
    }

    final nameController = TextEditingController(text: service?.name);
    final descController = TextEditingController(text: service?.description);
    final durationController = TextEditingController(text: service?.durationMinutes.toString() ?? '30');
    final priceController = TextEditingController(text: service?.price.toString() ?? '');
    bool isActive = service?.isActive ?? true;

    final saved = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      builder: (context) => StatefulBuilder(
        builder: (context, setSheetState) => Padding(
          padding: EdgeInsets.only(bottom: MediaQuery.of(context).viewInsets.bottom),
          child: SafeArea(
            child: Padding(
              padding: const EdgeInsets.fromLTRB(20, 12, 20, 20),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Center(
                    child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey.shade300, borderRadius: BorderRadius.circular(2))),
                  ),
                  const SizedBox(height: 16),
                  Text(service == null ? 'Add service' : 'Edit service', style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                  const SizedBox(height: 20),
                  TextField(controller: nameController, decoration: const InputDecoration(labelText: 'Service name')),
                  const SizedBox(height: 12),
                  TextField(controller: descController, decoration: const InputDecoration(labelText: 'Description'), maxLines: 2),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                        child: TextField(
                          controller: durationController,
                          decoration: const InputDecoration(labelText: 'Duration (min)'),
                          keyboardType: TextInputType.number,
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: TextField(
                          controller: priceController,
                          decoration: const InputDecoration(labelText: 'Price (\$)'),
                          keyboardType: const TextInputType.numberWithOptions(decimal: true),
                        ),
                      ),
                    ],
                  ),
                  SwitchListTile(
                    contentPadding: EdgeInsets.zero,
                    value: isActive,
                    title: const Text('Active', style: TextStyle(color: AppColors.textPrimary)),
                    onChanged: (v) => setSheetState(() => isActive = v),
                  ),
                  const SizedBox(height: 12),
                  SizedBox(
                    width: double.infinity,
                    child: FilledButton(onPressed: () => Navigator.of(context).pop(true), child: const Text('Save')),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );

    if (saved != true) return;
    if (nameController.text.trim().isEmpty) {
      showErrorSnackBar(context, 'Service name is required.');
      return;
    }

    final request = {
      'salonId': salonId,
      'name': nameController.text.trim(),
      'description': descController.text.trim().isEmpty ? null : descController.text.trim(),
      'durationMinutes': int.tryParse(durationController.text) ?? 30,
      'price': double.tryParse(priceController.text) ?? 0,
      'isActive': isActive,
    };

    try {
      if (service == null) {
        await context.read<SalonServiceProvider>().insert(request);
        if (mounted) showSuccessSnackBar(context, 'Service added.');
      } else {
        await context.read<SalonServiceProvider>().update(service.id, request);
        if (mounted) showSuccessSnackBar(context, 'Service updated.');
      }
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _delete(SalonServiceModel service) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Delete service',
      message: 'Delete "${service.name}"? This cannot be undone.',
    );
    if (!confirmed) return;
    try {
      await context.read<SalonServiceProvider>().remove(service.id);
      if (mounted) showSuccessSnackBar(context, 'Service deleted.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(
        title: const Text('Menu'),
        actions: [
          IconButton(icon: const Icon(Icons.add), onPressed: () => _openForm()),
        ],
      ),
      body: _isLoading
          ? const LoadingIndicator()
          : _loadError != null
              ? ErrorState(message: _loadError!, onRetry: _load)
              : _services.isEmpty
                  ? const Center(child: Text('No services yet. Tap + to add one.', style: TextStyle(color: AppColors.textSecondary)))
                  : ListView.separated(
                  padding: const EdgeInsets.all(16),
                  itemCount: _services.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 12),
                  itemBuilder: (context, index) {
                    final s = _services[index];
                    return Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        border: Border.all(color: const Color(0xFFEDEAF4)),
                      ),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Expanded(
                                child: Text(s.name, style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                              ),
                              Text('\$${s.price.toStringAsFixed(2)}', style: const TextStyle(fontSize: 15, fontWeight: FontWeight.bold, color: AppColors.primary)),
                            ],
                          ),
                          if (s.description != null && s.description!.isNotEmpty) ...[
                            const SizedBox(height: 4),
                            Text(s.description!, style: const TextStyle(color: AppColors.textSecondary, fontSize: 13)),
                          ],
                          const SizedBox(height: 10),
                          Row(
                            children: [
                              _tag('${s.durationMinutes} min'),
                              const SizedBox(width: 8),
                              if (!s.isActive) _tag('Inactive', color: AppColors.neutralBg, textColor: AppColors.neutralText),
                              const Spacer(),
                              TextButton(onPressed: () => _openForm(service: s), child: const Text('Edit')),
                              TextButton(
                                style: TextButton.styleFrom(foregroundColor: AppColors.declinedText),
                                onPressed: () => _delete(s),
                                child: const Text('Delete'),
                              ),
                            ],
                          ),
                        ],
                      ),
                    );
                  },
                ),
    );
  }

  Widget _tag(String label, {Color color = AppColors.primaryLight, Color textColor = AppColors.primaryDark}) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(color: color, borderRadius: BorderRadius.circular(20)),
      child: Text(label, style: TextStyle(color: textColor, fontSize: 11, fontWeight: FontWeight.w600)),
    );
  }
}
