import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'layouts/master_screen.dart';
import 'providers/auth_provider.dart';
import 'providers/entity_providers.dart';
import 'screens/auth/login_screen.dart';

void main() {
  runApp(const CutCalDesktopApp());
}

class CutCalDesktopApp extends StatelessWidget {
  const CutCalDesktopApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => AuthProvider()),
        ChangeNotifierProvider(create: (_) => SalonProvider()),
        ChangeNotifierProvider(create: (_) => SalonCategoryProvider()),
        ChangeNotifierProvider(create: (_) => CityProvider()),
        ChangeNotifierProvider(create: (_) => SalonServiceProvider()),
        ChangeNotifierProvider(create: (_) => StaffProvider()),
        ChangeNotifierProvider(create: (_) => AppointmentProvider()),
        ChangeNotifierProvider(create: (_) => ReviewProvider()),
        ChangeNotifierProvider(create: (_) => NotificationProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        Provider(create: (_) => ReportProvider()),
      ],
      child: MaterialApp(
        title: 'CutCal Admin',
        debugShowCheckedModeBanner: false,
        theme: ThemeData(
          colorSchemeSeed: Colors.deepPurple,
          useMaterial3: true,
        ),
        home: Consumer<AuthProvider>(
          builder: (context, auth, _) {
            return auth.isLoggedIn ? const MasterScreen() : const LoginScreen();
          },
        ),
      ),
    );
  }
}
