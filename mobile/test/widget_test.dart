import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';
import 'package:school_parental_system_app/app.dart';
import 'package:school_parental_system_app/services/supabase_service.dart';
import 'package:school_parental_system_app/viewmodels/auth_view_model.dart';
import 'package:school_parental_system_app/viewmodels/parent_view_model.dart';
import 'package:school_parental_system_app/viewmodels/theme_view_model.dart';

void main() {
  testWidgets('App renders correctly', (WidgetTester tester) async {
    // 1. Create the navigator key
    final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

    // 2. Build the app with the required Providers and the navigatorKey
    await tester.pumpWidget(
      MultiProvider(
        providers: [
          Provider<SupabaseService>(create: (_) => SupabaseService()),
          ChangeNotifierProvider(
              create: (c) => AuthViewModel(c.read<SupabaseService>())),
          ChangeNotifierProvider(
              create: (c) => ParentViewModel(c.read<SupabaseService>())),
          ChangeNotifierProvider(create: (_) => ThemeViewModel()),
        ],
        child: ParentPortalApp(navigatorKey: navigatorKey),
      ),
    );

    // 3. Since the app starts on a loading screen or welcome screen, 
    // we just verify that the app renders without throwing any errors.
    expect(find.byType(MaterialApp), findsOneWidget);
  });
}