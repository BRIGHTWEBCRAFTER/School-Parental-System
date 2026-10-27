import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../viewmodels/auth_view_model.dart';

// Screen that allows the user to request a password reset link via email
class ForgotPasswordScreen extends StatefulWidget {
  const ForgotPasswordScreen({super.key});

  @override
  State<ForgotPasswordScreen> createState() => _ForgotPasswordScreenState();
}

class _ForgotPasswordScreenState extends State<ForgotPasswordScreen> {
  // Controller to manage the email input field
  final _emailCtrl = TextEditingController();
  
  // Key used to validate the form
  final _formKey = GlobalKey<FormState>();
  
  // Tracks the loading state while sending the email
  bool _isLoading = false;

  // Handles sending the reset email
  Future<void> _resetPassword() async {
    // Validate the form input first
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    try {
      // Call the ViewModel to trigger the password reset email
      await context.read<AuthViewModel>().sendPasswordReset(_emailCtrl.text);

      if (!mounted) return;
      
      // Show a success message if the email was sent
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Password reset link sent to your email!'),
          backgroundColor: Colors.green,
        ),
      );
      
      // Navigate back to the login screen
      Navigator.pop(context); 
    } catch (e) {
      if (!mounted) return;
      
      // Show an error message if something goes wrong
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Error: $e'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      // Always stop the loading state when finished
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  void dispose() {
    // Clean up the controller when the widget is destroyed
    _emailCtrl.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    // Access theme colors for dynamic styling
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Forgot Password')),
      body: Padding(
        padding: const EdgeInsets.all(26.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 20),
              
              // Decorative lock icon
              Icon(
                Icons.lock_reset, 
                size: 70, 
                color: colorScheme.primary,
              ),
              const SizedBox(height: 20),
              
              // Instructions text
              Text(
                'Enter the email address associated with your account and we will send you a link to reset your password.',
                textAlign: TextAlign.center,
                style: TextStyle(
                  fontSize: 14, 
                  color: colorScheme.onSurfaceVariant,
                ),
              ),
              const SizedBox(height: 30),
              
              // Field label
              Text(
                'Email Address',
                style: TextStyle(
                  fontSize: 13, 
                  fontWeight: FontWeight.w600,
                  color: colorScheme.onSurface,
                ),
              ),
              const SizedBox(height: 6),
              
              // Email input field
              TextFormField(
                controller: _emailCtrl,
                keyboardType: TextInputType.emailAddress,
                style: TextStyle(color: colorScheme.onSurface),
                decoration: const InputDecoration(
                  hintText: 'Enter your email',
                  border: OutlineInputBorder(),
                ),
                validator: (val) {
                  // Basic validation for required field and email format
                  if (val == null || val.isEmpty) return 'Email is required';
                  if (!val.contains('@')) return 'Enter a valid email';
                  return null;
                },
              ),
              const SizedBox(height: 30),
              
              // Submit button
              SizedBox(
                height: 50,
                child: ElevatedButton(
                  // Disable button while loading
                  onPressed: _isLoading ? null : _resetPassword,
                  child: _isLoading
                      // Show spinner while loading
                      ? const CircularProgressIndicator(color: Colors.white)
                      // Show button text otherwise
                      : const Text(
                          'Send Reset Link',
                          style: TextStyle(
                              fontSize: 16, fontWeight: FontWeight.w600),
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