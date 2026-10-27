import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../theme.dart';
import '../viewmodels/announcement_view_model.dart';
import '../viewmodels/attendance_view_model.dart';
import '../viewmodels/event_view_model.dart';
import '../viewmodels/grades_view_model.dart';
import '../viewmodels/parent_view_model.dart';
import '../viewmodels/report_view_model.dart';
import 'announcement_screen.dart';
import 'dashboard_screen.dart';
import 'event_screen.dart';
import 'reports_screen.dart';

// Main shell with bottom navigation bar that hosts the app's primary screens
class MainShell extends StatefulWidget {
  const MainShell({super.key});

  @override
  State<MainShell> createState() => _MainShellState();
}

class _MainShellState extends State<MainShell> {
  // Tracks the currently selected bottom navigation tab
  int _index = 0;
  
  // Keeps track of the last loaded learner to avoid unnecessary reloads
  String? _lastLoadedLearnerId;

  @override
  void initState() {
    super.initState();
    // Load initial data after the first frame is rendered
    WidgetsBinding.instance.addPostFrameCallback((_) => _loadAll());
  }

  // Loads parent data and all dependent data for the currently selected learner
  Future<void> _loadAll() async {
    final parentVM = context.read<ParentViewModel>();
    
    // Only load the parent if we haven't already
    if (parentVM.allLearners.isEmpty) {
      await parentVM.load();
    }

    if (!mounted) return;
    final learner = parentVM.learner;
    
    // Exit early if no learner is available
    if (learner == null) {
      debugPrint('[MainShell] No learner loaded. Skipping data fetch.');
      return;
    }

    // If the learner has not changed, skip reloading to save resources
    if (_lastLoadedLearnerId == learner.id) {
      debugPrint('[MainShell] Same learner (${learner.fullName}). Not reloading.');
      return;
    }

    // Learner changed, so clear old data from all ViewModels
    debugPrint('[MainShell] Learner changed to ${learner.fullName}. Reloading data...');
    
    context.read<AttendanceViewModel>().reset();
    context.read<GradesViewModel>().reset();
    context.read<ReportViewModel>().reset();
    context.read<EventViewModel>().reset();

    // Update the tracked learner ID
    _lastLoadedLearnerId = learner.id;

    // Load new data for the newly selected learner in parallel
    await Future.wait([
      context.read<AttendanceViewModel>().load(learnerId: learner.id),
      context.read<GradesViewModel>().load(learnerId: learner.id),
      context.read<ReportViewModel>().load(
            learnerId: learner.id,
            learnerName: learner.fullName,
          ),
      context.read<EventViewModel>().load(learnerGrade: learner.grade),
      context.read<AnnouncementViewModel>().load(),
    ]);
    
    debugPrint('[MainShell] All data loaded for ${learner.fullName}');
  }

  @override
  Widget build(BuildContext context) {
    // Watch the learner ID so we can detect when it changes
    final currentLearnerId = context.select<ParentViewModel, String?>(
      (vm) => vm.learner?.id,
    );

    // Trigger a reload if the learner has changed since the last load
    if (currentLearnerId != null && currentLearnerId != _lastLoadedLearnerId) {
      WidgetsBinding.instance.addPostFrameCallback((_) => _loadAll());
    }

    // Watch the unread count to display a badge on the announcements tab
    final unread =
        context.select<AnnouncementViewModel, int>((vm) => vm.unreadCount);

    // The list of screens corresponding to each bottom navigation tab
    const screens = [
      DashboardScreen(),
      AnnouncementsScreen(),
      EventsScreen(),
      ReportsScreen(),
    ];

    return Scaffold(
      // Display the currently selected screen
      body: screens[_index],
      
      // Bottom navigation bar
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _index,
        onTap: (i) => setState(() => _index = i),
        type: BottomNavigationBarType.fixed,
        selectedItemColor: AppColors.primary,
        unselectedItemColor: AppColors.mutedText,
        selectedFontSize: 11,
        unselectedFontSize: 11,
        items: [
          // Home tab
          const BottomNavigationBarItem(
              icon: Icon(Icons.home_outlined),
              activeIcon: Icon(Icons.home),
              label: 'Home'),
          
          // Announcements tab with unread badge
          BottomNavigationBarItem(
            icon: Stack(
              children: [
                const Icon(Icons.campaign_outlined),
                // Show a red badge if there are unread announcements
                if (unread > 0)
                  Positioned(
                    right: 0,
                    top: 0,
                    child: Container(
                      padding: const EdgeInsets.all(3),
                      decoration: const BoxDecoration(
                        color: AppColors.red,
                        shape: BoxShape.circle,
                      ),
                      child: Text(
                        '$unread',
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 8,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
            activeIcon: const Icon(Icons.campaign),
            label: 'Announcements',
          ),
          
          // Events tab
          const BottomNavigationBarItem(
              icon: Icon(Icons.calendar_today_outlined),
              activeIcon: Icon(Icons.calendar_today),
              label: 'Events'),
          
          // Reports tab
          const BottomNavigationBarItem(
              icon: Icon(Icons.description_outlined),
              activeIcon: Icon(Icons.description),
              label: 'Reports'),
        ],
      ),
    );
  }
}