import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'viewmodels/auth_view_model.dart';
import 'viewmodels/theme_view_model.dart';
import 'views/welcome_screen.dart';
import 'views/main_shell.dart';
import 'views/must_change_password_screen.dart';

// The root widget of the application that configures the theme and routing
class ParentPortalApp extends StatelessWidget {
  // A global key used to access the navigator from anywhere in the app
  final GlobalKey<NavigatorState> navigatorKey;

  // Constructor requires the navigator key to be passed in
  const ParentPortalApp({
    super.key,
    required this.navigatorKey,
  });

  @override
  Widget build(BuildContext context) {
    // Watch the ThemeViewModel so the entire app rebuilds when the theme changes
    final themeVm = context.watch<ThemeViewModel>();

    return MaterialApp(
      title: 'School-Parental System',
      debugShowCheckedModeBanner: false,
      
      // Apply the currently selected theme
      theme: themeVm.theme,
      
      // Attach the global navigator key
      navigatorKey: navigatorKey,
      
      // Use a Consumer to react to authentication state changes
      home: Consumer<AuthViewModel>(
        builder: (context, auth, child) {
          // Log every rebuild to help with diagnosing state changes
          debugPrint('App rebuild: status=${auth.status}');

          // Determine which screen to show based on the current auth status
          switch (auth.status) {
            case AuthStatus.loading:
              // Show a loading spinner while authentication is in progress
              return const Scaffold(
                body: Center(child: CircularProgressIndicator()),
              );
            case AuthStatus.mustChangePassword:
              // Force the user to change their temporary password
              return const ParentActivationPage();
            case AuthStatus.authenticated:
              // User is logged in, show the main app shell
              return const MainShell();
            case AuthStatus.idle:
            case AuthStatus.error:
            case AuthStatus.unknown:
              // User is not logged in, show the welcome screen
              return const WelcomeScreen();
          }
        },
      ),
    );
  }
}