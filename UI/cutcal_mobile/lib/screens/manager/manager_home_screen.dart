import 'package:flutter/material.dart';

import '../notifications/notifications_screen.dart';
import 'manager_appointments_screen.dart';
import 'manager_dashboard_screen.dart';
import 'manager_profile_screen.dart';

class ManagerHomeScreen extends StatefulWidget {
  const ManagerHomeScreen({super.key});

  @override
  State<ManagerHomeScreen> createState() => _ManagerHomeScreenState();
}

class _ManagerHomeScreenState extends State<ManagerHomeScreen> {
  int _index = 0;

  static const _screens = [
    ManagerDashboardScreen(),
    ManagerAppointmentsScreen(),
    NotificationsScreen(),
    ManagerProfileScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(index: _index, children: _screens),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (i) => setState(() => _index = i),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.storefront), label: 'My Salon'),
          NavigationDestination(icon: Icon(Icons.event_note), label: 'Appointments'),
          NavigationDestination(icon: Icon(Icons.notifications), label: 'Alerts'),
          NavigationDestination(icon: Icon(Icons.person), label: 'Profile'),
        ],
      ),
    );
  }
}
