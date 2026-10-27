import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../viewmodels/auth_view_model.dart';
import '../viewmodels/parent_view_model.dart';
import 'change_password_screen.dart';
import 'personal_info_screen.dart';
import 'notification_screen.dart';
import 'theme_screen.dart';
import 'welcome_screen.dart';

// Displays the parent's profile information and provides navigation to settings
class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Select only the necessary parent data to minimize rebuilds
    final parentData = context.select<ParentViewModel, Map<String, String?>>(
      (vm) => {
        'name': vm.parent?.fullName,
        'picture': vm.parent?.profilePicture,
      },
    );

    // Extract the values from the map
    final name = parentData['name'];
    final profilePicture = parentData['picture'];
    
    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Profile')),
      body: SingleChildScrollView(
        child: Column(
          children: [
            // Parent Header section (tappable to edit personal info)
            GestureDetector(
              onTap: () {
                // Navigate to the personal information editing screen
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (_) => const PersonalInfoScreen()),
                );
              },
              child: Container(
                padding: const EdgeInsets.all(20),
                child: Row(
                  children: [
                    // Avatar with an edit icon overlay
                    Stack(
                      children: [
                        // Profile image or fallback icon
                        Container(
                          width: 64,
                          height: 64,
                          decoration: BoxDecoration(
                            color: colorScheme.primaryContainer,
                            shape: BoxShape.circle,
                          ),
                          clipBehavior: Clip.antiAlias,
                          child: profilePicture != null && profilePicture.isNotEmpty
                              ? Image.network(
                                  profilePicture,
                                  fit: BoxFit.cover,
                                  errorBuilder: (context, error, stackTrace) {
                                    return Icon(Icons.person, size: 34, color: colorScheme.onPrimaryContainer);
                                  },
                                )
                              : Icon(Icons.person, size: 34, color: colorScheme.onPrimaryContainer),
                        ),
                        // Small edit pencil icon positioned at the bottom right
                        Positioned(
                          bottom: 0,
                          right: 0,
                          child: Container(
                            padding: const EdgeInsets.all(4),
                            decoration: BoxDecoration(
                              color: colorScheme.primary,
                              shape: BoxShape.circle,
                              border: Border.all(color: theme.scaffoldBackgroundColor, width: 2),
                            ),
                            child: Icon(Icons.edit, size: 12, color: colorScheme.onPrimary),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(width: 16),
                    
                    // Name and role
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(name ?? 'Parent',
                            style: TextStyle(fontSize: 19, fontWeight: FontWeight.bold, color: colorScheme.onSurface)),
                        const SizedBox(height: 3),
                        Text('Parent',
                            style: TextStyle(fontSize: 13, color: colorScheme.primary, fontWeight: FontWeight.w600)),
                      ],
                    ),
                  ],
                ),
              ),
            ),

            Divider(height: 1, color: theme.dividerColor),
            const SizedBox(height: 14),

            // Menu Tiles
            // Change Password
            _tile(
              context, 
              Icons.lock_outline, 
              'Change Password',
              onTap: () {
                Navigator.push(context, MaterialPageRoute(builder: (_) => const ChangePasswordScreen()));
              },
            ),
            
            // Notification Settings
            _tile(
              context,
              Icons.notifications_none, 
              'Notification Settings',
              onTap: () {
                Navigator.push(context, MaterialPageRoute(builder: (_) => const NotificationSettingsScreen()));
              },
            ),
            
            // Theme Settings
            _tile(
              context,
              Icons.settings_outlined, 
              'Theme',
              onTap: () {
                Navigator.push(context, MaterialPageRoute(builder: (_) => const ThemeScreen()));
              },
            ),
            
            // Log Out
            _tile(
              context,
              Icons.logout,
              'Log Out',
              danger: true,
              onTap: () async {
                // Sign out the user
                await context.read<AuthViewModel>().signOut();
                if (context.mounted) {
                  // Clear parent data and navigate back to welcome screen
                  context.read<ParentViewModel>().reset();
                  Navigator.of(context).pushAndRemoveUntil(
                    MaterialPageRoute(builder: (context) => const WelcomeScreen()),
                    (route) => false,
                  );
                }
              },
            ),
            const SizedBox(height: 30),
          ],
        ),
      ),
    );
  }

  // Helper widget to build each menu tile
  Widget _tile(
    BuildContext context,
    IconData icon, 
    String title, {
    bool danger = false, 
    VoidCallback? onTap,
  }) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;
    
    // Apply different colors for danger (like Log Out)
    final color = danger ? colorScheme.error : colorScheme.primary;
    final textColor = danger ? colorScheme.error : colorScheme.onSurface;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 5),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 16),
        decoration: BoxDecoration(
          color: theme.cardColor,
          borderRadius: BorderRadius.circular(12),
          border: Border.all(color: theme.dividerColor),
        ),
        child: Row(
          children: [
            // Leading icon
            Icon(icon, color: color, size: 22),
            const SizedBox(width: 14),
            
            // Title text
            Expanded(
              child: Text(title,
                  style: TextStyle(fontSize: 14, fontWeight: FontWeight.w600, color: textColor)),
            ),
            
            // Trailing arrow
            Icon(Icons.chevron_right, color: colorScheme.onSurfaceVariant),
          ],
        ),
      ),
    );
  }
}