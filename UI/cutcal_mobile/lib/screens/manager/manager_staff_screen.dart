import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';

class ManagerStaffScreen extends StatefulWidget {
  const ManagerStaffScreen({super.key});

  @override
  State<ManagerStaffScreen> createState() => _ManagerStaffScreenState();
}

class _ManagerStaffScreenState extends State<ManagerStaffScreen> {
  List<SalonModel> _salons = [];
  List<SalonServiceModel> _services = [];
  List<StaffModel> _staff = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    // All three are auto-scoped server-side to salons this manager owns.
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 20});
    final services = await context.read<SalonServiceProvider>().get(filter: {'pageSize': 200});
    final staff = await context.read<StaffProvider>().get(filter: {'pageSize': 200});
    if (!mounted) return;
    setState(() {
      _salons = salons.items;
      _services = services.items;
      _staff = staff.items;
      _isLoading = false;
    });
  }

  Future<void> _openAddForm() async {
    final salonId = _salons.firstOrNull?.id;
    if (salonId == null) {
      showErrorSnackBar(context, 'Set up your salon first from the Salon tab.');
      return;
    }

    final userIdController = TextEditingController();
    final roleController = TextEditingController(text: 'Stylist');
    final selectedServiceIds = <int>{};

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
                  const Text('Add staff member', style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                  const SizedBox(height: 20),
                  TextField(
                    controller: userIdController,
                    decoration: const InputDecoration(labelText: 'User ID (existing account)'),
                    keyboardType: TextInputType.number,
                  ),
                  const SizedBox(height: 12),
                  TextField(controller: roleController, decoration: const InputDecoration(labelText: 'Role / title')),
                  const SizedBox(height: 16),
                  const Align(alignment: Alignment.centerLeft, child: Text('Services performed', style: TextStyle(color: AppColors.textSecondary, fontSize: 13))),
                  const SizedBox(height: 8),
                  Wrap(
                    spacing: 8,
                    runSpacing: 8,
                    children: _services
                        .map((s) => FilterChip(
                              label: Text(s.name),
                              selected: selectedServiceIds.contains(s.id),
                              onSelected: (v) => setSheetState(() => v ? selectedServiceIds.add(s.id) : selectedServiceIds.remove(s.id)),
                            ))
                        .toList(),
                  ),
                  const SizedBox(height: 20),
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
    final userId = int.tryParse(userIdController.text.trim());
    if (userId == null) {
      showErrorSnackBar(context, 'Enter a valid numeric User ID.');
      return;
    }

    try {
      await context.read<StaffProvider>().insert({
        'salonId': salonId,
        'userId': userId,
        'role': roleController.text.trim().isEmpty ? 'Stylist' : roleController.text.trim(),
        'serviceIds': selectedServiceIds.toList(),
      });
      if (mounted) showSuccessSnackBar(context, 'Staff member added.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  Future<void> _toggleActive(StaffModel staff) async {
    try {
      await context.read<StaffProvider>().update(staff.id, {
        'role': staff.role,
        'bio': staff.bio,
        'profileImageUrl': staff.profileImageUrl,
        'isActive': !staff.isActive,
        'serviceIds': staff.serviceIds,
      });
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
        title: const Text('Team'),
        actions: [
          IconButton(icon: const Icon(Icons.add), onPressed: _openAddForm),
        ],
      ),
      body: _isLoading
          ? const LoadingIndicator()
          : _staff.isEmpty
              ? const Center(child: Text('No staff yet. Tap + to add one.', style: TextStyle(color: AppColors.textSecondary)))
              : ListView.separated(
                  padding: const EdgeInsets.all(16),
                  itemCount: _staff.length,
                  separatorBuilder: (_, __) => const SizedBox(height: 12),
                  itemBuilder: (context, index) {
                    final s = _staff[index];
                    return Container(
                      padding: const EdgeInsets.all(14),
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        border: Border.all(color: const Color(0xFFEDEAF4)),
                      ),
                      child: Row(
                        children: [
                          CircleAvatar(
                            radius: 24,
                            backgroundColor: AppColors.primaryLight,
                            backgroundImage: s.profileImageUrl != null ? NetworkImage(s.profileImageUrl!) : null,
                            child: s.profileImageUrl == null
                                ? Text(
                                    (s.fullName?.isNotEmpty == true ? s.fullName![0] : '?').toUpperCase(),
                                    style: const TextStyle(color: AppColors.primaryDark, fontWeight: FontWeight.bold),
                                  )
                                : null,
                          ),
                          const SizedBox(width: 14),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(s.fullName ?? 'Staff #${s.id}', style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                                const SizedBox(height: 2),
                                Text('${s.role} • ${s.serviceIds.length} service(s)', style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                              ],
                            ),
                          ),
                          GestureDetector(
                            onTap: () => _toggleActive(s),
                            child: Container(
                              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
                              decoration: BoxDecoration(
                                color: s.isActive ? AppColors.completedBg : AppColors.neutralBg,
                                borderRadius: BorderRadius.circular(20),
                              ),
                              child: Text(
                                s.isActive ? 'Available' : 'Off',
                                style: TextStyle(
                                  color: s.isActive ? AppColors.completedText : AppColors.neutralText,
                                  fontWeight: FontWeight.bold,
                                  fontSize: 11,
                                ),
                              ),
                            ),
                          ),
                        ],
                      ),
                    );
                  },
                ),
    );
  }
}
