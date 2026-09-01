import 'package:flutter/material.dart';

import '../screens/appointments/my_appointments_screen.dart';
import '../screens/profile/profile_screen.dart';
import '../screens/salons/salon_list_screen.dart';
import '../screens/salons/salon_map_screen.dart';

// 4 tabs per the design reference (Discover / Map / Bookings / Profile) —
// notifications are reached from Profile's "Notifications" row instead of a
// dedicated tab.
class MasterScreen extends StatefulWidget {
  const MasterScreen({super.key});

  @override
  State<MasterScreen> createState() => _MasterScreenState();
}

class _MasterScreenState extends State<MasterScreen> {
  int _index = 0;

  static const _screens = [
    SalonListScreen(),
    SalonMapScreen(),
    MyAppointmentsScreen(),
    ProfileScreen(),
  ];

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: IndexedStack(index: _index, children: _screens),
      bottomNavigationBar: NavigationBar(
        selectedIndex: _index,
        onDestinationSelected: (i) => setState(() => _index = i),
        destinations: const [
          NavigationDestination(icon: Icon(Icons.explore_outlined), selectedIcon: Icon(Icons.explore), label: 'Discover'),
          NavigationDestination(icon: Icon(Icons.map_outlined), selectedIcon: Icon(Icons.map), label: 'Map'),
          NavigationDestination(icon: Icon(Icons.calendar_today_outlined), selectedIcon: Icon(Icons.calendar_today), label: 'Bookings'),
          NavigationDestination(icon: Icon(Icons.person_outline), selectedIcon: Icon(Icons.person), label: 'Profile'),
        ],
      ),
    );
  }
}
