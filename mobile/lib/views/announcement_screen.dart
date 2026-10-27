import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../viewmodels/announcement_view_model.dart';
import 'announcement_detail_screen.dart';

// Displays a list of all announcements received by the parent
class AnnouncementsScreen extends StatelessWidget {
  const AnnouncementsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Watch the ViewModel for state changes
    final vm = context.watch<AnnouncementViewModel>();
    
    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Announcements'),
        automaticallyImplyLeading: true,
      ),
      
      // Show loading indicator, empty state, or list based on ViewModel state
      body: vm.isLoading
          ? const Center(child: CircularProgressIndicator())
          : vm.items.isEmpty
              ? Center(
                  child: Text(
                    'No announcements',
                    style: TextStyle(color: colorScheme.onSurfaceVariant),
                  ),
                )
              : ListView.builder(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  itemCount: vm.items.length,
                  itemBuilder: (context, i) {
                    // Get the current announcement item
                    final a = vm.items[i];
                    
                    // Determine if the announcement is urgent based on its priority
                    final isUrgent = a.priority == 'High' || a.priority == 'Urgent';

                    return GestureDetector(
                      onTap: () {
                        // Mark the announcement as read when tapped
                        vm.markRead(i);
                        
                        // Navigate to the detail screen, passing required data
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) => AnnouncementDetailScreen(
                              title: a.title,
                              body: a.content,
                              announcementId: a.id,
                              isUrgent: isUrgent,
                              isAlreadyAcknowledged: a.isAcknowledged,
                            ),
                          ),
                        );
                      },
                      child: Container(
                        margin: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 7),
                        padding: const EdgeInsets.all(14),
                        decoration: BoxDecoration(
                          // Use theme card color for background
                          color: theme.cardColor, 
                          borderRadius: BorderRadius.circular(12),
                          // Use theme divider color for the border
                          border: Border.all(color: theme.dividerColor), 
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                // Announcement title (bold if unread)
                                Expanded(
                                  child: Text(
                                    a.title,
                                    style: TextStyle(
                                      fontSize: 15,
                                      fontWeight: a.isRead
                                          ? FontWeight.w600
                                          : FontWeight.bold,
                                      color: colorScheme.onSurface,
                                    ),
                                  ),
                                ),
                                // Time ago text
                                Text(
                                  a.timeAgo,
                                  style: TextStyle(
                                    fontSize: 11,
                                    color: colorScheme.onSurfaceVariant,
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 6),
                            // Announcement preview text
                            Text(
                              a.preview,
                              style: TextStyle(
                                fontSize: 12.5,
                                color: colorScheme.onSurfaceVariant,
                              ),
                            ),
                            // Unread badge (only shows if not read)
                            if (!a.isRead) ...[
                              const SizedBox(height: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 8, vertical: 3),
                                decoration: BoxDecoration(
                                  // Use error color for the unread badge
                                  color: colorScheme.errorContainer, 
                                  borderRadius: BorderRadius.circular(20),
                                ),
                                child: Text(
                                  'unread',
                                  style: TextStyle(
                                    fontSize: 10,
                                    color: colorScheme.onErrorContainer,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ],
                          ],
                        ),
                      ),
                    );
                  },
                ),
    );
  }
}