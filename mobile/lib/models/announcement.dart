
/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */


// Data model representing a single announcement sent to a parent
class Announcement {
  // Unique identifier for the announcement
  final String id;
  
  // The title shown in the list and detail screen
  final String title;
  
  // The full body text of the announcement
  final String content;
  
  // Priority level (e.g., Normal, High, Urgent)
  final String priority;
  
  // When the announcement was originally created
  final DateTime createdAt;
  
  // Whether the current user has read the announcement
  final bool isRead;
  
  // Whether the current user has formally acknowledged the announcement
  final bool isAcknowledged;

  // Constructor with required fields for the main content
  Announcement({
    required this.id,
    required this.title,
    required this.content,
    required this.priority,
    required this.createdAt,
    required this.isRead,
    this.isAcknowledged = false,
  });

  // Factory constructor that parses the joined Supabase response
  factory Announcement.fromJoin(Map<String, dynamic> json) {
    // Extract the nested 'announcements' object from the join
    final a = json['announcements'] as Map<String, dynamic>? ?? {};
    
    return Announcement(
      id: a['id'] as String? ?? '',
      title: a['title'] as String? ?? '',
      content: a['content'] as String? ?? '',
      priority: a['priority'] as String? ?? 'Normal',
      createdAt:
          DateTime.tryParse(a['created_at'] as String? ?? '') ?? DateTime.now(),
      // These fields come from the 'announcement_recipients' row
      isRead: json['is_read'] as bool? ?? false,
      isAcknowledged: json['is_acknowledged'] as bool? ?? false,
    );
  }

  // Creates a new Announcement object with some fields replaced
  Announcement copyWith({
    String? id,
    String? title,
    String? content,
    String? priority,
    DateTime? createdAt,
    bool? isRead,
    bool? isAcknowledged,
  }) {
    return Announcement(
      id: id ?? this.id,
      title: title ?? this.title,
      content: content ?? this.content,
      priority: priority ?? this.priority,
      createdAt: createdAt ?? this.createdAt,
      isRead: isRead ?? this.isRead,
      isAcknowledged: isAcknowledged ?? this.isAcknowledged,
    );
  }

  // Getter that returns a shortened version of the content for previews
  String get preview {
    if (content.length > 50) {
      return '${content.substring(0, 50)}...';
    }
    return content;
  }

  // Getter that calculates a human-readable "time ago" string
  String get timeAgo {
    final diff = DateTime.now().difference(createdAt);
    if (diff.inDays > 0) return '${diff.inDays}d ago';
    if (diff.inHours > 0) return '${diff.inHours}h ago';
    if (diff.inMinutes > 0) return '${diff.inMinutes}m ago';
    return 'Just now';
  }
}