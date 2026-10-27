import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

import 'package:school_parental_system_app/viewmodels/theme_view_model.dart';

import 'app.dart';
import 'services/supabase_service.dart';
import 'viewmodels/auth_view_model.dart';
import 'viewmodels/parent_view_model.dart';
import 'viewmodels/attendance_view_model.dart';
import 'viewmodels/grades_view_model.dart';
import 'viewmodels/report_view_model.dart';
import 'viewmodels/event_view_model.dart';
import 'viewmodels/announcement_view_model.dart';

import 'views/reset_password_screen.dart';

// Supabase project configuration
const String kSupabaseUrl =
    'https://jdperpigajvtywmhvecb.supabase.co';

// Keep your existing Supabase anon key here.
const String kSupabaseAnonKey =
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImpkcGVycGlnYWp2dHl3bWh2ZWNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3ODczMjU1MjIsImV4cCI6MjEwMjkwMTUyMn0.jhO1IwTI3bx0gJAebsp4Yp2QQd-jkVP7zu8E_Dw11OI';

// Global navigator key used to navigate from outside the widget tree
final GlobalKey<NavigatorState> navigatorKey =
    GlobalKey<NavigatorState>();

// Flag to prevent opening the reset screen multiple times
bool _resetScreenOpened = false;

// Opens the password reset screen using the global navigator key
void _openResetPasswordScreen() {
  // Prevent duplicate screens if this is called more than once
  if (_resetScreenOpened) return;

  final navigator = navigatorKey.currentState;

  // If the navigator is not ready yet, wait for the next frame and try again
  if (navigator == null) {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      _openResetPasswordScreen();
    });
    return;
  }

  _resetScreenOpened = true;

  // Push the reset password screen onto the navigation stack
  navigator.push(
    MaterialPageRoute<void>(
      builder: (_) => const ResetPasswordScreen(),
    ),
  );
}

Future<void> main() async {
  // Ensure Flutter bindings are initialized before using plugins
  WidgetsFlutterBinding.ensureInitialized();

  // Initialize the Supabase client with PKCE flow for secure auth
  await Supabase.initialize(
    url: kSupabaseUrl,
    anonKey: kSupabaseAnonKey,
    authOptions: const FlutterAuthClientOptions(
      authFlowType: AuthFlowType.pkce,
      detectSessionInUri: false,
    ),
  );

  final auth = Supabase.instance.client.auth;

  // Listen for recovery events before starting the app.
  auth.onAuthStateChange.listen(
    (data) {
      debugPrint('Supabase auth event: ${data.event}');

      // If a password recovery event occurs, open the reset screen
      if (data.event == AuthChangeEvent.passwordRecovery) {
        _openResetPasswordScreen();
      }
    },
    onError: (error) {
      debugPrint('Supabase auth listener error: $error');
    },
  );

  // Capture the initial URL before the app starts (useful for web)
  final uri = Uri.base;

  debugPrint(
    'Initial app URL: '
    '${uri.replace(queryParameters: {}).toString()}',
  );

  // Launch the app with all the required providers
  runApp(
    MultiProvider(
      providers: [
        // Core service provider
        Provider<SupabaseService>(
          create: (_) => SupabaseService(),
        ),
        
        // Authentication and user data
        ChangeNotifierProvider(
          create: (context) =>
              AuthViewModel(context.read<SupabaseService>()),
        ),
        ChangeNotifierProvider(
          create: (context) =>
              ParentViewModel(context.read<SupabaseService>()),
        ),
        
        // App-wide theme management
        ChangeNotifierProvider(
          create: (_) => ThemeViewModel(),
        ),
        
        // Feature-specific ViewModels
        ChangeNotifierProvider(
          create: (context) =>
              AttendanceViewModel(context.read<SupabaseService>()),
        ),
        ChangeNotifierProvider(
          create: (context) =>
              GradesViewModel(context.read<SupabaseService>()),
        ),
        ChangeNotifierProvider(
          create: (context) =>
              ReportViewModel(context.read<SupabaseService>()),
        ),
        ChangeNotifierProvider(
          create: (context) =>
              EventViewModel(context.read<SupabaseService>()),
        ),
        ChangeNotifierProvider(
          create: (context) =>
              AnnouncementViewModel(context.read<SupabaseService>()),
        ),
      ],
      // The root widget of the application
      child: ParentPortalApp(
        navigatorKey: navigatorKey,
      ),
    ),
  );

  // Process the PKCE recovery code from the initial URL
  final code = uri.queryParameters['code'];

  if (code != null && code.isNotEmpty) {
    try {
      debugPrint('Processing password recovery code...');

      // Exchange the recovery code for a valid session
      await auth.exchangeCodeForSession(code);

      debugPrint('Recovery session established.');

      // Open the password screen after Flutter has built its navigator
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _openResetPasswordScreen();
      });
    } on AuthException catch (error) {
      debugPrint('Password recovery failed: ${error.message}');
    } catch (error) {
      debugPrint('Unexpected recovery error: $error');
    }
  } else if (uri.queryParameters.containsKey('error_code')) {
    // Log any errors that were passed back in the URL
    debugPrint(
      'Recovery link error: '
      '${uri.queryParameters['error_code']} - '
      '${uri.queryParameters['error_description']}',
    );
  }
}