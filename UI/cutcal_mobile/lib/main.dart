import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'layouts/master_screen.dart';
import 'providers/auth_provider.dart';
import 'providers/entity_providers.dart';
import 'screens/auth/login_screen.dart';
import 'screens/manager/manager_home_screen.dart';
import 'utils/app_theme.dart';

void main() {
  runApp(const CutCalApp());
}

class CutCalApp extends StatelessWidget {
  const CutCalApp({super.key});

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
        ChangeNotifierProvider(create: (_) => RecommendationProvider()),
        ChangeNotifierProvider(create: (_) => PaymentProvider()),
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(create: (_) => FavoriteProvider()),
      ],
      child: MaterialApp(
        title: 'CutCal',
        debugShowCheckedModeBanner: false,
        theme: buildAppTheme(),
        home: Consumer<AuthProvider>(
          builder: (context, auth, _) {
            if (!auth.isLoggedIn) return const LoginScreen();
            if (auth.role == 'SalonManager') return const ManagerHomeScreen();
            return const MasterScreen();
          },
        ),
      ),
    );
  }
}
