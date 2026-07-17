import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../providers/auth_provider.dart';
import '../screens/appointments/appointments_screen.dart';
import '../screens/cities_screen.dart';
import '../screens/dashboard/dashboard_screen.dart';
import '../screens/reports_screen.dart';
import '../screens/reviews_screen.dart';
import '../screens/salon_categories_screen.dart';
import '../screens/salon_profile_screen.dart';
import '../screens/services_screen.dart';
import '../screens/settings_screen.dart';
import '../screens/staff_screen.dart';
import '../screens/users_screen.dart';
import '../utils/utils_widgets.dart';

class MasterScreen extends StatefulWidget {
  const MasterScreen({super.key});

  @override
  State<MasterScreen> createState() => _MasterScreenState();
}

class _NavItem {
  final String label;
  final IconData icon;
  final Widget screen;

  const _NavItem(this.label, this.icon, this.screen);
}

class _MasterScreenState extends State<MasterScreen> {
  int _index = 0;
  bool _expanded = false;

  static const double _collapsedWidth = 72;
  static const double _expandedWidth = 240;

  final List<_NavItem> _items = const [
    _NavItem('Dashboard', Icons.dashboard, DashboardScreen()),
    _NavItem('Appointments', Icons.event_note, AppointmentsScreen()),
    _NavItem('Services', Icons.design_services, ServicesScreen()),
    _NavItem('Staff', Icons.groups, StaffScreen()),
    _NavItem('Reviews', Icons.reviews, ReviewsScreen()),
    _NavItem('Users', Icons.people, UsersScreen()),
    _NavItem('Reports', Icons.bar_chart, ReportsScreen()),
    _NavItem('Salon Profile', Icons.storefront, SalonProfileScreen()),
    _NavItem('Salon Categories', Icons.category, SalonCategoriesScreen()),
    _NavItem('Cities', Icons.location_city, CitiesScreen()),
    _NavItem('Settings', Icons.settings, SettingsScreen()),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: false,
        titleSpacing: 16,
        title: Row(
          children: [
            const Icon(Icons.cut, color: Colors.deepPurple),
            const SizedBox(width: 10),
            const Text('CutCal Admin', style: TextStyle(fontWeight: FontWeight.bold)),
            const SizedBox(width: 20),
            Container(width: 1, height: 20, color: Theme.of(context).dividerColor),
            const SizedBox(width: 20),
            Text(_items[_index].label, style: const TextStyle(fontWeight: FontWeight.normal)),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout),
            tooltip: 'Log out',
            onPressed: () async {
              final confirmed = await showConfirmationDialog(
                context,
                title: 'Log out',
                message: 'Are you sure you want to log out?',
              );
              if (confirmed && context.mounted) {
                context.read<AuthProvider>().logout();
              }
            },
          ),
        ],
      ),
      body: Stack(
        children: [
          // Static layout: the reserved gap for the rail never changes size, so
          // hovering never triggers a relayout/repaint of the screen content below.
          Row(
            children: [
              const SizedBox(width: _collapsedWidth),
              const VerticalDivider(width: 1),
              Expanded(
                child: IndexedStack(
                  index: _index,
                  children: _items.map((e) => e.screen).toList(),
                ),
              ),
            ],
          ),
          // The rail itself floats on top and is the only thing that animates,
          // so expanding it is cheap regardless of how heavy the current screen is.
          _buildSidebar(),
        ],
      ),
    );
  }

  Widget _buildSidebar() {
    return AnimatedPositioned(
      duration: const Duration(milliseconds: 120),
      curve: Curves.easeOut,
      left: 0,
      top: 0,
      bottom: 0,
      width: _expanded ? _expandedWidth : _collapsedWidth,
      child: MouseRegion(
        onEnter: (_) => setState(() => _expanded = true),
        onExit: (_) => setState(() => _expanded = false),
        child: Material(
          elevation: _expanded ? 6 : 0,
          color: Theme.of(context).colorScheme.surfaceContainerLow,
          child: ListView(
            padding: const EdgeInsets.symmetric(vertical: 8),
            children: [
              for (var i = 0; i < _items.length; i++) _buildNavTile(i),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildNavTile(int i) {
    final item = _items[i];
    final selected = i == _index;
    final colorScheme = Theme.of(context).colorScheme;

    final tile = Padding(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
      child: Material(
        color: selected ? colorScheme.secondaryContainer : Colors.transparent,
        borderRadius: BorderRadius.circular(24),
        child: InkWell(
          borderRadius: BorderRadius.circular(24),
          onTap: () => setState(() => _index = i),
          child: SizedBox(
            height: 48,
            child: Row(
              children: [
                const SizedBox(width: 16),
                Icon(
                  item.icon,
                  color: selected ? colorScheme.onSecondaryContainer : colorScheme.onSurfaceVariant,
                ),
                if (_expanded) ...[
                  const SizedBox(width: 12),
                  Expanded(
                    child: Text(
                      item.label,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(
                        color: selected ? colorScheme.onSecondaryContainer : colorScheme.onSurfaceVariant,
                        fontWeight: selected ? FontWeight.w600 : FontWeight.normal,
                      ),
                    ),
                  ),
                ],
              ],
            ),
          ),
        ),
      ),
    );

    return Tooltip(
      message: item.label,
      waitDuration: const Duration(milliseconds: 400),
      child: tile,
    );
  }
}
