/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */



// Data model representing an overall grade for a subject
class SubjectGrade {
  // Unique identifier for the subject
  final String subjectId;
  
  // Display name of the subject
  final String subjectName;
  
  // The average or overall percentage for this subject
  final double percentage;
  
  // The academic term (e.g., "Term 1")
  final String term;
  
  // The academic year
  final int year;

  // Constructor with default values for all fields
  const SubjectGrade({
    this.subjectId = '',
    this.subjectName = '',
    this.percentage = 0,
    this.term = 'Term 1',
    this.year = 2026,
  });

  // Getter that rounds the percentage to the nearest integer
  int get percentageInt => percentage.round();
}

// Data model representing a single assessment for a subject
class SubjectAssessment {
  // Unique identifier for the assessment record
  final String id;
  
  // Name of the subject this assessment belongs to
  final String subjectName;
  
  // Name of the specific assessment (e.g., "Mid-Term Test")
  final String assessmentName;
  
  // Type of assessment (e.g., Test, Exam, Assignment)
  final String assessmentType;
  
  // The score achieved
  final double score;
  
  // The academic term
  final String term;
  
  // The academic year
  final int year;
  
  // Optional letter grade (e.g., A, B+)
  final String? gradeLetter;
  
  // Optional teacher remarks
  final String? remarks;

  // Constructor with required fields for the main content
  const SubjectAssessment({
    required this.id,
    required this.subjectName,
    required this.assessmentName,
    required this.assessmentType,
    required this.score,
    required this.term,
    required this.year,
    this.gradeLetter,
    this.remarks,
  });

  // Getter that rounds the score to the nearest integer
  int get scoreInt => score.round();

  // Factory constructor that parses the joined Supabase response
  factory SubjectAssessment.fromJoin(Map<String, dynamic> json) {
    // Extract the nested 'subjects' object from the join
    final subjects = json['subjects'] as Map<String, dynamic>?;
    
    return SubjectAssessment(
      id: json['id'] as String? ?? '',
      subjectName: subjects?['name'] as String? ?? '',
      assessmentName: json['assessment_name'] as String? ?? '',
      assessmentType: json['assessment_type'] as String? ?? 'Test',
      score: (json['score'] as num?)?.toDouble() ?? 0,
      term: json['academic_term'] as String? ?? 'Term 1',
      year: (json['academic_year'] as num?)?.toInt() ?? 2026,
      gradeLetter: json['grade_letter'] as String?,
      remarks: json['remarks'] as String?,
    );
  }
}