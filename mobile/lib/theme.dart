import 'package:flutter/material.dart';

// ═══════════════════════════════════════════════════════
//  1. STATIC COLORS (Used in every screen — KEEP THESE)
// ═══════════════════════════════════════════════════════
class AppColors {
  static const primary = Color(0xFF2196F3);
  static const primaryDark = Color(0xFF1A6FB5);
  static const bg = Color(0xFFF4F8FC);
  static const green = Color(0xFF34A853);
  static const red = Color(0xFFE53935);
  static const yellow = Color(0xFFF0B400);
  static const pink = Color(0xFFE94B6A);
  static const mutedText = Color(0xFF94A3B8);
}

// ═══════════════════════════════════════════════════════
//  2. THEME PRESET MODEL
// ═══════════════════════════════════════════════════════
class AppThemePreset {
  final String id;
  final String name;
  final Color seed;
  final Brightness brightness;

  const AppThemePreset({
    required this.id,
    required this.name,
    required this.seed,
    this.brightness = Brightness.light,
  });

  ThemeData toThemeData() {
    final isDark = brightness == Brightness.dark;
    final scheme = ColorScheme.fromSeed(
      seedColor: seed,
      brightness: brightness,
    );

    return ThemeData(
      useMaterial3: true,
      colorScheme: scheme,

      scaffoldBackgroundColor:
          isDark ? scheme.surface : const Color(0xFFF4F7FA),

      cardColor: isDark ? scheme.surfaceContainerHighest : Colors.white,

      appBarTheme: AppBarTheme(
        backgroundColor: isDark ? scheme.surface : scheme.primary,
        foregroundColor: isDark ? scheme.onSurface : scheme.onPrimary,
        elevation: 0,
        centerTitle: false,
        titleTextStyle: TextStyle(
          fontSize: 18,
          fontWeight: FontWeight.bold,
          color: isDark ? scheme.onSurface : Colors.white,
        ),
      ),

      dividerColor: isDark ? scheme.outlineVariant : const Color(0xFFE8EEF5),

      elevatedButtonTheme: ElevatedButtonThemeData(
        style: ElevatedButton.styleFrom(
          backgroundColor: scheme.primary,
          foregroundColor: scheme.onPrimary,
          shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
          elevation: 0,
          padding: const EdgeInsets.symmetric(vertical: 14),
        ),
      ),

      inputDecorationTheme: InputDecorationTheme(
        filled: isDark,
        fillColor: isDark ? scheme.surfaceContainerHighest : null,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(
              color: isDark ? scheme.outline : Colors.grey.shade400),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(
              color: isDark ? scheme.outline : Colors.grey.shade400),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: scheme.primary, width: 1.6),
        ),
      ),
    );
  }
}

// ═══════════════════════════════════════════════════════
//  3. THEME PRESETS LIST
// ═══════════════════════════════════════════════════════
class AppThemes {
  static const presets = <AppThemePreset>[
    AppThemePreset(id: 'blue', name: 'Ocean Blue', seed: Color(0xFF1A8FE3)),
    AppThemePreset(id: 'green', name: 'Forest Green', seed: Color(0xFF2E7D32)),
    AppThemePreset(id: 'purple', name: 'Royal Purple', seed: Color(0xFF6A1B9A)),
    AppThemePreset(id: 'orange', name: 'Sunset Orange', seed: Color(0xFFEF6C00)),
    AppThemePreset(id: 'red', name: 'Ruby Red', seed: Color(0xFFC62828)),
    AppThemePreset(id: 'teal', name: 'Teal Wave', seed: Color(0xFF00897B)),
    AppThemePreset(
        id: 'dark',
        name: 'Midnight Dark',
        seed: Color(0xFF90CAF9),
        brightness: Brightness.dark),
  ];

  static AppThemePreset byId(String? id) =>
      presets.firstWhere((p) => p.id == id, orElse: () => presets.first);
}

// ═══════════════════════════════════════════════════════
//  4. BACKWARD COMPATIBILITY WRAPPER
//     (So the old AppTheme.light() call in main.dart still works)
// ═══════════════════════════════════════════════════════
class AppTheme {
  static ThemeData light() => AppThemes.presets.first.toThemeData();

  static ThemeData byId(String id) => AppThemes.byId(id).toThemeData();
}