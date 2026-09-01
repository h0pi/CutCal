import 'package:flutter/material.dart';

/// Brand palette, matched to the Claude Design reference: a white app with a
/// vivid violet as the single CTA/brand color, plus semantic status colors
/// for booking states (pending/confirmed/completed/declined/paid).
class AppColors {
  static const primary = Color(0xFF7C3AED); // vivid violet — CTAs, selected states, links
  static const primaryDark = Color(0xFF5B21B6); // deeper violet — pressed/emphasis text
  static const primaryLight = Color(0xFFEDE9FE); // pale lavender — chip bg, selection tint, unfilled tracks
  static const primaryDisabled = Color(0xFFC4B5FD); // light violet — disabled CTA fill

  static const background = Color(0xFFF7F6FA);
  static const cardBackground = Colors.white;
  static const surfaceMuted = Color(0xFFF4F2FA); // input fill, subtle section backgrounds

  static const textPrimary = Color(0xFF1F2024);
  static const textSecondary = Color(0xFF6B7280);

  static const ratingBadge = Color(0xFF14141F); // near-black "★ 4.8" pill over photos
  static const starColor = Color(0xFFF59E0B); // amber review stars

  // Business Mode dashboard hero card.
  static const businessHero = Color(0xFF14141F);

  // Status colors: (text, background) pairs used by StatusPill.
  static const pendingText = Color(0xFF92400E);
  static const pendingBg = Color(0xFFFEF3C7);
  static const confirmedText = Color(0xFF1D4ED8);
  static const confirmedBg = Color(0xFFDBEAFE);
  static const completedText = Color(0xFF15803D);
  static const completedBg = Color(0xFFDCFCE7);
  static const declinedText = Color(0xFFB91C1C);
  static const declinedBg = Color(0xFFFEE2E2);
  static const paidText = Color(0xFF15803D);
  static const paidBg = Color(0xFFDCFCE7);
  static const neutralText = Color(0xFF4B5563);
  static const neutralBg = Color(0xFFF3F4F6);
}

ThemeData buildAppTheme() {
  final colorScheme = ColorScheme.fromSeed(
    seedColor: AppColors.primary,
    brightness: Brightness.light,
  ).copyWith(
    primary: AppColors.primary,
    secondary: AppColors.primary,
    surface: AppColors.cardBackground,
    onSurface: AppColors.textPrimary,
    onPrimary: Colors.white,
  );

  return ThemeData(
    colorScheme: colorScheme,
    useMaterial3: true,
    brightness: Brightness.light,
    scaffoldBackgroundColor: AppColors.background,
    dividerColor: const Color(0xFFEAE6F2),
    appBarTheme: const AppBarTheme(
      backgroundColor: AppColors.background,
      foregroundColor: AppColors.textPrimary,
      elevation: 0,
      centerTitle: false,
      surfaceTintColor: Colors.transparent,
      iconTheme: IconThemeData(color: AppColors.textPrimary),
      titleTextStyle: TextStyle(color: AppColors.textPrimary, fontSize: 20, fontWeight: FontWeight.bold),
    ),
    navigationBarTheme: NavigationBarThemeData(
      backgroundColor: Colors.white,
      indicatorColor: Colors.transparent,
      elevation: 3,
      shadowColor: Colors.black12,
      labelTextStyle: WidgetStateProperty.resolveWith((states) => TextStyle(
            fontSize: 11,
            fontWeight: states.contains(WidgetState.selected) ? FontWeight.w700 : FontWeight.w500,
            color: states.contains(WidgetState.selected) ? AppColors.primary : AppColors.textSecondary,
          )),
      iconTheme: WidgetStateProperty.resolveWith((states) => IconThemeData(
            color: states.contains(WidgetState.selected) ? AppColors.primary : AppColors.textSecondary,
          )),
    ),
    cardTheme: CardThemeData(
      color: AppColors.cardBackground,
      elevation: 1,
      shadowColor: Colors.black.withValues(alpha: 0.08),
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(18)),
    ),
    filledButtonTheme: FilledButtonThemeData(
      style: FilledButton.styleFrom(
        backgroundColor: AppColors.primary,
        foregroundColor: Colors.white,
        disabledBackgroundColor: AppColors.primaryDisabled,
        disabledForegroundColor: Colors.white,
        minimumSize: const Size.fromHeight(52),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(26)),
        textStyle: const TextStyle(fontWeight: FontWeight.bold, fontSize: 15),
      ),
    ),
    outlinedButtonTheme: OutlinedButtonThemeData(
      style: OutlinedButton.styleFrom(
        foregroundColor: AppColors.primary,
        side: const BorderSide(color: AppColors.primary),
        minimumSize: const Size.fromHeight(52),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(26)),
      ),
    ),
    textButtonTheme: TextButtonThemeData(style: TextButton.styleFrom(foregroundColor: AppColors.primary)),
    iconTheme: const IconThemeData(color: AppColors.textSecondary),
    inputDecorationTheme: InputDecorationTheme(
      filled: true,
      fillColor: Colors.white,
      hintStyle: const TextStyle(color: AppColors.textSecondary),
      labelStyle: const TextStyle(color: AppColors.textSecondary),
      prefixIconColor: AppColors.textSecondary,
      suffixIconColor: AppColors.textSecondary,
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(28), borderSide: BorderSide.none),
      enabledBorder: OutlineInputBorder(borderRadius: BorderRadius.circular(28), borderSide: BorderSide.none),
      focusedBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(28),
        borderSide: const BorderSide(color: AppColors.primary, width: 1.5),
      ),
    ),
    chipTheme: ChipThemeData(
      selectedColor: AppColors.primary,
      backgroundColor: Colors.white,
      labelStyle: const TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.w600),
      secondaryLabelStyle: const TextStyle(color: Colors.white, fontWeight: FontWeight.w700),
      side: const BorderSide(color: Color(0xFFE5E1EF)),
      shape: const StadiumBorder(),
    ),
    textTheme: ThemeData(brightness: Brightness.light).textTheme.apply(
          bodyColor: AppColors.textPrimary,
          displayColor: AppColors.textPrimary,
        ),
    listTileTheme: const ListTileThemeData(iconColor: AppColors.textSecondary, textColor: AppColors.textPrimary),
    dialogTheme: DialogThemeData(
      backgroundColor: Colors.white,
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
    ),
    snackBarTheme: const SnackBarThemeData(backgroundColor: AppColors.textPrimary, contentTextStyle: TextStyle(color: Colors.white)),
  );
}
