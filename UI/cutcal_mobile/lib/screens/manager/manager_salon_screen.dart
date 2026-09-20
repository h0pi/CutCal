import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import '../auth/login_screen.dart';
import 'manager_reviews_screen.dart';
import '../../utils/image_url.dart';

const _dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

class ManagerSalonScreen extends StatefulWidget {
  const ManagerSalonScreen({super.key});

  @override
  State<ManagerSalonScreen> createState() => _ManagerSalonScreenState();
}

class _ManagerSalonScreenState extends State<ManagerSalonScreen> {
  SalonModel? _salon;
  bool _isLoading = true;
  bool _isSaving = false;
  XFile? _pickedCover;

  final _nameController = TextEditingController();
  final _bioController = TextEditingController();
  final _addressController = TextEditingController();
  final _phoneController = TextEditingController();

  late List<SalonWorkingHoursModel> _hours;

  @override
  void initState() {
    super.initState();
    _hours = List.generate(7, (i) => SalonWorkingHoursModel(dayOfWeek: i, openTime: '09:00:00', closeTime: '18:00:00', isClosed: i == 0));
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 20});
    final salon = salons.items.firstOrNull;
    if (!mounted) return;
    setState(() {
      _salon = salon;
      if (salon != null) {
        _nameController.text = salon.name;
        _bioController.text = salon.description ?? '';
        _addressController.text = salon.address;
        _phoneController.text = salon.phone ?? '';
        if (salon.workingHours.isNotEmpty) {
          for (final wh in salon.workingHours) {
            if (wh.dayOfWeek >= 0 && wh.dayOfWeek < 7) _hours[wh.dayOfWeek] = wh;
          }
        }
      }
      _isLoading = false;
    });
  }

  Future<void> _pickCover() async {
    final picker = ImagePicker();
    final image = await picker.pickImage(source: ImageSource.gallery);
    if (image != null) setState(() => _pickedCover = image);
  }

  Future<void> _editDayHours(int dayIndex) async {
    var hours = _hours[dayIndex];
    final result = await showModalBottomSheet<SalonWorkingHoursModel>(
      context: context,
      backgroundColor: Colors.white,
      shape: const RoundedRectangleBorder(borderRadius: BorderRadius.vertical(top: Radius.circular(20))),
      builder: (context) => StatefulBuilder(
        builder: (context, setSheetState) => Padding(
          padding: const EdgeInsets.fromLTRB(20, 12, 20, 20),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Center(
                child: Container(width: 40, height: 4, decoration: BoxDecoration(color: Colors.grey.shade300, borderRadius: BorderRadius.circular(2))),
              ),
              const SizedBox(height: 16),
              Text(_dayNames[dayIndex], style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
              const SizedBox(height: 12),
              SwitchListTile(
                contentPadding: EdgeInsets.zero,
                value: !hours.isClosed,
                title: const Text('Open on this day', style: TextStyle(color: AppColors.textPrimary)),
                onChanged: (v) => setSheetState(() => hours = SalonWorkingHoursModel(dayOfWeek: dayIndex, openTime: hours.openTime, closeTime: hours.closeTime, isClosed: !v)),
              ),
              if (!hours.isClosed) ...[
                const SizedBox(height: 8),
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton(
                        onPressed: () async {
                          final picked = await showTimePicker(context: context, initialTime: _parseTime(hours.openTime) ?? const TimeOfDay(hour: 9, minute: 0));
                          if (picked != null) {
                            setSheetState(() => hours = SalonWorkingHoursModel(dayOfWeek: dayIndex, openTime: _formatTime(picked), closeTime: hours.closeTime, isClosed: hours.isClosed));
                          }
                        },
                        child: Text('Opens ${_displayTime(hours.openTime) ?? '--:--'}'),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: OutlinedButton(
                        onPressed: () async {
                          final picked = await showTimePicker(context: context, initialTime: _parseTime(hours.closeTime) ?? const TimeOfDay(hour: 18, minute: 0));
                          if (picked != null) {
                            setSheetState(() => hours = SalonWorkingHoursModel(dayOfWeek: dayIndex, openTime: hours.openTime, closeTime: _formatTime(picked), isClosed: hours.isClosed));
                          }
                        },
                        child: Text('Closes ${_displayTime(hours.closeTime) ?? '--:--'}'),
                      ),
                    ),
                  ],
                ),
              ],
              const SizedBox(height: 20),
              SizedBox(
                width: double.infinity,
                child: FilledButton(onPressed: () => Navigator.of(context).pop(hours), child: const Text('Done')),
              ),
            ],
          ),
        ),
      ),
    );
    if (result != null) setState(() => _hours[dayIndex] = result);
  }

  TimeOfDay? _parseTime(String? value) {
    if (value == null) return null;
    final parts = value.split(':');
    if (parts.length < 2) return null;
    return TimeOfDay(hour: int.tryParse(parts[0]) ?? 0, minute: int.tryParse(parts[1]) ?? 0);
  }

  String _formatTime(TimeOfDay t) => '${t.hour.toString().padLeft(2, '0')}:${t.minute.toString().padLeft(2, '0')}:00';

  String? _displayTime(String? value) {
    final t = _parseTime(value);
    if (t == null) return null;
    return '${t.hour.toString().padLeft(2, '0')}:${t.minute.toString().padLeft(2, '0')}';
  }

  Future<void> _save() async {
    if (_salon == null) return;
    setState(() => _isSaving = true);
    try {
      await context.read<SalonProvider>().update(_salon!.id, {
        'name': _nameController.text.trim(),
        'salonCategoryId': _salon!.salonCategoryId,
        'description': _bioController.text.trim(),
        'address': _addressController.text.trim(),
        'cityId': _salon!.cityId,
        'latitude': _salon!.latitude,
        'longitude': _salon!.longitude,
        'phone': _phoneController.text.trim(),
        'email': _salon!.email,
        'profileImageUrl': _salon!.profileImageUrl,
        'autoConfirm': _salon!.autoConfirm,
        'workingHours': _hours.map((h) => h.toJson()).toList(),
      });
      if (mounted) showSuccessSnackBar(context, 'Salon profile updated.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Salon')),
      body: _isLoading
          ? const LoadingIndicator()
          : _salon == null
              ? const Center(child: Text('No salon assigned to your account yet.', style: TextStyle(color: AppColors.textSecondary)))
              : ListView(
                  padding: const EdgeInsets.all(16),
                  children: [
                    GestureDetector(
                      onTap: _pickCover,
                      child: Container(
                        height: 140,
                        width: double.infinity,
                        decoration: BoxDecoration(
                          color: AppColors.primaryLight,
                          borderRadius: BorderRadius.circular(18),
                          image: _pickedCover != null
                              ? DecorationImage(image: NetworkImage(_pickedCover!.path), fit: BoxFit.cover)
                              : _salon!.profileImageUrl != null
                                  ? DecorationImage(image: NetworkImage(resolveImageUrl(_salon!.profileImageUrl!)), fit: BoxFit.cover)
                                  : null,
                        ),
                        child: (_pickedCover == null && _salon!.profileImageUrl == null)
                            ? const Center(
                                child: Column(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    Icon(Icons.add_a_photo_outlined, color: AppColors.primary, size: 28),
                                    SizedBox(height: 6),
                                    Text('Add cover photo', style: TextStyle(color: AppColors.primaryDark, fontWeight: FontWeight.w600)),
                                  ],
                                ),
                              )
                            : Align(
                                alignment: Alignment.bottomRight,
                                child: Padding(
                                  padding: const EdgeInsets.all(8),
                                  child: CircleAvatar(backgroundColor: Colors.black45, radius: 16, child: const Icon(Icons.edit, color: Colors.white, size: 16)),
                                ),
                              ),
                      ),
                    ),
                    const SizedBox(height: 20),
                    TextField(controller: _nameController, decoration: const InputDecoration(labelText: 'Salon name')),
                    const SizedBox(height: 12),
                    TextField(controller: _bioController, decoration: const InputDecoration(labelText: 'Bio'), maxLines: 3),
                    const SizedBox(height: 12),
                    TextField(controller: _addressController, decoration: const InputDecoration(labelText: 'Address')),
                    const SizedBox(height: 12),
                    TextField(controller: _phoneController, decoration: const InputDecoration(labelText: 'Phone')),
                    const SizedBox(height: 24),
                    const Text('Operating Hours', style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary, fontSize: 15)),
                    const SizedBox(height: 8),
                    Container(
                      decoration: BoxDecoration(
                        color: Colors.white,
                        borderRadius: BorderRadius.circular(16),
                        border: Border.all(color: const Color(0xFFEDEAF4)),
                      ),
                      child: Column(
                        children: List.generate(7, (i) {
                          final h = _hours[i];
                          final isLast = i == 6;
                          return InkWell(
                            onTap: () => _editDayHours(i),
                            child: Container(
                              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                              decoration: isLast ? null : const BoxDecoration(border: Border(bottom: BorderSide(color: Color(0xFFEDEAF4)))),
                              child: Row(
                                children: [
                                  Expanded(child: Text(_dayNames[i], style: const TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.w500))),
                                  Text(
                                    h.isClosed ? 'Closed' : '${_displayTime(h.openTime) ?? '--:--'} - ${_displayTime(h.closeTime) ?? '--:--'}',
                                    style: TextStyle(color: h.isClosed ? AppColors.textSecondary : AppColors.textPrimary, fontSize: 13),
                                  ),
                                  const SizedBox(width: 6),
                                  const Icon(Icons.chevron_right, size: 18, color: AppColors.textSecondary),
                                ],
                              ),
                            ),
                          );
                        }),
                      ),
                    ),
                    const SizedBox(height: 24),
                    _SettingsLikeTile(
                      icon: Icons.star_outline,
                      label: 'Reviews',
                      onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => ManagerReviewsScreen(salonId: _salon!.id))),
                    ),
                    const SizedBox(height: 24),
                    SizedBox(
                      width: double.infinity,
                      child: FilledButton(
                        onPressed: _isSaving ? null : _save,
                        child: _isSaving
                            ? const SizedBox(height: 20, width: 20, child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white))
                            : const Text('Save changes'),
                      ),
                    ),
                    const SizedBox(height: 12),
                    SizedBox(
                      width: double.infinity,
                      child: OutlinedButton(
                        style: OutlinedButton.styleFrom(foregroundColor: AppColors.declinedText, side: const BorderSide(color: Color(0xFFFCA5A5))),
                        onPressed: () => _logout(context),
                        child: const Text('Log out'),
                      ),
                    ),
                  ],
                ),
    );
  }

  Future<void> _logout(BuildContext context) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Log out',
      message: 'Are you sure you want to log out?',
      confirmLabel: 'Log out',
    );
    if (!confirmed) return;
    if (!context.mounted) return;
    context.read<AuthProvider>().logout();
    Navigator.of(context).pushAndRemoveUntil(
      MaterialPageRoute(builder: (_) => const LoginScreen()),
      (route) => false,
    );
  }
}

class _SettingsLikeTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;

  const _SettingsLikeTile({required this.icon, required this.label, required this.onTap});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: const Color(0xFFEDEAF4)),
        ),
        child: Row(
          children: [
            Icon(icon, size: 20, color: AppColors.primary),
            const SizedBox(width: 14),
            Expanded(child: Text(label, style: const TextStyle(fontSize: 14, color: AppColors.textPrimary, fontWeight: FontWeight.w500))),
            const Icon(Icons.chevron_right, size: 20, color: AppColors.textSecondary),
          ],
        ),
      ),
    );
  }
}
