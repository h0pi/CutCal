import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

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
  XFile? _pickedImage;
  bool _changePassword = false;

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final image = await picker.pickImage(source: ImageSource.gallery);
    if (image != null) setState(() => _pickedImage = image);
  }

  void _save() {
    // TODO: call UserProvider.update(userId, ...) with the edited fields and
    // upload _pickedImage to storage, then set the returned URL as profileImageUrl.
    showSuccessSnackBar(context, 'Profile updated successfully.');
    Navigator.of(context).pop();
  }

  @override
  Widget build(BuildContext context) {
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
                  backgroundImage: _pickedImage != null ? NetworkImage(_pickedImage!.path) : null,
                  child: _pickedImage == null ? const Icon(Icons.person, size: 48) : null,
                ),
                Positioned(
                  bottom: 0,
                  right: 0,
                  child: IconButton.filled(icon: const Icon(Icons.camera_alt, size: 18), onPressed: _pickImage),
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
          CheckboxListTile(
            value: _changePassword,
            title: const Text('Change password'),
            onChanged: (v) {
              setState(() => _changePassword = v ?? false);
              if (_changePassword) {
                Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ChangePasswordScreen()));
              }
            },
          ),
          const SizedBox(height: 16),
          SizedBox(
            width: double.infinity,
            child: FilledButton(onPressed: _save, child: const Text('Save changes')),
          ),
        ],
      ),
    );
  }
}
