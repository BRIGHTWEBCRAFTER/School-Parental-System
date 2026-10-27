import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/learner.dart';
import '../viewmodels/parent_view_model.dart';

// Displays the learner's personal and school information using a two-tab layout
class LearnerProfileScreen extends StatelessWidget {
  const LearnerProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    // Select only the currently active learner from the ViewModel
    final learner =
        context.select<ParentViewModel, Learner?>((vm) => vm.learner);

    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    // Fallback UI if no learner is selected or available
    if (learner == null) {
      return Scaffold(
        body: Center(
          child: Text(
            'No learner data available',
            style: TextStyle(color: colorScheme.onSurface),
          ),
        ),
      );
    }

    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Learner Profile'),
          // Tab bar with two sections: Learner Info and School Info
          bottom: TabBar(
            labelColor: colorScheme.onPrimary,
            unselectedLabelColor: colorScheme.onPrimary.withValues(alpha: 0.7),
            indicatorColor: colorScheme.onPrimary,
            tabs: const [
              Tab(text: 'Learner Info', icon: Icon(Icons.person_outline)),
              Tab(text: 'School Info', icon: Icon(Icons.school_outlined)),
            ],
          ),
        ),
        body: TabBarView(
          children: [
            // TAB 1: LEARNER INFO
            SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Profile header with avatar, name, grade, and status
                  Container(
                    padding: const EdgeInsets.all(20),
                    color: colorScheme.surfaceContainerHighest,
                    child: Row(
                      children: [
                        // Avatar icon container
                        Container(
                          width: 74,
                          height: 74,
                          decoration: BoxDecoration(
                              color: colorScheme.primaryContainer,
                              shape: BoxShape.circle),
                          alignment: Alignment.center,
                          child: Icon(Icons.person,
                              size: 40, color: colorScheme.onPrimaryContainer),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              // Learner full name
                              Text(learner.fullName,
                                  style: TextStyle(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: colorScheme.onSurface)),
                              const SizedBox(height: 4),
                              Row(
                                children: [
                                  // Grade text
                                  Text(learner.grade,
                                      style: TextStyle(
                                          fontSize: 13,
                                          color: colorScheme.primary,
                                          fontWeight: FontWeight.w600)),
                                  const SizedBox(width: 10),
                                  // Status badge (e.g., Active)
                                  Container(
                                    padding: const EdgeInsets.symmetric(
                                        horizontal: 8, vertical: 3),
                                    decoration: BoxDecoration(
                                        color: colorScheme.tertiary,
                                        borderRadius:
                                            BorderRadius.circular(20)),
                                    child: Text(learner.status,
                                        style: TextStyle(
                                            fontSize: 10,
                                            color: colorScheme.onTertiary,
                                            fontWeight: FontWeight.w600)),
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                  
                  // Personal Information section
                  Padding(
                    padding: const EdgeInsets.fromLTRB(20, 20, 20, 8),
                    child: Text('Personal Information',
                        style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: colorScheme.onSurface)),
                  ),
                  _row(context, 'Admission No', learner.admissionNumber),
                  _row(context, 'Date of Birth', learner.dateOfBirth ?? '—'),
                  _row(context, 'Gender', learner.gender ?? '—'),
                  _row(context, 'Current Address', learner.address ?? '—'),
                  _row(context, 'Grade', learner.grade),
                  _row(context, 'Enrollment Status', learner.status),
                  const SizedBox(height: 30),
                ],
              ),
            ),

            // TAB 2: SCHOOL INFO
            SingleChildScrollView(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // School header with icon and name
                  Padding(
                    padding: const EdgeInsets.all(20),
                    child: Row(
                      children: [
                        Icon(Icons.school,
                            size: 40, color: colorScheme.primary),
                        const SizedBox(width: 16),
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text('School Details',
                                  style: TextStyle(
                                      fontSize: 18,
                                      fontWeight: FontWeight.bold,
                                      color: colorScheme.onSurface)),
                              Text(learner.schoolName,
                                  style: TextStyle(
                                      fontSize: 14,
                                      color: colorScheme.primary,
                                      fontWeight: FontWeight.w600)),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ),
                  Divider(color: theme.dividerColor),

                  // School contact information (email, phone, location)
                  _iconRow(context, Icons.email_outlined, 'Email Address',
                      learner.schoolEmail ?? 'Not available'),
                  _iconRow(context, Icons.phone_outlined, 'Phone Number',
                      learner.schoolPhone ?? 'Not available'),
                  _iconRow(context, Icons.location_on_outlined, 'Location',
                      learner.schoolAddress ?? 'Not available'),

                  const SizedBox(height: 10),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Simple row widget for displaying a label-value pair in the Learner Info tab
  Widget _row(BuildContext context, String k, String v) {
    final colorScheme = Theme.of(context).colorScheme;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Fixed-width label
          SizedBox(
              width: 130,
              child: Text(k,
                  style: TextStyle(
                      fontSize: 13, color: colorScheme.onSurfaceVariant))),
          // Flexible value
          Expanded(
              child: Text(v,
                  style: TextStyle(
                      fontSize: 13.5,
                      fontWeight: FontWeight.w600,
                      color: colorScheme.onSurface))),
        ],
      ),
    );
  }

  // Row widget with an icon for displaying contact information in the School Info tab
  Widget _iconRow(BuildContext context, IconData icon, String label, String value) {
    final colorScheme = Theme.of(context).colorScheme;

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 12),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          // Leading icon
          Icon(icon, color: colorScheme.primary, size: 22),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Small label text
                Text(label,
                    style: TextStyle(
                        fontSize: 12,
                        color: colorScheme.onSurfaceVariant)),
                const SizedBox(height: 2),
                // Main value text
                Text(value,
                    style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: colorScheme.onSurface)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}