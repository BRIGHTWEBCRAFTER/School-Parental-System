import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../viewmodels/announcement_view_model.dart';

// Displays the full details of a specific announcement
class AnnouncementDetailScreen extends StatefulWidget {
  final String title;
  final String body;
  final String? announcementId; 
  final bool isUrgent; 
  final bool isAlreadyAcknowledged;

  // Constructor allows passing details from the previous screen
  const AnnouncementDetailScreen({
    super.key,
    required this.title,
    required this.body,
    this.announcementId,
    this.isUrgent = false, 
    this.isAlreadyAcknowledged = false, 
  });

  @override
  State<AnnouncementDetailScreen> createState() => _AnnouncementDetailScreenState();
}

class _AnnouncementDetailScreenState extends State<AnnouncementDetailScreen> {
  // Tracks whether the user has acknowledged this announcement
  late bool _isAcknowledged;
  
  // Tracks the loading state while saving the acknowledgment
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    // Initialize the acknowledgment state from the passed parameter
    _isAcknowledged = widget.isAlreadyAcknowledged; 
  }

  // Handles the logic when the user taps the acknowledge button
  Future<void> _handleAcknowledge() async {
    // Prevent action if there is no valid ID
    if (widget.announcementId == null) return;
    
    setState(() => _isLoading = true);

    try {
      // Call the ViewModel to save the acknowledgment to the database
      await context
          .read<AnnouncementViewModel>()
          .acknowledgeAnnouncement(widget.announcementId!);

      if (!mounted) return;
      
      // Update the UI to show the acknowledged state
      setState(() {
        _isAcknowledged = true;
        _isLoading = false;
      });

      // Show a success message
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Announcement acknowledged. Thank you!'),
          backgroundColor: Colors.green,
        ),
      );
    } catch (e) {
      if (!mounted) return;
      
      // Handle any errors that occur during the save process
      setState(() => _isLoading = false);
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error: $e'), backgroundColor: Colors.red),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Announcement')),
      body: Column(
        children: [
          // Expanded section for the scrollable announcement content
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.all(20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Urgent Badge (only shows if the announcement is marked urgent)
                  if (widget.isUrgent)
                    Container(
                      margin: const EdgeInsets.only(bottom: 12),
                      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                      decoration: BoxDecoration(
                        color: colorScheme.errorContainer,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: Text(
                        'IMPORTANT / URGENT',
                        style: TextStyle(
                          fontSize: 11,
                          fontWeight: FontWeight.bold,
                          color: colorScheme.onErrorContainer,
                        ),
                      ),
                    ),

                  // Announcement title and body
                  Text(widget.title,
                      style: TextStyle(
                          fontSize: 20, 
                          fontWeight: FontWeight.bold,
                          color: colorScheme.onSurface)),
                  const SizedBox(height: 12),
                  Text(widget.body,
                      style: TextStyle(
                          fontSize: 14, 
                          height: 1.6,
                          color: colorScheme.onSurfaceVariant)),
                ],
              ),
            ),
          ),

          // Acknowledge Button section (only shows if the announcement is urgent)
          if (widget.isUrgent)
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: theme.cardColor,
                border: Border(top: BorderSide(color: theme.dividerColor)),
              ),
              child: SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  // Disable the button if already acknowledged or currently loading
                  onPressed: (_isAcknowledged || _isLoading) ? null : _handleAcknowledge,
                  style: ElevatedButton.styleFrom(
                    // Change button color based on acknowledgment state
                    backgroundColor: _isAcknowledged 
                        ? colorScheme.surfaceContainerHighest 
                        : colorScheme.primary,
                    foregroundColor: _isAcknowledged 
                        ? colorScheme.onSurfaceVariant 
                        : colorScheme.onPrimary,
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
                  ),
                  child: _isLoading
                      // Show a loading spinner while saving
                      ? const SizedBox(
                          width: 22, height: 22,
                          child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2.5),
                        )
                      // Show the button label and icon based on state
                      : Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(_isAcknowledged ? Icons.check_circle : Icons.touch_app, 
                                size: 20),
                            const SizedBox(width: 8),
                            Text(
                              _isAcknowledged ? 'Acknowledged' : 'Acknowledge Receipt',
                              style: const TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                            ),
                          ],
                        ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}