import 'package:flutter/material.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

//
// Class Name       : ParentActivationPage
// Purpose          : Activate a Parent account using
//                    an emailed temporary PIN.
// Re-use           : Parent first-time login.
// Input Parameters : None.
// Output Type      : StatefulWidget.
//

class ParentActivationPage extends StatefulWidget {
  const ParentActivationPage({super.key});

  @override
  State<ParentActivationPage> createState() =>
      _ParentActivationPageState();
}

class _ParentActivationPageState
    extends State<ParentActivationPage> {
  final _supabase = Supabase.instance.client;

  final _emailController = TextEditingController();
  final _pinController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmController = TextEditingController();

  bool _pinVerified = false;
  bool _loading = false;
  String? _error;

  @override
  void dispose() {
    _emailController.dispose();
    _pinController.dispose();
    _passwordController.dispose();
    _confirmController.dispose();
    super.dispose();
  }

  //
  // Method Name      : verifyTemporaryPin
  // Purpose          : Verify the PIN received by email
  //                    and authenticate the Parent.
  // Input Parameters : Email and temporary PIN.
  // Output Type      : Future<void>.
  //
  Future<void> verifyTemporaryPin() async {
    final email = _emailController.text.trim().toLowerCase();
    final pin = _pinController.text.trim();

    if (!RegExp(r'^[^\s@]+@[^\s@]+\.[^\s@]+$').hasMatch(email) ||
        !RegExp(r'^[0-9]{6,8}$').hasMatch(pin)) {
      setState(() {
        _error = 'Enter a valid email and temporary PIN.';
      });
      return;
    }

    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      await _supabase.auth.verifyOTP(
        email: email,
        token: pin,
        type: OtpType.email,
      );

      final user = _supabase.auth.currentUser;

      if (user == null) {
        throw Exception('PIN verification failed.');
      }

      final profile = await _supabase
          .from('users')
          .select('role, is_active')
          .eq('id', user.id)
          .single();

      if (profile['role'] != 'Parent' || profile['is_active'] != true) {
        await _supabase.auth.signOut();
        throw Exception('Not an active Parent account.');
      }

      if (!mounted) return;

      setState(() {
        _pinVerified = true;
      });
    } catch (_) {
      if (!mounted) return;

      setState(() {
        _error = 'Invalid or expired PIN. '
            'Check the email and try again.';
      });
    } finally {
      if (mounted) {
        setState(() => _loading = false);
      }
    }
  }

  //
  // Method Name      : createPassword
  // Purpose          : Save the Parent's chosen password,
  //                    then complete account activation.
  // Input Parameters : New password and confirmation.
  // Output Type      : Future<void>.
  //
  Future<void> createPassword() async {
    final password = _passwordController.text;

    if (password.length < 12) {
      setState(() {
        _error = 'Password must contain at least 12 characters.';
      });
      return;
    }

    if (password != _confirmController.text) {
      setState(() {
        _error = 'Passwords do not match.';
      });
      return;
    }

    setState(() {
      _loading = true;
      _error = null;
    });

    try {
      final user = _supabase.auth.currentUser;

      if (!_pinVerified || user == null) {
        throw Exception('Verify your PIN first.');
      }

      // The Parent creates their own password.
      await _supabase.auth.updateUser(
        UserAttributes(password: password),
      );

      // Allow future normal password login.
      await _supabase
          .from('users')
          .update({
            'requires_password_change': false,
          })
          .eq('id', user.id);

      // End the temporary activation session.
      await _supabase.auth.signOut();

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Account activated. Log in with your new password.',
          ),
        ),
      );

      // Return to the Parent login screen.
      Navigator.of(context).popUntil(
        (route) => route.isFirst,
      );
    } catch (error) {
      if (!mounted) return;

      setState(() {
        _error = 'Activation could not be completed. '
            'Please try again or contact your school.';
      });
    } finally {
      if (mounted) {
        setState(() => _loading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    // ⭐ USING DYNAMIC THEME COLORS ⭐
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      appBar: AppBar(
        title: const Text('Activate Parent Account'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Icon(
              Icons.lock_reset,
              color: colorScheme.primary,
              size: 60,
            ),
            const SizedBox(height: 20),
            Text(
              _pinVerified
                  ? 'Create Your Password'
                  : 'Enter Temporary PIN',
              textAlign: TextAlign.center,
              style: TextStyle(
                fontSize: 23,
                fontWeight: FontWeight.bold,
                color: colorScheme.onSurface,
              ),
            ),
            const SizedBox(height: 20),
            if (!_pinVerified) ...[
              TextField(
                controller: _emailController,
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(
                  labelText: 'Email Address',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 15),
              TextField(
                controller: _pinController,
                keyboardType: TextInputType.number,
                decoration: const InputDecoration(
                  labelText: 'Temporary PIN',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _loading ? null : verifyTemporaryPin,
                child: const Text('Verify PIN'),
              ),
            ] else ...[
              Text(
                'Create your own password to access the Parent portal.',
                textAlign: TextAlign.center,
                style: TextStyle(color: colorScheme.onSurfaceVariant),
              ),
              const SizedBox(height: 15),
              TextField(
                controller: _passwordController,
                obscureText: true,
                decoration: const InputDecoration(
                  labelText: 'New Password',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 15),
              TextField(
                controller: _confirmController,
                obscureText: true,
                decoration: const InputDecoration(
                  labelText: 'Confirm Password',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _loading ? null : createPassword,
                child: const Text('Save New Password'),
              ),
            ],
            if (_loading)
              const Padding(
                padding: EdgeInsets.all(15),
                child: Center(
                  child: CircularProgressIndicator(),
                ),
              ),
            if (_error != null)
              Padding(
                padding: const EdgeInsets.all(15),
                child: Text(
                  _error!,
                  textAlign: TextAlign.center,
                  style: TextStyle(color: colorScheme.error),
                ),
              ),
          ],
        ),
      ),
    );
  }
}