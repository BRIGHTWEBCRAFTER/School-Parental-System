/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */



// Data model representing a single learner linked to a parent
class Learner {
  // Unique identifier for the learner
  final String id;
  
  // The learner's admission number
  final String admissionNumber;
  
  // The learner's full name
  final String fullName;
  
  // The learner's date of birth (optional)
  final String? dateOfBirth;
  
  // The learner's gender (optional)
  final String? gender;
  
  // The grade the learner is currently in (e.g., "Grade 5")
  final String grade;
  
  // The learner's home address (optional)
  final String? address;
  
  // Whether the learner is currently active at the school
  final bool isActive;
  
  // The name of the learner's current class (optional)
  final String? className;

  // School information fields
  // The name of the school the learner is enrolled in
  final String schoolName;
  
  // The school's contact email (optional)
  final String? schoolEmail;
  
  // The school's contact phone number (optional)
  final String? schoolPhone;
  
  // The school's physical address (optional)
  final String? schoolAddress;
  
  // URL pointing to the school's logo image (optional)
  final String? schoolLogoUrl;

  // Constructor with required fields for the main content
  const Learner({
    required this.id,
    required this.admissionNumber,
    required this.fullName,
    this.dateOfBirth,
    this.gender,
    required this.grade,
    this.address,
    required this.isActive,
    this.className,
    required this.schoolName,
    this.schoolEmail,
    this.schoolPhone,
    this.schoolAddress,
    this.schoolLogoUrl,
  });

  // Convenience getter that returns a readable status string
  String get status => isActive ? 'Active' : 'Inactive';

  // Factory constructor that parses the nested Supabase JSON response
  factory Learner.fromJson(Map<String, dynamic> json) {
    // Extract the class name from the enrollments list
    String? className;
    final enrollments = json['enrollments'] as List<dynamic>?;
    if (enrollments != null && enrollments.isNotEmpty) {
      final first = enrollments.first as Map<String, dynamic>;
      final classes = first['classes'] as Map<String, dynamic>?;
      className = classes?['name'] as String?;
    }

    // Extract the school object from the JSON
    // Supabase usually returns embedded relations as a List, even for one-to-one relationships
    dynamic schoolData = json['schools'];
    Map<String, dynamic>? school;
    if (schoolData is List && schoolData.isNotEmpty) {
      school = schoolData.first as Map<String, dynamic>?;
    } else if (schoolData is Map<String, dynamic>) {
      school = schoolData;
    }

    return Learner(
      id: json['id'] as String? ?? '',
      admissionNumber: json['admission_number'] as String? ?? '',
      fullName: json['full_name'] as String? ?? '',
      dateOfBirth: json['date_of_birth'] as String?,
      gender: json['gender'] as String?,
      grade: json['grade'] as String? ?? '',
      address: json['address'] as String?,
      isActive: json['is_active'] as bool? ?? true,
      className: className,
      // Map the school fields from the extracted school object
      schoolName: school?['name'] as String? ?? '',
      schoolEmail: school?['email'] as String?,
      schoolPhone: school?['phone'] as String?,
      schoolAddress: school?['address'] as String?,
      schoolLogoUrl: school?['logo_url'] as String?,
    );
  }
}