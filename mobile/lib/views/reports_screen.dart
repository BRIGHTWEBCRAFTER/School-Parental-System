import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';

import '../viewmodels/report_view_model.dart';
import '../viewmodels/parent_view_model.dart';

// Screen that displays the learner's academic report files
class ReportsScreen extends StatefulWidget {
  const ReportsScreen({super.key});

  @override
  State<ReportsScreen> createState() => _ReportsScreenState();
}

class _ReportsScreenState extends State<ReportsScreen> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      final learner = context.read<ParentViewModel>().learner;
      if (learner != null) {
        context.read<ReportViewModel>().load(
          learnerId: learner.id,
          learnerName: learner.fullName,
        );
      }
    });
  }

  Future<void> _openReport(String url) async {
    final uri = Uri.parse(url);
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } else {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Could not open the PDF file.')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final vm = context.watch<ReportViewModel>();
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      backgroundColor: Theme.of(context).scaffoldBackgroundColor,
      appBar: AppBar(
        title: const Text('Reports'),
        automaticallyImplyLeading: true,
      ),
      body: vm.isLoading
          ? const Center(child: CircularProgressIndicator())
          : vm.filteredReports.isEmpty
              ? Center(
                  child: Padding(
                    padding: const EdgeInsets.all(32),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(Icons.folder_open, size: 64, color: colorScheme.outline),
                        const SizedBox(height: 16),
                        Text(
                          'No reports available yet',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                            color: colorScheme.onSurfaceVariant,
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Reports will appear here once the school uploads them.',
                          textAlign: TextAlign.center,
                          style: TextStyle(fontSize: 13, color: colorScheme.outline),
                        ),
                      ],
                    ),
                  ),
                )
              : ListView(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  children: [
                    // Header banner
                    Container(
                      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                      color: colorScheme.primary.withValues(alpha: 0.12),
                      child: Row(
                        children: [
                          Icon(Icons.description, color: colorScheme.primary, size: 18),
                          const SizedBox(width: 8),
                          Text(
                            "Your Child's Academic Reports",
                            style: TextStyle(
                              fontSize: 14,
                              fontWeight: FontWeight.w600,
                              color: colorScheme.primary,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 10),

                    // Filter dropdowns
                    if (vm.availableYears.isNotEmpty) ...[
                      Padding(
                        padding: const EdgeInsets.symmetric(horizontal: 16),
                        child: Row(
                          children: [
                            Expanded(
                              child: _dropdown(
                                context: context,
                                label: 'Year',
                                value: vm.selectedYear?.toString() ?? 'All',
                                items: ['All', ...vm.availableYears.map((y) => y.toString())],
                                onChanged: (v) => vm.setYear(v == 'All' ? null : int.tryParse(v!)),
                              ),
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: _dropdown(
                                context: context,
                                label: 'Term',
                                value: vm.selectedTerm ?? 'All',
                                items: ['All', ...vm.availableTerms],
                                onChanged: (v) => vm.setTerm(v == 'All' ? null : v),
                              ),
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(height: 10),
                    ],

                    // List of report files
                    ...vm.filteredReports.map((r) => _ReportTile(
                      report: r,
                      onDownload: () => _openReport(r.fileUrl ?? ''),
                    )),
                  ],
                ),
    );
  }

  Widget _dropdown({
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
        color: Theme.of(context).cardColor,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: colorScheme.outlineVariant),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(label, style: TextStyle(fontSize: 10, color: colorScheme.onSurfaceVariant)),
          DropdownButton<String>(
            value: value,
            isExpanded: true,
            underline: const SizedBox(),
            dropdownColor: Theme.of(context).cardColor,
            icon: Icon(Icons.arrow_drop_down, color: colorScheme.primary),
            items: items.map((e) => DropdownMenuItem(
              value: e,
              child: Text(e, style: TextStyle(color: colorScheme.onSurface)),
            )).toList(),
            onChanged: onChanged,
          ),
        ],
      ),
    );
  }
}

// Individual report file tile
class _ReportTile extends StatelessWidget {
  final dynamic report;
  final VoidCallback onDownload;

  const _ReportTile({required this.report, required this.onDownload});

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 5),
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: Theme.of(context).cardColor,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: colorScheme.outlineVariant),
      ),
      child: Row(
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: BoxDecoration(
              color: colorScheme.error.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(10),
            ),
            alignment: Alignment.center,
            child: Icon(Icons.picture_as_pdf, color: colorScheme.error, size: 24),
          ),
          const SizedBox(width: 14),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  report.fileName,
                  style: TextStyle(
                    fontSize: 14,
                    fontWeight: FontWeight.w600,
                    color: colorScheme.onSurface,
                  ),
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
                const SizedBox(height: 4),
                Text(
                  '${report.term} • ${report.year}',
                  style: TextStyle(fontSize: 12, color: colorScheme.onSurfaceVariant),
                ),
              ],
            ),
          ),
          IconButton(
            icon: Icon(Icons.download, color: colorScheme.primary),
            onPressed: onDownload,
          ),
        ],
      ),
    );
  }
}