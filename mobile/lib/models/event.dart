/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */

class SchoolEvent {
  final String id;
  final String name;
  final String? description;
  final DateTime date;
  final String? startTime;
  final String? endTime;
  final String? venue;

  const SchoolEvent({
    required this.id,
    required this.name,
    this.description,
    required this.date,
    this.startTime,
    this.endTime,
    this.venue,
  });

  String get day => date.day.toString().padLeft(2, '0');
  
  String get month {
    const months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    return months[date.month - 1];
  }

  String get timeRange {
    if (startTime == null || startTime!.isEmpty) return 'All Day';
    // Safely extract hours and minutes
    final s = startTime!.length >= 5 ? startTime!.substring(0, 5) : startTime!;
    final e = (endTime != null && endTime!.length >= 5) ? endTime!.substring(0, 5) : '';
    return e.isEmpty ? s : '$s - $e';
  }

  factory SchoolEvent.fromJson(Map<String, dynamic> json) {
    // Safely parse the date, handling both String and DateTime from Supabase
    DateTime parsedDate;
    try {
      if (json['event_date'] is DateTime) {
        parsedDate = json['event_date'] as DateTime;
      } else if (json['event_date'] is String) {
        parsedDate = DateTime.parse(json['event_date']);
      } else {
        parsedDate = DateTime.now(); // Fallback
      }
    } catch (_) {
      parsedDate = DateTime.now(); // Fallback if parsing fails
    }

    return SchoolEvent(
      id: json['id']?.toString() ?? '',
      name: json['event_name']?.toString() ?? 'Untitled Event',
      description: json['description']?.toString(),
      date: parsedDate,
      startTime: json['start_time']?.toString(),
      endTime: json['end_time']?.toString(),
      venue: json['venue']?.toString(),
    );
  }
}