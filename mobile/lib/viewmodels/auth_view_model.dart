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
import 'package:supabase_flutter/supabase_flutter.dart';

import '../services/supabase_service.dart';

// Defines the possible authentication states for the application
enum AuthStatus {
  unknown,
  idle,
  loading,
  authenticated,
  mustChangePassword,
  error,
}

// Manages the authentication state and logic
class AuthViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor initializes the view model and checks the current session
  AuthViewModel(this._api) {
    _init();
  }

  // Internal state variables
  AuthStatus _status = AuthStatus.unknown;
  String _idNumber = '';
  String _accessCode = '';
  String? _error;

  // Getters to expose state to the UI
  AuthStatus get status => _status;
  String get idNumber => _idNumber;
  String get accessCode => _accessCode;
  String? get error => _error;
  bool get isAuthenticated => _status == AuthStatus.authenticated;

  // Initializes the authentication state by checking for an existing session
  void _init() {
    debugPrint('AuthViewModel._init()');
    final session = Supabase.instance.client.auth.currentSession;

    // If a session exists, check if the user needs to change their password
    if (session != null) {
      _checkMustChangePassword();
    } else {
      _status = AuthStatus.idle;
    }

    // Listen for sign-out events to reset the state
    Supabase.instance.client.auth.onAuthStateChange.listen((data) {
      debugPrint('AuthStateChange: ${data.event}');
      if (data.event == AuthChangeEvent.signedOut) {
        _status = AuthStatus.idle;
        notifyListeners();
      }
      // Note: Signed-in events are handled explicitly by the signIn method
      // to ensure the mustChangePassword status is evaluated correctly.
    });

    notifyListeners();
  }

  // Checks the user's metadata to determine if a password change is required
  void _checkMustChangePassword() {
    // Force fresh user data by checking the current session
    final session = Supabase.instance.client.auth.currentSession;
    final user = session?.user;
    
    if (user == null) {
      _status = AuthStatus.idle;
      return;
    }

    final mustChange = user.userMetadata?['must_change_password'] == true;
    debugPrint('_checkMustChangePassword: mustChange=$mustChange');
    
    if (mustChange) {
      _status = AuthStatus.mustChangePassword;
    } else {
      _status = AuthStatus.authenticated;
    }
  }

  // Updates the stored ID number from the UI input
  void updateIdNumber(String value) {
    _idNumber = value;
    notifyListeners();
  }

  // Updates the stored access code from the UI input
  void updateAccessCode(String value) {
    _accessCode = value;
    notifyListeners();
  }

  // Handles the sign-in process using either an email or phone number
  Future<bool> signIn() async {
    debugPrint('signIn() id="$_idNumber" code_len=${_accessCode.length}');

    // Validate that both fields are filled before proceeding
    if (_idNumber.isEmpty || _accessCode.isEmpty) {
      _error = 'Please enter your Phone Number/Email or password';
      _status = AuthStatus.error;
      notifyListeners();
      return false;
    }

    _status = AuthStatus.loading;
    _error = null;
    notifyListeners();

    final isEmail = _idNumber.contains('@');

    try {
      // Call the appropriate sign-in method based on the input format
      final ok = isEmail
          ? await _api.signInWithEmail(
              email: _idNumber, accessCode: _accessCode)
          : await _api.signInWithIdNumber(
              idNumber: _idNumber, accessCode: _accessCode);

      debugPrint('signIn result: $ok');

      if (ok) {
        await Future.delayed(const Duration(milliseconds: 100));
        
        // After successful login, check if the user needs to change their password
        _checkMustChangePassword();
        
        debugPrint('After check - status: $_status');
        notifyListeners();
        return true;
      }

      _error = 'Invalid Phone Number/Email or password';
      _status = AuthStatus.error;
      notifyListeners();
      return false;
    } catch (e) {
      debugPrint('signIn exception: $e');
      _error = e.toString();
      _status = AuthStatus.error;
      notifyListeners();
      return false;
    }
  }

  // Handles the process of changing the user's password
  Future<bool> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    final supabase = Supabase.instance.client;
    String? userEmail;

    try {
      final user = supabase.auth.currentUser;
      if (user == null || user.email == null) {
        throw Exception('No logged in user found.');
      }
      userEmail = user.email;

      // Sign out first to ensure a clean session for re-authentication
      debugPrint('1. Signing out for a clean session');
      await supabase.auth.signOut();

      // Re-authenticate using the current password to verify the user
      debugPrint('2. Signing in with current password');
      final authResponse = await supabase.auth.signInWithPassword(
        email: userEmail!,
        password: currentPassword,
      );

      if (authResponse.user == null || authResponse.session == null) {
        throw Exception('Re-authentication failed.');
      }

      debugPrint('3. Waiting for session cache...');
      await Future.delayed(const Duration(milliseconds: 500));

      final currentSession = supabase.auth.currentSession;
      if (currentSession == null) {
        throw Exception('Session lost during re-authentication.');
      }

      // Update the user's password and clear the must_change_password flag
      debugPrint('4. Updating password');
      await supabase.auth.updateUser(
        UserAttributes(
          password: newPassword,
          data: {'must_change_password': false},
        ),
      );

      debugPrint('5. Refreshing session');
      await supabase.auth.refreshSession();

      // Wait for metadata to fully propagate
      await Future.delayed(const Duration(milliseconds: 300));

      debugPrint('Password updated. Setting status to authenticated');

      _status = AuthStatus.authenticated;
      _error = null;
      notifyListeners();
      return true;
    } on AuthException catch (e) {
      debugPrint('changePassword AuthException: ${e.message}');

      // Attempt to restore the session if it was lost during the process
      if (userEmail != null && supabase.auth.currentUser == null) {
        try {
          await supabase.auth.signInWithPassword(
            email: userEmail,
            password: currentPassword,
          );
        } catch (_) {}
      }

      _error = _humanizeError(e.message);
      notifyListeners();
      return false;
    } catch (e) {
      debugPrint('changePassword exception: $e');
      _error = e.toString();
      notifyListeners();
      return false;
    }
  }

  // Converts raw Supabase error messages into user-friendly text
  String _humanizeError(String raw) {
    if (raw.contains('current_password_required')) {
      return 'Current password is incorrect. Please try again.';
    }
    if (raw.contains('Invalid login credentials')) {
      return 'Current password is incorrect.';
    }
    if (raw.contains('New password should be different')) {
      return 'New password must be different from your current password.';
    }
    if (raw.contains('Password should be at least')) {
      return 'Password is too short. Use at least 6 characters.';
    }
    return raw;
  }

  // Signs the user out and resets all local state
  Future<void> signOut() async {
    await _api.signOut();
    _idNumber = '';
    _accessCode = '';
    _status = AuthStatus.idle;
    _error = null;
    notifyListeners();
  }

  // Sends a password reset email to the provided address
  Future<void> sendPasswordReset(String email) async {
    await Supabase.instance.client.auth.resetPasswordForEmail(
      email.trim(),
      redirectTo: 'http://localhost:44324',
    );
  }
}