import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../viewmodels/attendance_view_model.dart';
import '../viewmodels/parent_view_model.dart';

// Displays the list of subjects and their attendance registers
class AttendanceScreen extends StatefulWidget {
  const AttendanceScreen({super.key});

  @override
  State<AttendanceScreen> createState() => _AttendanceScreenState();
}

class _AttendanceScreenState extends State<AttendanceScreen> {
  @override
  void initState() {
    super.initState();
    // Use addPostFrameCallback to safely access context after the widget builds
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final learner = context.read<ParentViewModel>().learner;
      if (learner != null) {
        // Load attendance data for the currently selected learner
        context.read<AttendanceViewModel>().load(learnerId: learner.id);
      } else {
        debugPrint('No learner loaded - attendance cannot fetch');
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      appBar: AppBar(
        title: const Text('Attendance'),
      ),
      body: Consumer<AttendanceViewModel>(
        builder: (context, vm, _) {
          // Show a loading spinner if data is being fetched and no subjects are loaded yet
          if (vm.isLoading && vm.subjects.isEmpty) {
            return const Center(child: CircularProgressIndicator());
          }

          return Column(
            children: [
              // Header section showing the screen title
              Container(
                width: double.infinity,
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
                color: colorScheme.primary.withValues(alpha: 0.12),
                child: Row(
                  children: [
                    Icon(Icons.calendar_today,
                        color: colorScheme.primary, size: 18),
                    const SizedBox(width: 8),
                    Text(
                      'Attendance Register',
                      style: TextStyle(
                        fontSize: 14,
                        fontWeight: FontWeight.w600,
                        color: colorScheme.primary,
                      ),
                    ),
                  ],
                ),
              ),

              // List
              Expanded(
                child: vm.subjects.isEmpty
                    ? Center(
                        child: Text(
                          'No data available',
                          style: TextStyle(color: colorScheme.onSurfaceVariant),
                        ),
                      )
                    : ListView(
                        children: [
                          // ⭐ 1. DAILY CLASS REGISTER (Fixed with Material) ⭐
                          Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                            child: Material(
                              color: colorScheme.primary.withValues(alpha: 0.05),
                              borderRadius: BorderRadius.circular(12),
                              child: InkWell(
                                borderRadius: BorderRadius.circular(12),
                                onTap: () {
                                  final learner = context.read<ParentViewModel>().learner;
                                  if (learner == null) return;

                                  // Passing 'DAILY_ONLY' tells the ViewModel to strictly filter out subjects
                                  vm.selectSubject(
                                    learnerId: learner.id,
                                    subjectId: 'DAILY_ONLY',
                                    subjectName: 'Daily Class',
                                  );
                                  _showCalendarSheet(context, vm, 'Daily Class');
                                },
                                child: Container(
                                  padding: const EdgeInsets.all(16),
                                  decoration: BoxDecoration(
                                    borderRadius: BorderRadius.circular(12),
                                    border: Border.all(color: colorScheme.primary.withValues(alpha: 0.2)),
                                  ),
                                  child: Row(
                                    children: [
                                      Icon(Icons.groups, color: colorScheme.primary),
                                      const SizedBox(width: 16),
                                      Expanded(
                                        child: Column(
                                          crossAxisAlignment: CrossAxisAlignment.start,
                                          children: [
                                            Text(
                                              'Daily Class Register',
                                              style: TextStyle(
                                                fontWeight: FontWeight.bold,
                                                color: colorScheme.primary,
                                                fontSize: 14,
                                              ),
                                            ),
                                            const SizedBox(height: 2),
                                            Text(
                                              'Overall daily attendance marked by class teacher',
                                              style: TextStyle(
                                                color: colorScheme.onSurfaceVariant,
                                                fontSize: 12,
                                              ),
                                            ),
                                          ],
                                        ),
                                      ),
                                      Icon(Icons.arrow_forward_ios, size: 14, color: colorScheme.primary),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                          ),

                          const SizedBox(height: 8),
                          Padding(
                            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                            child: Text(
                              'Subject Registers',
                              style: TextStyle(
                                fontSize: 13,
                                fontWeight: FontWeight.bold,
                                color: colorScheme.onSurfaceVariant,
                              ),
                            ),
                          ),

                          // ⭐ 2. SUBJECT REGISTERS ⭐
                          ...vm.subjects.map((subject) {
                            return _SubjectRow(
                              subject: subject.subjectName,
                              onView: () {
                                final learner =
                                    context.read<ParentViewModel>().learner;
                                if (learner == null) return;

                                vm.selectSubject(
                                  learnerId: learner.id,
                                  subjectId: subject.subjectId,
                                  subjectName: subject.subjectName,
                                );
                                _showCalendarSheet(
                                  context,
                                  vm,
                                  subject.subjectName,
                                );
                              },
                            );
                          }).toList(),
                        ],
                      ),
              ),
            ],
          );
        },
      ),
    );
  }

  // Bottom sheet with calendar and month navigation
  void _showCalendarSheet(
    BuildContext context,
    AttendanceViewModel vm,
    String subject,
  ) {
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
        builder: (_, scrollController) => Consumer<AttendanceViewModel>(
          builder: (context, vm2, _) {
            final learner = context.read<ParentViewModel>().learner;
            final colorScheme = Theme.of(context).colorScheme;

            return ListView(
              controller: scrollController,
              padding: const EdgeInsets.symmetric(horizontal: 16),
              children: [
                // Drag handle indicator
                Center(
                  child: Container(
                    width: 40,
                    height: 4,
                    margin: const EdgeInsets.symmetric(vertical: 12),
                    decoration: BoxDecoration(
                      color: colorScheme.outlineVariant,
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),

                // Subject title
                Text(
                  '$subject Attendance',
                  style: TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                    color: colorScheme.onSurface,
                  ),
                ),
                const SizedBox(height: 6),

                // Month navigation
                Container(
                  margin: const EdgeInsets.symmetric(vertical: 8),
                  padding:
                      const EdgeInsets.symmetric(horizontal: 4, vertical: 4),
                  decoration: BoxDecoration(
                    color: colorScheme.surfaceContainerHighest,
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      // Previous month button
                      IconButton(
                        icon: Icon(Icons.chevron_left,
                            color: colorScheme.primary),
                        onPressed: () {
                          if (learner != null) {
                            vm2.previousMonth(learnerId: learner.id);
                          }
                        },
                      ),
                      // Current month and year label
                      Text(
                        vm2.monthLabel,
                        style: TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: colorScheme.onSurface,
                        ),
                      ),
                      // Next month button (disabled if at the current month)
                      IconButton(
                        icon: Icon(
                          Icons.chevron_right,
                          color: vm2.canGoForward
                              ? colorScheme.primary
                              : colorScheme.outline,
                        ),
                        onPressed: vm2.canGoForward
                            ? () {
                                if (learner != null) {
                                  vm2.nextMonth(learnerId: learner.id);
                                }
                              }
                            : null,
                      ),
                    ],
                  ),
                ),

                const SizedBox(height: 8),

                // Show loading indicator or the calendar content
                if (vm2.isLoading)
                  const Padding(
                    padding: EdgeInsets.all(20),
                    child: Center(child: CircularProgressIndicator()),
                  )
                else ...[
                  // Present, Absent, Late, and Excused summary boxes
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      _statBox('Present', vm2.presentCount, Colors.green),
                      _statBox('Absent', vm2.absentCount, Colors.red),
                      _statBox('Late', vm2.lateCount, Colors.orange),
                      _statBox('Excused', vm2.excusedCount, Colors.blue),
                    ],
                  ),
                  const SizedBox(height: 14),
                  
                  // Legend explaining all 4 calendar colors
                  Wrap(
                    spacing: 16,
                    runSpacing: 8,
                    children: [
                      _legend(Colors.green, 'Present'),
                      _legend(Colors.red, 'Absent'),
                      _legend(Colors.orange, 'Late'),
                      _legend(Colors.blue, 'Excused'),
                    ],
                  ),
                  const SizedBox(height: 12),
                  
                  // The calendar grid
                  _buildCalendar(context, vm2),
                  const SizedBox(height: 24),
                ],
              ],
            );
          },
        ),
      ),
    );
  }

  // Creates a summary box displaying a count with a specific color
  Widget _statBox(String label, int count, Color color) {
    return Expanded(
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 2),
        padding: const EdgeInsets.symmetric(vertical: 8),
        decoration: BoxDecoration(
          color: color.withValues(alpha: 0.1),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Column(
          children: [
            Text(
              '$count',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            const SizedBox(height: 2),
            Text(
              label,
              style: TextStyle(
                fontSize: 10,
                fontWeight: FontWeight.w600,
                color: color,
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Creates a legend item (color dot and text)
  Widget _legend(Color c, String label) {
    final colorScheme = Theme.of(context).colorScheme;
    return Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Container(
          width: 10,
          height: 10,
          decoration: BoxDecoration(color: c, shape: BoxShape.circle),
        ),
        const SizedBox(width: 4),
        Text(label,
            style:
                TextStyle(fontSize: 11, color: colorScheme.onSurfaceVariant)),
      ],
    );
  }

  // Builds the monthly calendar grid
  Widget _buildCalendar(BuildContext context, AttendanceViewModel vm) {
    final colorScheme = Theme.of(context).colorScheme;
    const dayNames = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'];
    final month = vm.month;
    
    // Calculate the total days in the month and the starting day of the week
    final daysInMonth = DateTime(month.year, month.month + 1, 0).day;
    final firstWeekday = DateTime(month.year, month.month, 1).weekday % 7;

    final cells = <Widget>[];

    // Add the headers for the days of the week
    for (final d in dayNames) {
      cells.add(Container(
        alignment: Alignment.center,
        padding: const EdgeInsets.symmetric(vertical: 6),
        child: Text(
          d,
          style: TextStyle(
            fontSize: 11,
            fontWeight: FontWeight.bold,
            color: colorScheme.onSurfaceVariant,
          ),
        ),
      ));
    }

    // Add empty cells for the days before the first day of the month
    for (int i = 0; i < firstWeekday; i++) {
      cells.add(const SizedBox());
    }

    // Add a cell for each day of the month
    for (int day = 1; day <= daysInMonth; day++) {
      // Check the status dynamically
      final status = vm.dayStatus[day];
      
      Color bg = colorScheme.surfaceContainerHighest;
      Color fg = colorScheme.onSurface;
      String symbol = '';

      // Override colors if attendance is marked
      if (status != null) {
        final normalized = status.toString().toLowerCase();
        if (normalized == 'present' || normalized == 'true') {
          bg = Colors.green.withValues(alpha: 0.15);
          fg = Colors.green;
          symbol = '✓';
        } else if (normalized == 'absent' || normalized == 'false') {
          bg = Colors.red.withValues(alpha: 0.15);
          fg = Colors.red;
          symbol = 'X';
        } else if (normalized == 'late') {
          bg = Colors.orange.withValues(alpha: 0.15);
          fg = Colors.orange;
          symbol = 'L';
        } else if (normalized == 'excused') {
          bg = Colors.blue.withValues(alpha: 0.15);
          fg = Colors.blue;
          symbol = 'E';
        }
      }

      cells.add(Container(
        margin: const EdgeInsets.all(2),
        alignment: Alignment.center,
        decoration: BoxDecoration(
          color: bg,
          borderRadius: BorderRadius.circular(6),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(
              '$day',
              style: TextStyle(
                fontSize: 12,
                fontWeight: FontWeight.w600,
                color: fg,
              ),
            ),
            if (symbol.isNotEmpty)
              Text(
                symbol,
                style: TextStyle(
                  fontSize: 10,
                  color: fg,
                  fontWeight: FontWeight.bold,
                ),
              ),
          ],
        ),
      ));
    }

    // Return the grid view containing all the cells
    return GridView.count(
      crossAxisCount: 7,
      shrinkWrap: true,
      physics: const NeverScrollableScrollPhysics(),
      childAspectRatio: 0.85,
      children: cells,
    );
  }
}

// Subject Row (Fixed with Material to prevent crash)
class _SubjectRow extends StatelessWidget {
  final String subject;
  final VoidCallback onView;

  const _SubjectRow({
    required this.subject,
    required this.onView,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 2),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: colorScheme.outlineVariant),
      ),
      child: Material(
        color: Colors.transparent,
        child: InkWell(
          borderRadius: BorderRadius.circular(12),
          onTap: onView,
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
            child: Row(
              children: [
                Expanded(
                  child: Text(
                    subject,
                    style: TextStyle(
                      fontSize: 14,
                      fontWeight: FontWeight.w600,
                      color: colorScheme.onSurface,
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
                  decoration: BoxDecoration(
                    color: colorScheme.primary.withValues(alpha: 0.12),
                    borderRadius: BorderRadius.circular(20),
                  ),
                  child: Text(
                    'View Register',
                    style: TextStyle(
                      fontSize: 11,
                      color: colorScheme.primary,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}