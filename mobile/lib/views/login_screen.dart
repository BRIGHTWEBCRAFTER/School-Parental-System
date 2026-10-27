import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../theme.dart';
import '../viewmodels/auth_view_model.dart';
import 'main_shell.dart';
import 'forgot_password_screen.dart';
import 'must_change_password_screen.dart';

// Screen for parents to log in to the application
class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  // Controllers for the input fields
  final _idCtrl = TextEditingController();
  final _codeCtrl = TextEditingController();
  
  // State for password visibility and auto-hide timer
  bool _obscurePassword = true; 
  Timer? _hidePasswordTimer; 

  @override
  void dispose() {
    // Clean up controllers and cancel any active timers
    _idCtrl.dispose();
    _codeCtrl.dispose();
    _hidePasswordTimer?.cancel(); 
    super.dispose();
  }

  // Toggles password visibility and starts a 10-second timer to auto-hide it
  void _togglePasswordVisibility() {
    setState(() {
      _obscurePassword = !_obscurePassword;
    });

    // If the password is now visible, start the timer
    if (!_obscurePassword) {
      _hidePasswordTimer?.cancel(); 
      _hidePasswordTimer = Timer(const Duration(seconds: 10), () {
        if (mounted) {
          setState(() {
            _obscurePassword = true;
          });
        }
      });
    } else {
      // If hidden manually, cancel the timer
      _hidePasswordTimer?.cancel();
    }
  }

  @override
  Widget build(BuildContext context) {
    // Watch the AuthViewModel for state changes
    final auth = context.watch<AuthViewModel>();
    final loading = auth.status == AuthStatus.loading;

    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.symmetric(horizontal: 26, vertical: 30),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Back button to return to the previous screen
              Align(
                alignment: Alignment.centerLeft,
                child: IconButton(
                  padding: EdgeInsets.zero,
                  alignment: Alignment.centerLeft,
                  icon: const Icon(
                    Icons.arrow_back,
                    color: AppColors.primaryDark,
                    size: 26,
                  ),
                  onPressed: () => Navigator.pop(context),
                ),
              ),

              const SizedBox(height: 10),
              
              // App logo
              Center(
                child: Container(
                  width: 80,
                  height: 80,
                  decoration: const BoxDecoration(
                    image: DecorationImage(
                      image: AssetImage('asset/image/Background.jpg'),
                      fit: BoxFit.cover,
                    ),
                    shape: BoxShape.circle,
                  ),
                ),
              ),
              const SizedBox(height: 16),
              
              // App title
              const Center(
                child: Text(
                  'School-Parental System',
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: AppColors.primaryDark,
                  ),
                ),
              ),
              const SizedBox(height: 4),
              
              // Subtitle
              const Center(
                child: Text(
                  'Parent / Guardian Sign In',
                  style: TextStyle(fontSize: 12, color: Colors.grey),
                ),
              ),
              const SizedBox(height: 40),
              
              // ID/Email Field
              const Text(
                'Phone Number/ Email *',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
              ),
              const SizedBox(height: 6),
              TextField(
                controller: _idCtrl,
                onChanged: (value) =>
                    context.read<AuthViewModel>().updateIdNumber(value),
                decoration:
                    const InputDecoration(hintText: 'e.g., 0744568768'),
              ),
              const SizedBox(height: 16),
              
              // Password Field
              const Text(
                'Password *',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600),
              ),
              const SizedBox(height: 6),
              TextField(
                controller: _codeCtrl,
                onChanged: (value) =>
                    context.read<AuthViewModel>().updateAccessCode(value),
                obscureText: _obscurePassword,
                decoration: InputDecoration(
                  hintText: 'Password',
                  suffixIcon: IconButton(
                    icon: Icon(
                      _obscurePassword ? Icons.visibility_off : Icons.visibility,
                      color: Colors.grey,
                    ),
                    onPressed: _togglePasswordVisibility, 
                  ),
                ),
              ),
              
              const SizedBox(height: 20),
              
              // Error message display
              if (auth.error != null) ...[
                Container(
                  padding: const EdgeInsets.all(10),
                  decoration: BoxDecoration(
                    color: const Color(0xFFFDE8E8),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    auth.error!,
                    style: const TextStyle(color: AppColors.red, fontSize: 12),
                  ),
                ),
                const SizedBox(height: 12),
              ],
              
              // Sign In Button
              SizedBox(
                height: 52,
                child: ElevatedButton(
                  onPressed: loading
                      ? null
                      : () async {
                          // Attempt to sign in
                          final ok =
                              await context.read<AuthViewModel>().signIn();
                          if (!context.mounted) return;

                          if (ok) {
                            // Navigate to the main shell on success
                            Navigator.of(context).pushAndRemoveUntil(
                              MaterialPageRoute(
                                  builder: (_) => const MainShell()),
                              (route) => false,
                            );
                          } else {
                            // Show error message on failure
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(
                                content: Text(
                                    'Login failed. Check your credentials.'),
                                backgroundColor: Colors.red,
                              ),
                            );
                          }
                        },
                  child: loading
                      // Show a loading spinner while signing in
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(
                            color: Colors.white,
                            strokeWidth: 2.5,
                          ),
                        )
                      // Show the button text otherwise
                      : const Text(
                          'Sign In',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                ),
              ),

              // Forgot Password Button
              const SizedBox(height: 16),
              TextButton(
                onPressed: () {
                  // Navigate to the password reset screen
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => const ForgotPasswordScreen(),
                    ),
                  );
                },
                child: const Text(
                  'Forgot Password?',
                  style: TextStyle(
                    color: AppColors.primary,
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),

              // ⭐ NEW: ACTIVATE ACCOUNT BUTTON ⭐
              TextButton(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const ParentActivationPage(),
                    ),
                  );
                },
                child: const Text(
                  'Received a temporary PIN? Activate Account',
                  style: TextStyle(
                    color: AppColors.primaryDark,
                    fontSize: 13,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),

            ],
          ),
        ),
      ),
    );
  }
}