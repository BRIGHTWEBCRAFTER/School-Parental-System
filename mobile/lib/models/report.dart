/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */

// Data model representing an academic report file
class ReportFile {
  // Unique identifier for the report
  final String id;

  // The ID of the learner this report belongs to
  final String learnerId;

  // The academic year of the report
  final int year;

  // The academic term of the report (e.g., "Term 1")
  final String term;

  // The display name of the file
  final String fileName;

  // The URL to download or view the file (optional)
  final String? fileUrl;

  // When the file was uploaded (optional)
  final DateTime? uploadedAt;

  // The overall letter grade on the report (optional)
  final String? gradeLetter;

  // Constructor with required fields for the main content
  const ReportFile({
    required this.id,
    required this.learnerId,
    required this.year,
    required this.term,
    required this.fileName,
    this.fileUrl,
    this.uploadedAt,
    this.gradeLetter,
  });

  // Getter that combines the term and year into a single label
  String get label => '$term $year';

  // Getter that returns the year as a string
  String get yearLabel => year.toString();

  // Getter that returns the term
  String get termLabel => term;

  // Factory constructor that parses a grade row from Supabase
  factory ReportFile.fromGradeRow(Map<String, dynamic> json) {
    // Extract the learner's name from the joined data
    final learnerName =
        (json['learners'] as Map<String, dynamic>?)?['full_name'] as String? ??
            'Learner';

    // Parse the year and term with fallbacks
    final year =
        (json['academic_year'] as num?)?.toInt() ?? DateTime.now().year;
    final term = (json['academic_term'] as String?) ?? 'Term 1';

    return ReportFile(
      id: json['id'] as String,
      learnerId: json['learner_id'] as String,
      year: year,
      term: term,
      // Generate a display name based on the learner and term info
      fileName: '$learnerName - $term $year.pdf',
      fileUrl: json['report_url'] as String?,
      uploadedAt: json['updated_at'] != null
          ? DateTime.tryParse(json['updated_at'] as String)
          : null,
      gradeLetter: json['grade_letter'] as String?,
    );
  }

  // Factory constructor for creating a ReportFile from a storage entry
  factory ReportFile.fromStorageEntry({
    required String learnerId,
    required String learnerName,
    required int year,
    required String term,
    required String publicUrl,
    DateTime? uploadedAt,
  }) {
    return ReportFile(
      // Construct a unique ID from the learner, year, and term
      id: '$learnerId-$year-$term',
      learnerId: learnerId,
      year: year,
      term: term,
      fileName: '$learnerName - $term $year.pdf',
      fileUrl: publicUrl,
      uploadedAt: uploadedAt,
    );
  }

  // Override the default toString method for easier debugging
  @override
  String toString() => 'ReportFile($fileName)';
}
