import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../utils/utils_widgets.dart';
import '../auth/login_screen.dart';

class ManagerProfileScreen extends StatelessWidget {
  const ManagerProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const CircleAvatar(radius: 40, child: Icon(Icons.storefront, size: 40)),
          const SizedBox(height: 16),
          Center(
            child: Text(
              '${AuthProvider.accessTokenDecoded?['FirstName'] ?? ''} ${AuthProvider.accessTokenDecoded?['LastName'] ?? ''}',
              style: Theme.of(context).textTheme.titleMedium,
            ),
          ),
          const Center(child: Text('Salon Manager', style: TextStyle(color: Colors.grey))),
          const SizedBox(height: 24),
          const Divider(),
          ListTile(
            leading: const Icon(Icons.logout, color: Colors.red),
            title: const Text('Log out', style: TextStyle(color: Colors.red)),
            onTap: () async {
              final confirmed = await showConfirmationDialog(
                context,
                title: 'Log out',
                message: 'Are you sure you want to log out?',
              );
              if (!confirmed) return;
              if (!context.mounted) return;
              context.read<AuthProvider>().logout();
              Navigator.of(context).pushAndRemoveUntil(
                MaterialPageRoute(builder: (_) => const LoginScreen()),
                (route) => false,
              );
            },
          ),
        ],
      ),
    );
  }
}
