import 'dart:typed_data';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/image_url.dart';
import '../../utils/utils_widgets.dart';
import 'change_password_screen.dart';

class ProfileSettingsScreen extends StatefulWidget {
  const ProfileSettingsScreen({super.key});

  @override
  State<ProfileSettingsScreen> createState() => _ProfileSettingsScreenState();
}

class _ProfileSettingsScreenState extends State<ProfileSettingsScreen> {
  final _firstName = TextEditingController();
  final _lastName = TextEditingController();
  final _phone = TextEditingController();
  Uint8List? _pickedBytes;
  String? _pickedName;
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    final user = AuthProvider.currentUser;
    _firstName.text = user?.firstName ?? '';
    _lastName.text = user?.lastName ?? '';
    _phone.text = user?.phone ?? '';
  }

  @override
  void dispose() {
    _firstName.dispose();
    _lastName.dispose();
    _phone.dispose();
    super.dispose();
  }

  Future<void> _pickImage() async {
    final image = await ImagePicker().pickImage(source: ImageSource.gallery, maxWidth: 1024, imageQuality: 85);
    if (image == null) return;
    final bytes = await image.readAsBytes();
    if (!mounted) return;
    setState(() {
      _pickedBytes = bytes;
      _pickedName = image.name;
    });
  }

  Future<void> _save() async {
    if (_firstName.text.trim().isEmpty || _lastName.text.trim().isEmpty) {
      showErrorSnackBar(context, 'First and last name are required.');
      return;
    }

    setState(() => _isSaving = true);
    try {
      final userId = context.read<AuthProvider>().userId!;
      final userProvider = context.read<UserProvider>();

      // Uploaded first (if a new photo was picked) so the profileImageUrl below
      // is always the current one, whether or not the photo changed this time.
      var current = AuthProvider.currentUser;
      if (_pickedBytes != null) {
        current = await userProvider.uploadAvatar(userId, _pickedBytes!, _pickedName ?? 'avatar.jpg');
      }

      final updated = await userProvider.update(userId, {
        'firstName': _firstName.text.trim(),
        'lastName': _lastName.text.trim(),
        'email': current?.email,
        'phone': _phone.text.trim().isEmpty ? null : _phone.text.trim(),
        'profileImageUrl': current?.profileImageUrl,
        'isActive': current?.isActive ?? true,
      });

      if (!mounted) return;
      context.read<AuthProvider>().updateCurrentUser(updated);
      showSuccessSnackBar(context, 'Profile updated successfully.');
      Navigator.of(context).pop();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final existingUrl = AuthProvider.currentUser?.profileImageUrl;

    return Scaffold(
      appBar: AppBar(title: const Text('Profile settings')),
      body: ListView(
        padding: const EdgeInsets.all(24),
        children: [
          Center(
            child: Stack(
              children: [
                CircleAvatar(
                  radius: 48,
                  backgroundImage: _pickedBytes != null
                      ? MemoryImage(_pickedBytes!)
                      : (existingUrl != null ? NetworkImage(resolveImageUrl(existingUrl)) as ImageProvider : null),
                  child: _pickedBytes == null && existingUrl == null ? const Icon(Icons.person, size: 48) : null,
                ),
                Positioned(
                  bottom: 0,
                  right: 0,
                  child: IconButton.filled(icon: const Icon(Icons.camera_alt, size: 18), onPressed: _isSaving ? null : _pickImage),
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),
          TextField(controller: _firstName, decoration: const InputDecoration(labelText: 'First name', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          TextField(controller: _lastName, decoration: const InputDecoration(labelText: 'Last name', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          TextField(controller: _phone, decoration: const InputDecoration(labelText: 'Phone', border: OutlineInputBorder())),
          const SizedBox(height: 16),
          ListTile(
            contentPadding: EdgeInsets.zero,
            leading: const Icon(Icons.lock_outline),
            title: const Text('Change password'),
            trailing: const Icon(Icons.chevron_right),
            onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ChangePasswordScreen())),
          ),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: FilledButton(
              onPressed: _isSaving ? null : _save,
              child: _isSaving ? const LoadingIndicator() : const Text('Save changes'),
            ),
          ),
        ],
      ),
    );
  }
}
