import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';
import '../auth/login_screen.dart';
import '../notifications/notifications_screen.dart';
import 'favorites_screen.dart';
import 'profile_settings_screen.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  int _visits = 0;
  int _reviews = 0;
  int _saved = 0;
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

    final userId = context.read<AuthProvider>().userId;
    final appointments = context.read<AppointmentProvider>();
    final reviews = context.read<ReviewProvider>();
    final favorites = context.read<FavoriteProvider>();

    try {
      final visitsFuture = appointments.get(filter: {'customerId': userId, 'status': 'Completed', 'pageSize': 1});
      final reviewsFuture = reviews.get(filter: {'customerId': userId, 'pageSize': 1});
      final savedFuture = favorites.getMine();

      final visitsResult = await visitsFuture;
      final reviewsResult = await reviewsFuture;
      final savedResult = await savedFuture;
      if (!mounted) return;
      setState(() {
        _visits = visitsResult.totalCount;
        _reviews = reviewsResult.totalCount;
        _saved = savedResult.length;
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

  String get _initials {
    final claims = AuthProvider.accessTokenDecoded;
    final first = (claims?['FirstName'] as String?)?.trim() ?? '';
    final last = (claims?['LastName'] as String?)?.trim() ?? '';
    final initials = '${first.isNotEmpty ? first[0] : ''}${last.isNotEmpty ? last[0] : ''}';
    return initials.isEmpty ? '?' : initials.toUpperCase();
  }

  String get _fullName {
    final claims = AuthProvider.accessTokenDecoded;
    final first = (claims?['FirstName'] as String?)?.trim() ?? '';
    final last = (claims?['LastName'] as String?)?.trim() ?? '';
    final name = '$first $last'.trim();
    return name.isEmpty ? 'My account' : name;
  }

  String get _email => (AuthProvider.accessTokenDecoded?['Email'] as String?) ?? '';

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

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Profile')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Container(
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              border: Border.all(color: const Color(0xFFEDEAF4)),
            ),
            child: Column(
              children: [
                Row(
                  children: [
                    CircleAvatar(
                      radius: 32,
                      backgroundColor: AppColors.primaryLight,
                      child: Text(_initials, style: const TextStyle(fontSize: 22, fontWeight: FontWeight.bold, color: AppColors.primaryDark)),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(_fullName, style: const TextStyle(fontSize: 17, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                          const SizedBox(height: 2),
                          Text(_email, style: const TextStyle(fontSize: 13, color: AppColors.textSecondary)),
                        ],
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 20),
                if (_isLoading)
                  const Padding(padding: EdgeInsets.symmetric(vertical: 8), child: LoadingIndicator())
                else if (_loadError != null)
                  Column(
                    children: [
                      Text(_loadError!, textAlign: TextAlign.center, style: const TextStyle(color: AppColors.textSecondary)),
                      const SizedBox(height: 8),
                      OutlinedButton(onPressed: _load, child: const Text('Try again')),
                    ],
                  )
                else
                  Row(
                    children: [
                      Expanded(child: _StatColumn(value: _visits, label: 'Visits')),
                      _statDivider(),
                      Expanded(child: _StatColumn(value: _reviews, label: 'Reviews')),
                      _statDivider(),
                      Expanded(child: _StatColumn(value: _saved, label: 'Saved')),
                    ],
                  ),
              ],
            ),
          ),
          const SizedBox(height: 20),
          _SettingsGroup(children: [
            _SettingsTile(
              icon: Icons.settings_outlined,
              label: 'Profile settings',
              onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const ProfileSettingsScreen())),
            ),
            _SettingsTile(
              icon: Icons.favorite_outline,
              label: 'Saved salons',
              onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const FavoritesScreen())),
            ),
            _SettingsTile(
              icon: Icons.notifications_outlined,
              label: 'Notifications',
              onTap: () => Navigator.of(context).push(MaterialPageRoute(builder: (_) => const NotificationsScreen())),
              isLast: true,
            ),
          ]),
          const SizedBox(height: 24),
          SizedBox(
            width: double.infinity,
            child: OutlinedButton(
              onPressed: () => ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Log in with a business manager account to access Business Mode.')),
              ),
              child: const Text('Switch to business mode'),
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

  Widget _statDivider() => Container(width: 1, height: 32, color: const Color(0xFFEDEAF4));
}

class _StatColumn extends StatelessWidget {
  final int value;
  final String label;

  const _StatColumn({required this.value, required this.label});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text('$value', style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
        const SizedBox(height: 2),
        Text(label, style: const TextStyle(fontSize: 12, color: AppColors.textSecondary)),
      ],
    );
  }
}

class _SettingsGroup extends StatelessWidget {
  final List<Widget> children;

  const _SettingsGroup({required this.children});

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(18),
        border: Border.all(color: const Color(0xFFEDEAF4)),
      ),
      child: Column(children: children),
    );
  }
}

class _SettingsTile extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;
  final bool isLast;

  const _SettingsTile({required this.icon, required this.label, required this.onTap, this.isLast = false});

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(18),
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: isLast ? null : const BoxDecoration(border: Border(bottom: BorderSide(color: Color(0xFFEDEAF4)))),
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
