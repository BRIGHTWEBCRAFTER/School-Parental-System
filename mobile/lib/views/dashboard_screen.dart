import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:supabase_flutter/supabase_flutter.dart';
import '../theme.dart';
import '../viewmodels/parent_view_model.dart';
import '../viewmodels/announcement_view_model.dart';
import '../models/learner.dart';
import 'announcement_screen.dart';
import 'attandance_screen.dart';
import 'event_screen.dart';
import 'grades_view_screen.dart';
import 'learner_profile_screen.dart';
import 'profile_screen.dart';
import 'reports_screen.dart';
import 'parent_notifications_page.dart';

// Main landing page displaying parent info, learner selection, and feature shortcuts
class DashboardScreen extends StatelessWidget {
  const DashboardScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Access the theme color scheme for dynamic styling
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      body: SafeArea(
        child: Consumer<ParentViewModel>(
          builder: (context, vm, _) {
            // Show a loading spinner until parent data is available
            if (vm.isLoading || vm.parent == null) {
              return const Center(child: CircularProgressIndicator());
            }
            
            // Extract parent and learner data from the ViewModel
            final parent = vm.parent!;
            final learner = vm.learner;
            final profilePicture = parent.profilePicture;

            // ⭐ PULL-TO-REFRESH: Allows parents to swipe down to fetch the latest data
            return RefreshIndicator(
              onRefresh: () async {
                await vm.load(); // ⭐ FIXED: Using your actual load() method
                await context.read<AnnouncementViewModel>().load();
              },
              child: SingleChildScrollView(
                physics: const AlwaysScrollableScrollPhysics(),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // TOP BAR
                    Container(
                      padding:
                          const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
                      color: colorScheme.primary,
                      child: Row(
                        children: [
                          // App logo container
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

                          // ⭐ MANUAL REFRESH BUTTON ⭐
                          IconButton(
                            icon: const Icon(Icons.refresh),
                            color: colorScheme.onPrimary,
                            tooltip: 'Refresh Data',
                            onPressed: () async {
                              await vm.load(); // ⭐ FIXED: Using your actual load() method
                              if (!context.mounted) return;
                              await context.read<AnnouncementViewModel>().load();
                              if (!context.mounted) return;
                              
                              ScaffoldMessenger.of(context).showSnackBar(
                                const SnackBar(
                                  content: Text('Data refreshed'),
                                  duration: Duration(seconds: 1),
                                ),
                              );
                            },
                          ),

                          // ⭐ NOTIFICATION BELL WITH UNREAD BADGE ⭐
                          StreamBuilder<List<Map<String, dynamic>>>(
                            stream: Supabase.instance.client
                                .from('notifications')
                                .stream(primaryKey: ['id'])
                                .eq('user_id', Supabase.instance.client.auth.currentUser?.id ?? '')
                                .eq('is_read', false),
                            builder: (context, snapshot) {
                              final unreadCount = snapshot.data?.length ?? 0;
                              
                              return Stack(
                                children: [
                                  IconButton(
                                    icon: const Icon(Icons.notifications_outlined),
                                    color: colorScheme.onPrimary,
                                    tooltip: 'View Notifications',
                                    onPressed: () {
                                      Navigator.push(
                                        context,
                                        MaterialPageRoute(
                                          builder: (context) => const ParentNotificationsPage(),
                                        ),
                                      );
                                    },
                                  ),
                                  if (unreadCount > 0)
                                    Positioned(
                                      right: 8,
                                      top: 8,
                                      child: Container(
                                        padding: const EdgeInsets.all(4),
                                        decoration: const BoxDecoration(
                                          color: Colors.red,
                                          shape: BoxShape.circle,
                                        ),
                                        constraints: const BoxConstraints(
                                          minWidth: 16,
                                          minHeight: 16,
                                        ),
                                        child: Text(
                                          '$unreadCount',
                                          style: const TextStyle(
                                            color: Colors.white,
                                            fontSize: 10,
                                            fontWeight: FontWeight.bold,
                                          ),
                                          textAlign: TextAlign.center,
                                        ),
                                      ),
                                    ),
                                ],
                              );
                            },
                          ),
                          const SizedBox(width: 8),

                          // Profile button
                          GestureDetector(
                            onTap: () => Navigator.push(
                              context,
                              MaterialPageRoute(
                                  builder: (_) => const ProfileScreen()),
                            ),
                            child: Container(
                              width: 38,
                              height: 38,
                              decoration: BoxDecoration(
                                color: colorScheme.onPrimary.withValues(alpha: 0.2),
                                shape: BoxShape.circle,
                              ),
                              clipBehavior: Clip.antiAlias,
                              alignment: Alignment.center,
                              child: profilePicture != null &&
                                      profilePicture.isNotEmpty
                                  ? Image.network(
                                      profilePicture,
                                      fit: BoxFit.cover,
                                      errorBuilder: (context, error, stackTrace) {
                                        return Icon(
                                          Icons.person,
                                          color: colorScheme.onPrimary,
                                          size: 22,
                                        );
                                      },
                                    )
                                  : Icon(
                                      Icons.person,
                                      color: colorScheme.onPrimary,
                                      size: 22,
                                    ),
                            ),
                          ),
                        ],
                      ),
                    ),

                    // Welcome message
                    Padding(
                      padding: const EdgeInsets.fromLTRB(16, 18, 16, 6),
                      child: Text(
                        'Welcome ${parent.fullName}',
                        style: TextStyle(
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                          color: colorScheme.onSurface,
                        ),
                      ),
                    ),

                    // CHILD SWITCHER
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 16),
                      child: vm.hasMultipleChildren
                          ? Container(
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 12, vertical: 0),
                              decoration: BoxDecoration(
                                color: colorScheme.surfaceContainerHighest,
                                borderRadius: BorderRadius.circular(10),
                                border:
                                    Border.all(color: colorScheme.outlineVariant),
                              ),
                              child: DropdownButtonHideUnderline(
                                child: DropdownButton<Learner>(
                                  value: vm.learner,
                                  isExpanded: true,
                                  dropdownColor:
                                      colorScheme.surfaceContainerHighest,
                                  icon: Icon(Icons.arrow_drop_down,
                                      color: colorScheme.primary),
                                  onChanged: (Learner? newLearner) {
                                    if (newLearner != null) {
                                      vm.selectLearner(newLearner);
                                    }
                                  },
                                  items: vm.allLearners.map((child) {
                                    return DropdownMenuItem<Learner>(
                                      value: child,
                                      child: Text(
                                        '${child.fullName} - ${child.grade}',
                                        style: TextStyle(
                                          fontSize: 13,
                                          color: colorScheme.primary,
                                          fontWeight: FontWeight.w600,
                                        ),
                                      ),
                                    );
                                  }).toList(),
                                ),
                              ),
                            )
                          : Text(
                              learner != null
                                  ? 'Child: ${learner.fullName} - ${learner.grade}'
                                  : 'No learner linked',
                              style: TextStyle(
                                  fontSize: 13,
                                  color: colorScheme.primary,
                                  fontWeight: FontWeight.w600),
                            ),
                    ),
                    const SizedBox(height: 14),

                    // School card
                    if (learner != null)
                      GestureDetector(
                        onTap: () => Navigator.push(
                          context,
                          MaterialPageRoute(
                              builder: (_) => const LearnerProfileScreen()),
                        ),
                        child: Container(
                          margin: const EdgeInsets.symmetric(horizontal: 16),
                          padding: const EdgeInsets.all(12),
                          decoration: BoxDecoration(
                            color: colorScheme.surfaceContainerHighest,
                            borderRadius: BorderRadius.circular(12),
                            border: Border.all(color: colorScheme.outlineVariant),
                          ),
                          child: Row(
                            children: [
                              // School logo or fallback text
                              Container(
                                width: 46,
                                height: 46,
                                decoration: BoxDecoration(
                                  color: (learner.schoolLogoUrl != null &&
                                          learner.schoolLogoUrl!.isNotEmpty)
                                      ? Colors.transparent
                                      : colorScheme.primary,
                                  borderRadius: BorderRadius.circular(8),
                                ),
                                clipBehavior: Clip.antiAlias,
                                alignment: Alignment.center,
                                child: (learner.schoolLogoUrl != null &&
                                        learner.schoolLogoUrl!.isNotEmpty)
                                    ? Image.network(
                                        learner.schoolLogoUrl!,
                                        fit: BoxFit.cover,
                                        errorBuilder:
                                            (context, error, stackTrace) {
                                          return Text(
                                            'SCHOOL',
                                            textAlign: TextAlign.center,
                                            style: TextStyle(
                                              color: colorScheme.onPrimary,
                                              fontWeight: FontWeight.bold,
                                              fontSize: 9,
                                            ),
                                          );
                                        },
                                      )
                                    : Text(
                                        'SCHOOL',
                                        textAlign: TextAlign.center,
                                        style: TextStyle(
                                          color: colorScheme.onPrimary,
                                          fontWeight: FontWeight.bold,
                                          fontSize: 9,
                                        ),
                                      ),
                              ),
                              const SizedBox(width: 12),
                              
                              // School name and subtitle
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(learner.schoolName,
                                        style: TextStyle(
                                            fontSize: 14,
                                            color: colorScheme.primary,
                                            fontWeight: FontWeight.bold)),
                                    const SizedBox(height: 2),
                                    Text('View school information',
                                        style: TextStyle(
                                            fontSize: 11,
                                            color: colorScheme.onSurfaceVariant)),
                                  ],
                                ),
                              ),
                              Icon(Icons.arrow_forward,
                                  color: colorScheme.primary),
                            ],
                          ),
                        ),
                      ),
                    const SizedBox(height: 16),

                    // Feature grid
                    Padding(
                      padding: const EdgeInsets.symmetric(horizontal: 16),
                      child: GridView.count(
                        crossAxisCount: 2,
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        crossAxisSpacing: 12,
                        mainAxisSpacing: 12,
                        childAspectRatio: 1.0,
                        children: [
                          // Attendance tile
                          _tile(
                              context,
                              Icons.check_circle,
                              AppColors.green,
                              const Color(0xFFE3F6E8),
                              'Attendance',
                              'View attendance', () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (_) => const AttendanceScreen()));
                          }),
                          
                          // Grades tile
                          _tile(
                              context,
                              Icons.school,
                              const Color(0xFF673AB7),
                              const Color(0xFFEDE7F6),
                              'Grades',
                              'View grades', () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (_) => const GradesScreen()));
                          }),
                          
                          // Reports tile
                          _tile(
                              context,
                              Icons.description,
                              colorScheme.primary,
                              colorScheme.primary.withValues(alpha: 0.12),
                              'Reports',
                              'View reports', () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (_) => const ReportsScreen()));
                          }),
                          
                          // Events tile
                          _tile(
                              context,
                              Icons.calendar_month,
                              AppColors.yellow,
                              const Color(0xFFFDF6E0),
                              'Events',
                              'View events', () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (_) => const EventsScreen()));
                          }),
                          
                          // Announcements tile
                          _tile(
                              context,
                              Icons.campaign,
                              AppColors.pink,
                              const Color(0xFFFDE8EC),
                              'Announcements',
                              'View updates', () {
                            Navigator.push(
                                context,
                                MaterialPageRoute(
                                    builder: (_) => const AnnouncementsScreen()));
                          }),
                        ],
                      ),
                    ),

                    const SizedBox(height: 20),

                    // Recent updates section
                    _recentUpdatesSection(context),

                    const SizedBox(height: 30),
                  ],
                ),
              ),
            );
          },
        ),
      ),
    );
  }

  // Builds the "Recent Updates" section using data from the AnnouncementViewModel
  Widget _recentUpdatesSection(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    return Consumer<AnnouncementViewModel>(
      builder: (context, announcements, _) {
        // Take the top 3 most recent announcements
        final recent = announcements.items.take(3).toList();

        return Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Section header with "View All" link
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    'Recent Updates',
                    style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: colorScheme.onSurface,
                    ),
                  ),
                  GestureDetector(
                    onTap: () => Navigator.push(
                      context,
                      MaterialPageRoute(
                          builder: (_) => const AnnouncementsScreen()),
                    ),
                    child: Text(
                      'View All',
                      style: TextStyle(
                        fontSize: 13,
                        color: colorScheme.primary,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),
            
            // Show placeholder if no announcements exist
            if (recent.isEmpty)
              Container(
                margin: const EdgeInsets.symmetric(horizontal: 16),
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: colorScheme.surfaceContainerHighest,
                  borderRadius: BorderRadius.circular(10),
                  border: Border(
                    left: BorderSide(color: colorScheme.primary, width: 4),
                  ),
                ),
                child: Text(
                  'No recent updates',
                  style: TextStyle(
                      fontSize: 13, color: colorScheme.onSurfaceVariant),
                ),
              )
            // Otherwise, render a small tile for each recent announcement
            else
              ...recent.map((a) => _updateTile(
                    context,
                    a.title,
                    a.timeAgo,
                    unread: !a.isRead,
                  )),
          ],
        );
      },
    );
  }

  // Builds an individual tile for the recent updates list
  Widget _updateTile(BuildContext context, String title, String timeAgo,
      {bool unread = false}) {
    final colorScheme = Theme.of(context).colorScheme;

    return GestureDetector(
      onTap: () => Navigator.push(
        context,
        MaterialPageRoute(builder: (_) => const AnnouncementsScreen()),
      ),
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
        padding: const EdgeInsets.all(14),
        decoration: BoxDecoration(
          color: colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.circular(10),
          border: Border(
            left: BorderSide(color: colorScheme.primary, width: 4),
          ),
        ),
        child: Row(
          children: [
            // Icon container
            Container(
              width: 34,
              height: 34,
              decoration: BoxDecoration(
                color: colorScheme.primary.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(8),
              ),
              alignment: Alignment.center,
              child: Icon(
                Icons.campaign_outlined,
                color: colorScheme.primary,
                size: 18,
              ),
            ),
            const SizedBox(width: 12),
            
            // Title and time
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    title,
                    style: TextStyle(
                      fontSize: 13,
                      fontWeight: unread ? FontWeight.bold : FontWeight.w600,
                      color: colorScheme.onSurface,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 2),
                  Text(
                    timeAgo,
                    style: TextStyle(
                      fontSize: 11,
                      color: colorScheme.onSurfaceVariant,
                    ),
                  ),
                ],
              ),
            ),
            
            // Unread indicator dot
            if (unread)
              Container(
                width: 8,
                height: 8,
                decoration: BoxDecoration(
                  color: colorScheme.error,
                  shape: BoxShape.circle,
                ),
              ),
          ],
        ),
      ),
    );
  }

  // Builds a generic feature tile used in the grid
  Widget _tile(BuildContext ctx, IconData icon, Color color, Color bg,
      String title, String sub, VoidCallback onTap) {
    final colorScheme = Theme.of(ctx).colorScheme;

    return GestureDetector(
      onTap: onTap,
      child: Container(
        decoration: BoxDecoration(
          color: Theme.of(ctx).cardColor,
          borderRadius: BorderRadius.circular(14),
          border: Border.all(color: colorScheme.outlineVariant),
        ),
        padding: const EdgeInsets.all(12),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          mainAxisSize: MainAxisSize.min,
          children: [
            // Icon container with custom background color
            Container(
              width: 42,
              height: 42,
              decoration: BoxDecoration(
                  color: bg, borderRadius: BorderRadius.circular(12)),
              child: Icon(icon, color: color, size: 22),
            ),
            const SizedBox(height: 8),
            
            // Title text
            Flexible(
              child: FittedBox(
                fit: BoxFit.scaleDown,
                child: Text(
                  title,
                  style: TextStyle(
                    fontSize: 13,
                    fontWeight: FontWeight.bold,
                    color: colorScheme.onSurface,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
            ),
            const SizedBox(height: 2),
            
            // Subtitle text
            Flexible(
              child: FittedBox(
                fit: BoxFit.scaleDown,
                child: Text(
                  sub,
                  style: TextStyle(
                    fontSize: 10,
                    color: colorScheme.onSurfaceVariant,
                  ),
                  textAlign: TextAlign.center,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}