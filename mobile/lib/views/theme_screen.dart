import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../theme.dart';
import '../viewmodels/theme_view_model.dart';

// Screen that allows the user to pick a color theme for the app
class ThemeScreen extends StatelessWidget {
  const ThemeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Watch the ThemeViewModel so the UI rebuilds when a new theme is selected
    final themeVm = context.watch<ThemeViewModel>();
    
    // The ID of the currently active theme preset
    final selectedId = themeVm.preset.id;

    // Access the current theme for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Choose a Theme')),
      body: ListView.builder(
        padding: const EdgeInsets.all(16),
        itemCount: AppThemes.presets.length,
        itemBuilder: (context, i) {
          // Get the current preset from the list
          final preset = AppThemes.presets[i];
          
          // Check if this preset is the currently selected one
          final isSelected = preset.id == selectedId;

          // Generate a preview color scheme for this specific preset
          final scheme = ColorScheme.fromSeed(
            seedColor: preset.seed,
            brightness: preset.brightness,
          );

          return GestureDetector(
            // Apply the theme when the card is tapped
            onTap: () => themeVm.select(preset.id),
            child: Container(
              margin: const EdgeInsets.only(bottom: 14),
              padding: const EdgeInsets.all(14),
              decoration: BoxDecoration(
                // Use the app's card color for the background
                color: theme.cardColor,
                borderRadius: BorderRadius.circular(14),
                border: Border.all(
                  // Highlight the border if selected, otherwise use a divider color
                  color: isSelected
                      ? scheme.primary
                      : theme.dividerColor,
                  width: isSelected ? 2 : 1,
                ),
              ),
              child: Row(
                children: [
                  // Color swatch preview showing the primary color of the preset
                  Container(
                    width: 56,
                    height: 56,
                    decoration: BoxDecoration(
                      color: scheme.primary,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    alignment: Alignment.center,
                    child: Icon(Icons.palette,
                        color: scheme.onPrimary, size: 24),
                  ),
                  const SizedBox(width: 14),
                  
                  // Theme name and type (Light/Dark)
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          preset.name,
                          style: TextStyle(
                            fontSize: 15, 
                            fontWeight: FontWeight.bold,
                            color: colorScheme.onSurface,
                          ),
                        ),
                        const SizedBox(height: 4),
                        Text(
                          preset.brightness == Brightness.dark
                              ? 'Dark theme'
                              : 'Light theme',
                          style: TextStyle(
                            fontSize: 12, 
                            color: colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ],
                    ),
                  ),
                  
                  // Selection indicator (radio button style)
                  Icon(
                    isSelected
                        ? Icons.radio_button_checked
                        : Icons.radio_button_off,
                    color: isSelected 
                        ? scheme.primary 
                        : colorScheme.outlineVariant,
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}