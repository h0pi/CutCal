import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

class UsersScreen extends StatefulWidget {
  const UsersScreen({super.key});

  @override
  State<UsersScreen> createState() => _UsersScreenState();
}

class _UsersScreenState extends State<UsersScreen> {
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  String? _role;
  List<UserModel> _users = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<UserProvider>().get(filter: {
      'name': _nameController.text.isEmpty ? null : _nameController.text,
      'email': _emailController.text.isEmpty ? null : _emailController.text,
      'pageSize': 100,
    });
    var items = result.items;
    if (_role != null) {
      items = items.where((u) => u.roles.contains(_role)).toList();
    }
    setState(() {
      _users = items;
      _isLoading = false;
    });
  }

  Future<void> _openAddForm() async {
    final usernameController = TextEditingController();
    final firstNameController = TextEditingController();
    final lastNameController = TextEditingController();
    final emailController = TextEditingController();
    final passwordController = TextEditingController();
    String role = 'Customer';

    final result = await showDialog<bool>(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: const Text('Add user'),
          content: SizedBox(
            width: 400,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextField(controller: usernameController, decoration: const InputDecoration(labelText: 'Username')),
                TextField(controller: firstNameController, decoration: const InputDecoration(labelText: 'First name')),
                TextField(controller: lastNameController, decoration: const InputDecoration(labelText: 'Last name')),
                TextField(controller: emailController, decoration: const InputDecoration(labelText: 'Email')),
                TextField(controller: passwordController, obscureText: true, decoration: const InputDecoration(labelText: 'Password')),
                DropdownButtonFormField<String>(
                  initialValue: role,
                  decoration: const InputDecoration(labelText: 'Role'),
                  items: const [
                    DropdownMenuItem(value: 'Customer', child: Text('Customer')),
                    DropdownMenuItem(value: 'Staff', child: Text('Staff')),
                    DropdownMenuItem(value: 'SalonManager', child: Text('Salon Manager')),
                    DropdownMenuItem(value: 'Admin', child: Text('Admin')),
                  ],
                  onChanged: (v) => setDialogState(() => role = v ?? 'Customer'),
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

    if (result != true) return;

    await context.read<UserProvider>().insert({
      'username': usernameController.text,
      'firstName': firstNameController.text,
      'lastName': lastNameController.text,
      'email': emailController.text,
      'password': passwordController.text,
      'role': role,
    });
    if (mounted) showSuccessSnackBar(context, 'User created.');
    _load();
  }

  Future<void> _toggleActive(UserModel user) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: user.isActive ? 'Deactivate user' : 'Activate user',
      message: user.isActive
          ? 'Deactivate ${user.firstName} ${user.lastName}? They will no longer be able to log in.'
          : 'Reactivate ${user.firstName} ${user.lastName}?',
      isDestructive: user.isActive,
    );
    if (!confirmed) return;

    await context.read<UserProvider>().update(user.id, {
      'firstName': user.firstName,
      'lastName': user.lastName,
      'email': user.email,
      'phone': user.phone,
      'profileImageUrl': user.profileImageUrl,
      'isActive': !user.isActive,
    });
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
            children: [
              SizedBox(
                width: 200,
                child: TextField(
                  controller: _nameController,
                  decoration: const InputDecoration(labelText: 'Name', border: OutlineInputBorder(), isDense: true),
                  onSubmitted: (_) => _load(),
                ),
              ),
              SizedBox(
                width: 200,
                child: TextField(
                  controller: _emailController,
                  decoration: const InputDecoration(labelText: 'Email', border: OutlineInputBorder(), isDense: true),
                  onSubmitted: (_) => _load(),
                ),
              ),
              SizedBox(
                width: 180,
                child: DropdownButtonFormField<String?>(
                  initialValue: _role,
                  decoration: const InputDecoration(labelText: 'Role', border: OutlineInputBorder(), isDense: true),
                  items: const [
                    DropdownMenuItem(value: null, child: Text('All')),
                    DropdownMenuItem(value: 'Customer', child: Text('Customer')),
                    DropdownMenuItem(value: 'Staff', child: Text('Staff')),
                    DropdownMenuItem(value: 'SalonManager', child: Text('Salon Manager')),
                    DropdownMenuItem(value: 'Admin', child: Text('Admin')),
                  ],
                  onChanged: (v) {
                    setState(() => _role = v);
                    _load();
                  },
                ),
              ),
              FilledButton.icon(icon: const Icon(Icons.search), label: const Text('Search'), onPressed: _load),
              FilledButton.icon(icon: const Icon(Icons.person_add), label: const Text('Add user'), onPressed: _openAddForm),
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
                        DataColumn(label: Text('Username')),
                        DataColumn(label: Text('Email')),
                        DataColumn(label: Text('Roles')),
                        DataColumn(label: Text('Active')),
                      ],
                      rows: _users
                          .map((u) => DataRow(cells: [
                                DataCell(Text('${u.firstName} ${u.lastName}')),
                                DataCell(Text(u.username)),
                                DataCell(Text(u.email)),
                                DataCell(Text(u.roles.join(', '))),
                                DataCell(Switch(value: u.isActive, onChanged: (_) => _toggleActive(u))),
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
