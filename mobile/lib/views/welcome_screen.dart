import 'package:flutter/material.dart';

import 'login_screen.dart';

// The initial screen displayed to users before they log in
class WelcomeScreen extends StatelessWidget {
  const WelcomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Access the theme's color scheme for dynamic styling
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      // Uses the theme's background color, which adapts to dark mode
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      body: SafeArea(
        child: Column(
          children: [
            // TOP BAR using the theme's primary color
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              color: colorScheme.primary,
              child: Row(
                children: [
                  // Department of Basic Education logo container
                  Container(
                    width: 44,
                    height: 44,
                    alignment: Alignment.center,
                    decoration: BoxDecoration(
                      color: Colors.white,
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Image.asset(
                      'asset/image/education.png',
                      width: 40,
                      height: 40,
                      fit: BoxFit.contain,
                      // Fallback icon if the image fails to load
                      errorBuilder: (context, error, stackTrace) {
                        return Icon(
                          Icons.shield_outlined,
                          color: colorScheme.primary,
                          size: 26,
                        );
                      },
                    ),
                  ),
                  const SizedBox(width: 12),

                  // App title
                  Expanded(
                    child: Text(
                      'School-Parental System',
                      style: TextStyle(
                        color: colorScheme.onPrimary,
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                ],
              ),
            ),

            // SCROLLABLE CONTENT
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(20),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const SizedBox(height: 20),

                    // Instruction text for parents
                    Text(
                      'Registered Parents of the learner ',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: colorScheme.onSurface,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'Please click on',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 14,
                        color: colorScheme.onSurface,
                      ),
                    ),
                    const SizedBox(height: 8),
                    Text(
                      'on the Login box and log in with\nyour Phone number or email.',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 14,
                        color: colorScheme.onSurface,
                      ),
                    ),
                    const SizedBox(height: 16),
                    Text(
                      'For example: Parents of\nthe learner use Username: Phone Number or email address',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 13,
                        color: colorScheme.onSurfaceVariant,
                      ),
                    ),
                    const SizedBox(height: 6),
                    // Example phone number text
                    Text(
                      '(e.g., 0765432390)',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 13,
                        fontWeight: FontWeight.bold,
                        color: colorScheme.onSurface,
                      ),
                    ),
                    const SizedBox(height: 6),
                    Text(
                      'and your assigned\npassword',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 13,
                        color: colorScheme.onSurfaceVariant,
                      ),
                    ),

                    const SizedBox(height: 40),

                    // Login button
                    SizedBox(
                      height: 60,
                      child: ElevatedButton(
                        onPressed: () {
                          // Navigate to the login screen
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                                builder: (_) => const LoginScreen()),
                          );
                        },
                        style: ElevatedButton.styleFrom(
                          backgroundColor: colorScheme.primary,
                          foregroundColor: colorScheme.onPrimary,
                          shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(6)),
                          elevation: 0,
                        ),
                        child: const Text(
                          'Login as a Parent',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                              fontSize: 15, fontWeight: FontWeight.w600),
                        ),
                      ),
                    ),

                    const SizedBox(height: 20),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}