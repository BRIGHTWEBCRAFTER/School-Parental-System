/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */



import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../theme.dart';

// Manages the application's theme state and persists the user's selection
class ThemeViewModel extends ChangeNotifier {
  // Key used to store the selected theme ID in SharedPreferences
  static const String _prefsKey = 'selected_theme_id';
  
  // Internal state variables
  AppThemePreset _preset = AppThemes.presets.first;
  bool _isLoaded = false;

  // Getters to expose state to the UI
  AppThemePreset get preset => _preset;
  ThemeData get theme => _preset.toThemeData();
  bool get isLoaded => _isLoaded;

  // Constructor triggers the loading of the previously saved theme
  ThemeViewModel() {
    _loadSavedTheme();
  }

  // Load saved theme from SharedPreferences at app start
  Future<void> _loadSavedTheme() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final savedId = prefs.getString(_prefsKey);
      
      // If a saved theme ID exists, apply it
      if (savedId != null) {
        _preset = AppThemes.byId(savedId);
        debugPrint('Loaded saved theme: ${_preset.name}');
      }
    } catch (e) {
      debugPrint('Failed to load saved theme: $e');
    }
    
    // Mark as loaded and notify listeners to rebuild the UI
    _isLoaded = true;
    notifyListeners();
  }

  // Change theme and persist the choice
  Future<void> select(String id) async {
    final next = AppThemes.byId(id);
    if (next.id == _preset.id) return;
    
    // Update the local state and notify the UI immediately
    _preset = next;
    notifyListeners();

    try {
      // Save the newly selected theme ID to SharedPreferences
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString(_prefsKey, id);
      debugPrint('Saved theme: ${next.name}');
    } catch (e) {
      debugPrint('Failed to save theme: $e');
    }
  }
}