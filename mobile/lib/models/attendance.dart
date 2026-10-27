/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */

// Data model representing a single attendance record for a learner
class AttendanceRecord {
  // Unique identifier for the attendance record
  final String id;
  
  // The date the attendance was recorded
  final DateTime date;
  
  // The attendance status (e.g., Present, Absent, Late, Excused)
  final String status;
  
  // The time the learner checked in, if applicable
  final String? checkInTime;
  
  // Any additional notes or remarks about the attendance
  final String? remarks;

  // Constructor with required fields for the main content
  const AttendanceRecord({
    required this.id,
    required this.date,
    required this.status,
    this.checkInTime,
    this.remarks,
  });

  // Convenience getter to check if the learner was present
  bool get isPresent => status.toLowerCase() == 'present';

  // Factory constructor that parses the Supabase JSON response
  factory AttendanceRecord.fromJson(Map<String, dynamic> json) {
    // Parse the date safely
    final rawDate = json['attendance_date'] as String?;
    DateTime parsedDate;
    
    if (rawDate != null) {
      // If it's a full timestamp, DateTime.parse handles it correctly
      parsedDate = DateTime.parse(rawDate);
    } else {
      // Fallback to the current date if no date is provided
      parsedDate = DateTime.now();
    }

    return AttendanceRecord(
      id: json['id'] as String? ?? '',
      date: parsedDate,
      status: json['status'] as String? ?? 'Absent',
      checkInTime: json['check_in_time'] as String?,
      remarks: json['remarks'] as String?,
    );
  }
}