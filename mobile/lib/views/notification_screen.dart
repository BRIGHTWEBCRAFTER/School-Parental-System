import 'package:flutter/material.dart';

// Screen that allows the user to configure their notification preferences
class NotificationSettingsScreen extends StatefulWidget {
  const NotificationSettingsScreen({super.key});

  @override
  State<NotificationSettingsScreen> createState() =>
      _NotificationSettingsScreenState();
}

class _NotificationSettingsScreenState
    extends State<NotificationSettingsScreen> {
  // State variables to track the toggle positions
  bool _pushEnabled = true;
  bool _emailEnabled = true;
  bool _smsEnabled = false;
  
  // Tracks whether the settings are currently being saved
  bool _isSaving = false;

  @override
  void initState() {
    super.initState();
    // Future implementation: load saved preferences from database or local storage
  }

  // Handles saving the selected notification settings
  Future<void> _saveSettings() async {
    setState(() => _isSaving = true);

    try {
      // Simulate a network call to save preferences
      await Future.delayed(const Duration(milliseconds: 600));

      if (!mounted) return;
      
      // Show success message
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Notification settings saved!'),
          backgroundColor: Colors.green,
        ),
      );
      
      // Return to the previous screen
      Navigator.pop(context);
    } catch (e) {
      if (!mounted) return;
      
      // Show error message if saving fails
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Failed to save: $e'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      // Always stop the loading state
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Notification Settings')),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header section explaining the screen's purpose
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(16),
              color: colorScheme.primaryContainer,
              child: Text(
                'Choose how you want to be notified about school events, announcements, and learner progress.',
                style: TextStyle(
                  fontSize: 13,
                  color: colorScheme.onPrimaryContainer,
                ),
              ),
            ),

            const SizedBox(height: 16),
            
            // Section title for delivery methods
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: Text(
                'Delivery Methods',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: colorScheme.primary,
                ),
              ),
            ),
            const SizedBox(height: 8),

            // Push Notifications toggle
            SwitchListTile(
              title: Text('Push Notifications',
                  style: TextStyle(color: colorScheme.onSurface)),
              subtitle: Text('Receive alerts directly on your phone',
                  style: TextStyle(color: colorScheme.onSurfaceVariant)),
              secondary: Icon(Icons.notifications_active_outlined,
                  color: colorScheme.primary),
              value: _pushEnabled,
              activeThumbColor: colorScheme.primary,
              onChanged: (val) => setState(() => _pushEnabled = val),
            ),
            Divider(height: 1, color: theme.dividerColor),

            // Email Notifications toggle
            SwitchListTile(
              title: Text('Email Notifications',
                  style: TextStyle(color: colorScheme.onSurface)),
              subtitle: Text('Receive a daily summary via email',
                  style: TextStyle(color: colorScheme.onSurfaceVariant)),
              secondary: Icon(Icons.email_outlined, color: colorScheme.primary),
              value: _emailEnabled,
              activeThumbColor: colorScheme.primary,
              onChanged: (val) => setState(() => _emailEnabled = val),
            ),
            Divider(height: 1, color: theme.dividerColor),

            // SMS Notifications toggle
            SwitchListTile(
              title: Text('SMS Notifications',
                  style: TextStyle(color: colorScheme.onSurface)),
              subtitle: Text('Receive urgent alerts via text message',
                  style: TextStyle(color: colorScheme.onSurfaceVariant)),
              secondary: Icon(Icons.sms_outlined, color: colorScheme.primary),
              value: _smsEnabled,
              activeThumbColor: colorScheme.primary,
              onChanged: (val) => setState(() => _smsEnabled = val),
            ),

            const SizedBox(height: 24),
            
            // Section title for notification categories
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: Text(
                'Notification Types',
                style: TextStyle(
                  fontSize: 14,
                  fontWeight: FontWeight.bold,
                  color: colorScheme.primary,
                ),
              ),
            ),
            const SizedBox(height: 8),

            // List of notification categories (currently placeholders)
            ListTile(
              leading:
                  Icon(Icons.campaign_outlined, color: colorScheme.primary),
              title: Text('Announcements',
                  style: TextStyle(color: colorScheme.onSurface)),
              trailing: Icon(Icons.chevron_right,
                  color: colorScheme.onSurfaceVariant),
              onTap: () {}, 
            ),
            ListTile(
              leading: Icon(Icons.calendar_month_outlined,
                  color: colorScheme.primary),
              title: Text('Events & Reminders',
                  style: TextStyle(color: colorScheme.onSurface)),
              trailing: Icon(Icons.chevron_right,
                  color: colorScheme.onSurfaceVariant),
              onTap: () {},
            ),
            ListTile(
              leading: Icon(Icons.school_outlined, color: colorScheme.primary),
              title: Text('Academic Reports',
                  style: TextStyle(color: colorScheme.onSurface)),
              trailing: Icon(Icons.chevron_right,
                  color: colorScheme.onSurfaceVariant),
              onTap: () {},
            ),

            const SizedBox(height: 30),

            // Save Button
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 20),
              child: SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: _isSaving ? null : _saveSettings,
                  child: _isSaving
                      // Show a loading spinner while saving
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
                          'Save Preferences',
                          style: TextStyle(
                              fontSize: 16, fontWeight: FontWeight.w600),
                        ),
                ),
              ),
            ),
            const SizedBox(height: 30),
          ],
        ),
      ),
    );
  }
}