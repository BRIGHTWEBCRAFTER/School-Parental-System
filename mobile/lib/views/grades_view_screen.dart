import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../viewmodels/grades_view_model.dart';
import '../viewmodels/parent_view_model.dart';

// Screen displaying a list of subjects and their latest marks for the selected learner
class GradesScreen extends StatefulWidget {
  const GradesScreen({super.key});

  @override
  State<GradesScreen> createState() => _GradesScreenState();
}

class _GradesScreenState extends State<GradesScreen> {
  @override
  void initState() {
    super.initState();
    // Use addPostFrameCallback to safely access the context after the widget builds
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final learner = context.read<ParentViewModel>().learner;
      if (learner != null) {
        // Load the grades for the currently selected learner
        context.read<GradesViewModel>().load(learnerId: learner.id);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    // Access theme colors for dynamic styling
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      appBar: AppBar(title: const Text('Grades')),
      body: Consumer<GradesViewModel>(
        builder: (context, vm, _) {
          // Show a loading spinner while data is being fetched
          if (vm.isLoading) {
            return const Center(child: CircularProgressIndicator());
          }

          return Column(
            children: [
              // Header section
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(16),
                color: colorScheme.primary.withValues(alpha: 0.12),
                child: Text(
                  'Latest Subject Marks',
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: colorScheme.primary,
                  ),
                ),
              ),
              Expanded(
                // Display an empty state message if no subjects are found
                child: vm.subjects.isEmpty
                    ? Center(
                        child: Padding(
                          padding: const EdgeInsets.all(24.0),
                          child: Text(
                            'No subjects or grades found.\n\nPlease ensure grades are marked as "Published" in the database.',
                            textAlign: TextAlign.center,
                            style: TextStyle(
                              fontSize: 14,
                              color: colorScheme.onSurfaceVariant,
                            ),
                          ),
                        ),
                      )
                    // Otherwise, build the list of subject rows
                    : ListView.builder(
                        itemCount: vm.subjects.length,
                        itemBuilder: (context, i) {
                          final subject = vm.subjects[i];
                          final latest = vm.getLatestMark(subject.subjectId);

                          return _SubjectRow(
                            subject: subject.subjectName,
                            latestMark: latest,
                            onView: () => _showSubjectHistory(
                                context, vm, subject.subjectName, subject.subjectId),
                          );
                        },
                      ),
              ),
            ],
          );
        },
      ),
    );
  }

  // Displays a bottom sheet containing the assessment history for a specific subject
  void _showSubjectHistory(
      BuildContext context, GradesViewModel vm, String subject, String subjectId) {
    final learner = context.read<ParentViewModel>().learner;
    if (learner == null) return;

    // Trigger the ViewModel to load the history data
    vm.openSubjectHistory(learnerId: learner.id, subjectId: subjectId);

    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      backgroundColor: Theme.of(context).cardColor,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
      ),
      builder: (_) => DraggableScrollableSheet(
        expand: false,
        initialChildSize: 0.85,
        maxChildSize: 0.95,
        minChildSize: 0.5,
        builder: (_, scrollController) => Consumer<GradesViewModel>(
          builder: (context, vm2, _) {
            final colorScheme = Theme.of(context).colorScheme;
            
            return ListView(
              controller: scrollController,
              padding: const EdgeInsets.all(20),
              children: [
                // Drag handle indicator
                Center(
                  child: Container(
                    width: 40,
                    height: 4,
                    margin: const EdgeInsets.only(bottom: 16),
                    decoration: BoxDecoration(
                      color: colorScheme.outlineVariant,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),
                
                // Subject title
                Text(
                  subject,
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: colorScheme.onSurface,
                  ),
                ),
                const SizedBox(height: 16),

                // Filters for year and term
                if (vm2.subjectHistory.isNotEmpty) ...[
                  Row(
                    children: [
                      Expanded(
                        child: _buildDropdown(
                          context: context,
                          label: 'Year',
                          value: vm2.selectedYear ?? 'All',
                          items: vm2.availableYears,
                          onChanged: (v) {
                            if (v != null) {
                              vm2.setYear(v,
                                  learnerId: learner.id, subjectId: subjectId);
                            }
                          },
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: _buildDropdown(
                          context: context,
                          label: 'Term',
                          value: vm2.selectedTerm ?? 'All',
                          items: vm2.availableTerms,
                          onChanged: (v) {
                            if (v != null) {
                              vm2.setTerm(v,
                                  learnerId: learner.id, subjectId: subjectId);
                            }
                          },
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 20),
                ],

                // Section header for the assessments list
                Text('Assessments',
                    style: TextStyle(
                        fontSize: 15,
                        fontWeight: FontWeight.bold,
                        color: colorScheme.onSurface)),
                const SizedBox(height: 10),

                // Show loading spinner, empty message, or the list of assessments
                if (vm2.assessmentsLoading)
                  const Padding(
                    padding: EdgeInsets.all(20),
                    child: Center(child: CircularProgressIndicator()),
                  )
                else if (vm2.assessments.isEmpty)
                  Text('No assessments recorded for this term.',
                      style: TextStyle(color: colorScheme.onSurfaceVariant))
                else
                  ...vm2.assessments.map((a) => _AssessmentTile(
                        name: a.assessmentName,
                        type: a.assessmentType,
                        score: a.scoreInt,
                        term: a.term,
                        gradeLetter: a.gradeLetter,
                      )),
                const SizedBox(height: 20),
              ],
            );
          },
        ),
      ),
    );
  }

  // Helper widget to build a styled dropdown filter
  Widget _buildDropdown({
    required BuildContext context,
    required String label,
    required String value,
    required List<String> items,
    required ValueChanged<String?> onChanged,
  }) {
    final colorScheme = Theme.of(context).colorScheme;

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 4),
      decoration: BoxDecoration(
        color: colorScheme.surfaceContainerHighest,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: colorScheme.outlineVariant),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Small label above the dropdown
          Text(label,
              style: TextStyle(
                  fontSize: 10, color: colorScheme.onSurfaceVariant)),
          DropdownButton<String>(
            value: value,
            isExpanded: true,
            underline: const SizedBox(),
            dropdownColor: Theme.of(context).cardColor,
            icon: Icon(Icons.arrow_drop_down, color: colorScheme.primary),
            items: items
                .map((e) => DropdownMenuItem(
                      value: e,
                      child: Text(e,
                          style: TextStyle(color: colorScheme.onSurface)),
                    ))
                .toList(),
            onChanged: onChanged,
          ),
        ],
      ),
    );
  }
}

// Subject Row
// A widget representing a single subject with its latest mark and a view button
class _SubjectRow extends StatelessWidget {
  final String subject;
  final dynamic latestMark;
  final VoidCallback onView;

  const _SubjectRow({
    required this.subject,
    required this.latestMark,
    required this.onView,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    int score = latestMark?.scoreInt ?? 0;
    Color color = _colorForScore(score, colorScheme);

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        border: Border(bottom: BorderSide(color: colorScheme.outlineVariant)),
      ),
      child: Row(
        children: [
          // Subject name and latest assessment info
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(subject,
                    style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: colorScheme.onSurface)),
                const SizedBox(height: 4),
                if (latestMark != null)
                  Text(
                    'Latest: ${latestMark!.assessmentName} (${latestMark!.assessmentType})',
                    style: TextStyle(
                        fontSize: 12, color: colorScheme.onSurfaceVariant),
                  )
                else
                  Text('No marks recorded',
                      style: TextStyle(
                          fontSize: 12, color: colorScheme.outline)),
              ],
            ),
          ),

          // Score badge
          if (latestMark != null)
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                color: color.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(20),
              ),
              child: Text(
                '${latestMark!.scoreInt}%',
                style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.bold,
                    color: color),
              ),
            ),

          const SizedBox(width: 12),
          
          // View button
          GestureDetector(
            onTap: onView,
            child: Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
              decoration: BoxDecoration(
                color: colorScheme.primary.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(20),
              ),
              child: Text(
                'View',
                style: TextStyle(
                    fontSize: 11,
                    color: colorScheme.primary,
                    fontWeight: FontWeight.w600),
              ),
            ),
          ),
        ],
      ),
    );
  }

  // Determines the color to use based on the score
  Color _colorForScore(int score, ColorScheme scheme) {
    if (score >= 80) return scheme.primary;
    if (score >= 60) return scheme.primary;
    if (score >= 40) return scheme.tertiary;
    return scheme.error;
  }
}

// Assessment Tile
// A widget representing a single assessment record in the history sheet
class _AssessmentTile extends StatelessWidget {
  final String name;
  final String type;
  final int score;
  final String term;
  final String? gradeLetter;

  const _AssessmentTile({
    required this.name,
    required this.type,
    required this.score,
    required this.term,
    this.gradeLetter,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    final color = _colorForScore(score, colorScheme);

    return Container(
      margin: const EdgeInsets.only(bottom: 10),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: colorScheme.outlineVariant),
      ),
      child: Row(
        children: [
          // Icon container based on assessment type
          Container(
            width: 40,
            height: 40,
            decoration: BoxDecoration(
              color: color.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(8),
            ),
            alignment: Alignment.center,
            child: Icon(_iconForType(type), color: color, size: 20),
          ),
          const SizedBox(width: 12),
          
          // Assessment name, type, and term
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(name,
                    style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: colorScheme.onSurface)),
                const SizedBox(height: 4),
                Row(
                  children: [
                    // Type badge
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 2),
                      decoration: BoxDecoration(
                        color: colorScheme.primary.withValues(alpha: 0.12),
                        borderRadius: BorderRadius.circular(20),
                      ),
                      child: Text(
                        type,
                        style: TextStyle(
                            fontSize: 10,
                            color: colorScheme.primary,
                            fontWeight: FontWeight.w600),
                      ),
                    ),
                    const SizedBox(width: 8),
                    // Term text
                    Text(term,
                        style: TextStyle(
                            fontSize: 11, color: colorScheme.outline)),
                  ],
                ),
              ],
            ),
          ),
          
          // Score and grade letter
          Column(
            crossAxisAlignment: CrossAxisAlignment.end,
            children: [
              Text('$score%',
                  style: TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                      color: color)),
              if (gradeLetter != null && gradeLetter!.isNotEmpty)
                Text(gradeLetter!,
                    style: TextStyle(
                        fontSize: 11, color: colorScheme.onSurfaceVariant)),
            ],
          ),
        ],
      ),
    );
  }

  // Returns an icon based on the assessment type
  IconData _iconForType(String type) {
    final t = type.toLowerCase();
    if (t.contains('exam')) return Icons.assignment_late;
    if (t.contains('test')) return Icons.assignment;
    if (t.contains('assignment')) return Icons.edit_note;
    return Icons.school;
  }

  // Determines the color to use based on the score
  Color _colorForScore(int score, ColorScheme scheme) {
    if (score >= 80) return scheme.primary;
    if (score >= 60) return scheme.primary;
    if (score >= 40) return scheme.tertiary;
    return scheme.error;
  }
}