import 'package:flutter/material.dart';

/// Brand palette (per design reference): deep teal/navy as the dominant
/// background color everywhere, warm gold/tan as the accent used for
/// ratings, highlights, primary actions and card glow shadows.
class AppColors {
  static const primary = Color(0xFF254252); // Blue Dianne — main background color, everywhere
  static const cardBackground = Color(0xFF32586C); // lighter tint of primary, for cards/inputs/sheets
  static const accent = Color(0xFFEAB56F); // Porsche — secondary/accent color
  static const background = primary;
}

ThemeData buildAppTheme() {
  final colorScheme = ColorScheme.fromSeed(
    seedColor: AppColors.primary,
    brightness: Brightness.dark,
  ).copyWith(
    primary: AppColors.accent,
    secondary: AppColors.accent,
    surface: AppColors.cardBackground,
    onSurface: Colors.white,
    onPrimary: AppColors.primary,
  );

  return ThemeData(
    colorScheme: colorScheme,
    useMaterial3: true,
    brightness: Brightness.dark,
    scaffoldBackgroundColor: AppColors.background,
    dividerColor: Colors.white24,
    appBarTheme: const AppBarTheme(
      backgroundColor: AppColors.primary,
      foregroundColor: Colors.white,
      elevation: 0,
      centerTitle: false,
      surfaceTintColor: Colors.transparent,
    ),
    navigationBarTheme: NavigationBarThemeData(
      backgroundColor: AppColors.primary,
      indicatorColor: AppColors.accent,
      labelTextStyle: WidgetStateProperty.resolveWith((states) => TextStyle(
            fontSize: 12,
            fontWeight: states.contains(WidgetState.selected) ? FontWeight.w700 : FontWeight.w500,
            color: states.contains(WidgetState.selected) ? AppColors.primary : Colors.white70,
          )),
      iconTheme: WidgetStateProperty.resolveWith((states) => IconThemeData(
            color: states.contains(WidgetState.selected) ? AppColors.primary : Colors.white70,
          )),
    ),
    cardTheme: CardThemeData(
      color: AppColors.cardBackground,
      elevation: 8,
      shadowColor: AppColors.accent.withValues(alpha: 0.45),
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
    ),
    filledButtonTheme: FilledButtonThemeData(
      style: FilledButton.styleFrom(backgroundColor: AppColors.accent, foregroundColor: AppColors.primary),
    ),
    outlinedButtonTheme: OutlinedButtonThemeData(
      style: OutlinedButton.styleFrom(foregroundColor: AppColors.accent, side: const BorderSide(color: AppColors.accent)),
    ),
    textButtonTheme: TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: AppColors.accent)),
    iconTheme: const IconThemeData(color: Colors.white70),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: AppColors.cardBackground,
      hintStyle: const TextStyle(color: Colors.white54),
      labelStyle: const TextStyle(color: Colors.white70),
      prefixIconColor: Colors.white70,
      suffixIconColor: Colors.white70,
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
      enabledBorder: OutlineInputBorder(borderRadius: BorderRadius.circular(14), borderSide: BorderSide.none),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(14),
        borderSide: const BorderSide(color: AppColors.accent, width: 1.5),
      ),
    ),
    chipTheme: ChipThemeData(
      selectedColor: AppColors.accent,
      backgroundColor: AppColors.cardBackground,
      labelStyle: const TextStyle(color: Colors.white70, fontWeight: FontWeight.w600),
      secondaryLabelStyle: const TextStyle(color: AppColors.primary, fontWeight: FontWeight.w700),
      side: BorderSide.none,
    ),
    textTheme: ThemeData(brightness: Brightness.dark).textTheme.apply(
          bodyColor: Colors.white,
          displayColor: Colors.white,
        ),
    listTileTheme: const ListTileThemeData(iconColor: Colors.white70, textColor: Colors.white),
    dialogTheme: DialogThemeData(
      backgroundColor: AppColors.cardBackground,
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    ),
    snackBarTheme: const SnackBarThemeData(backgroundColor: AppColors.cardBackground, contentTextStyle: TextStyle(color: Colors.white)),
  );
}
