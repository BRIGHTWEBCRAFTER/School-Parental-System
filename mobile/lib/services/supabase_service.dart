/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */

import 'package:flutter/foundation.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

import '../models/parent.dart';
import '../models/learner.dart';
import '../models/grade.dart';
import '../models/attendance.dart';
import '../models/announcement.dart';
import '../models/event.dart';
import '../models/report.dart';

class SupabaseService {
  final SupabaseClient _client = Supabase.instance.client;

  SupabaseClient get client => _client;
  User? get currentUser => _client.auth.currentUser;
  bool get isAuthenticated => _client.auth.currentUser != null;

  // AUTH
  Future<bool> signInWithIdNumber({required String idNumber, required String accessCode}) async {
    try {
      final rows = await _client.from('users').select('email').eq('phone', idNumber).eq('role', 'Parent').limit(1);
      if (rows.isEmpty) return false;
      final email = rows.first['email'] as String?;
      if (email == null || email.isEmpty) return false;
      final res = await _client.auth.signInWithPassword(email: email, password: accessCode);
      return res.user != null;
    } catch (_) { return false; }
  }

  Future<bool> signInWithEmail({required String email, required String accessCode}) async {
    try {
      final res = await _client.auth.signInWithPassword(email: email, password: accessCode);
      return res.user != null;
    } catch (_) { return false; }
  }

  Future<void> signOut() async => _client.auth.signOut();

  // PARENT
  Future<ParentProfile?> fetchParentProfile() async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) return null;
    final data = await _client.from('users').select('''
      id, full_name, email, phone, role, profile_picture,
      parents(guardian_number, gender, alternative_phone, address)
    ''').eq('id', userId).maybeSingle();
    if (data == null) return null;
    return ParentProfile.fromJoin(data);
  }

  Future<void> updateParentInfo({required String fullName, required String phone}) async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) throw Exception('Not authenticated');
    await _client.from('users').update({'full_name': fullName, 'phone': phone}).eq('id', userId);
  }

  Future<void> updateEmail({required String currentPassword, required String newEmail}) async {
    final user = _client.auth.currentUser;
    if (user == null) throw Exception('Not authenticated');
    if (user.email == null) throw Exception('No current email');
    try {
      await _client.auth.signInWithPassword(email: user.email!, password: currentPassword);
    } catch (_) { throw Exception('Current password is incorrect'); }
    await _client.auth.updateUser(UserAttributes(email: newEmail));
    await _client.from('users').update({'email': newEmail}).eq('id', user.id);
  }

  Future<String?> updateProfilePicture({required List<int> bytes, required String extension}) async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) throw Exception('Not authenticated');
    final fileName = '$userId/profile_${DateTime.now().millisecondsSinceEpoch}.$extension';
    await _client.storage.from('avatars').uploadBinary(fileName, Uint8List.fromList(bytes), fileOptions: const FileOptions(upsert: true));
    final publicUrl = _client.storage.from('avatars').getPublicUrl(fileName);
    await _client.from('users').update({'profile_picture': publicUrl}).eq('id', userId);
    return publicUrl;
  }

  Future<void> acknowledgeAnnouncement({required String announcementId}) async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) return;
    await _client.from('announcement_recipients').update({
      'is_acknowledged': true, 'acknowledged_at': DateTime.now().toIso8601String(),
      'is_read': true, 'read_at': DateTime.now().toIso8601String(),
    }).eq('announcement_id', announcementId).eq('user_id', userId);
  }

  // LEARNERS
  Future<List<Learner>> fetchAllLearners() async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) return [];
    final parentRow = await _client.from('parents').select('id').eq('user_id', userId).maybeSingle();
    if (parentRow == null) return [];
    final rows = await _client.from('parent_learners').select('''
      learners!inner(
        id, admission_number, full_name, date_of_birth, gender, grade, address, is_active,
        schools(name, email, phone, address, logo_url),
        enrollments(is_active, classes(name))
      )
    ''').eq('parent_id', parentRow['id']);

    final learners = <Learner>[];
    for (final row in rows) {
      final learnerData = row['learners'];
      Map<String, dynamic>? json;
      if (learnerData is List && learnerData.isNotEmpty) {
        json = learnerData.first as Map<String, dynamic>?;
      } else if (learnerData is Map<String, dynamic>) {
        json = learnerData;
      }
      if (json == null) continue;
      try {
        learners.add(Learner.fromJson(json));
      } catch (e) { debugPrint('parse error $e'); }
    }
    learners.sort((a, b) => a.fullName.compareTo(b.fullName));
    return learners;
  }

  // SUBJECTS & GRADES
  Future<List<SubjectGrade>> fetchSubjectsOnly({required String learnerId}) async {
    try {
      final learnerRow = await _client.from('learners').select('school_id').eq('id', learnerId).maybeSingle();
      if (learnerRow == null) return [];
      final rows = await _client.from('subjects').select('id, name').eq('school_id', learnerRow['school_id']).order('name');
      return rows.map((e) => SubjectGrade(subjectId: e['id'], subjectName: e['name'], percentage: 0, term: 'Term 1', year: 2026)).toList();
    } catch (e) { return []; }
  }

  Future<List<SubjectGrade>> fetchSubjectGrades({required String learnerId}) async {
    try {
      final rows = await _client.from('grades').select('subject_id, score, academic_term, academic_year').eq('learner_id', learnerId).eq('publication_status', 'Published');
      if (rows.isEmpty) return [];
      final subjectIds = rows.map((r) => r['subject_id'] as String).toSet().toList();
      final subjectsData = await _client.from('subjects').select('id, name').inFilter('id', subjectIds);
      final subjectNames = {for (var s in subjectsData) s['id'] as String: s['name'] as String};
      final map = <String, SubjectGrade>{};
      for (final row in rows) {
        final id = row['subject_id'] as String;
        final name = subjectNames[id] ?? 'Unknown Subject';
        final score = (row['score'] as num?)?.toDouble() ?? 0.0;
        final term = row['academic_term'] as String? ?? 'Term 1';
        final year = (row['academic_year'] as num?)?.toInt() ?? 2026;
        if (map.containsKey(id)) {
          final old = map[id]!;
          map[id] = SubjectGrade(subjectId: id, subjectName: name, percentage: (old.percentage + score) / 2, term: term, year: year);
        } else {
          map[id] = SubjectGrade(subjectId: id, subjectName: name, percentage: score, term: term, year: year);
        }
      }
      return map.values.toList()..sort((a, b) => a.subjectName.compareTo(b.subjectName));
    } catch (e) { return []; }
  }

  Future<SubjectAssessment?> fetchLatestAssessment({required String learnerId, required String subjectId}) async {
    try {
      final rows = await _client.from('grades').select('''
        id, assessment_name, assessment_type, score, academic_term, academic_year, grade_letter, remarks
      ''').eq('learner_id', learnerId).eq('subject_id', subjectId).order('created_at', ascending: false).limit(1);
      if (rows.isEmpty) return null;
      final data = rows.first;
      final subjectRow = await _client.from('subjects').select('name').eq('id', subjectId).maybeSingle();
      return SubjectAssessment(
        id: data['id'] as String? ?? '', subjectName: subjectRow?['name'] as String? ?? 'Unknown',
        assessmentName: data['assessment_name'] as String? ?? '', assessmentType: data['assessment_type'] as String? ?? 'Test',
        score: (data['score'] as num?)?.toDouble() ?? 0, term: data['academic_term'] as String? ?? 'Term 1',
        year: (data['academic_year'] as num?)?.toInt() ?? 2026, gradeLetter: data['grade_letter'] as String?, remarks: data['remarks'] as String?,
      );
    } catch (e) { return null; }
  }

  Future<Map<String, Set<String>>> fetchSubjectHistory({required String learnerId, required String subjectId}) async {
    try {
      final rows = await _client.from('grades').select('academic_year, academic_term').eq('learner_id', learnerId).eq('subject_id', subjectId);
      final history = <String, Set<String>>{};
      for (final row in rows) {
        final year = row['academic_year'].toString();
        final term = row['academic_term'] as String;
        history.putIfAbsent(year, () => {}).add(term);
      }
      return history;
    } catch (e) { return {}; }
  }

  Future<List<SubjectAssessment>> fetchAssessments({required String learnerId, required String subjectId, int? year, String? term}) async {
    try {
      var query = _client.from('grades').select('''
        id, assessment_name, assessment_type, score, academic_term, academic_year, grade_letter, remarks
      ''').eq('learner_id', learnerId).eq('subject_id', subjectId);
      if (year != null) query = query.eq('academic_year', year);
      if (term != null) query = query.eq('academic_term', term);
      final rows = await query.order('created_at', ascending: false);
      final subjectRow = await _client.from('subjects').select('name').eq('id', subjectId).maybeSingle();
      final subjectName = subjectRow?['name'] as String? ?? 'Unknown Subject';
      return rows.map((e) => SubjectAssessment(
        id: e['id'] as String? ?? '', subjectName: subjectName, assessmentName: e['assessment_name'] as String? ?? '',
        assessmentType: e['assessment_type'] as String? ?? 'Test', score: (e['score'] as num?)?.toDouble() ?? 0,
        term: e['academic_term'] as String? ?? 'Term 1', year: (e['academic_year'] as num?)?.toInt() ?? 2026,
        gradeLetter: e['grade_letter'] as String?, remarks: e['remarks'] as String?,
      )).toList();
    } catch (e) { return []; }
  }

  // ATTENDANCE
  Future<List<AttendanceRecord>> fetchAttendance({required String learnerId, required DateTime month, String? subjectId}) async {
    final startStr = "${month.year}-${month.month.toString().padLeft(2, '0')}-01";
    final nextMonth = DateTime(month.year, month.month + 1, 1);
    final endStr = "${nextMonth.year}-${nextMonth.month.toString().padLeft(2, '0')}-01";
    var query = _client.from('attendance').select('id, attendance_date, status, check_in_time').eq('learner_id', learnerId).gte('attendance_date', startStr).lt('attendance_date', endStr);
    if (subjectId != null) query = query.eq('subject_id', subjectId);
    final rows = await query.order('attendance_date');
    return rows.map((e) => AttendanceRecord.fromJson(e)).toList();
  }

  // ANNOUNCEMENTS
  Future<List<Announcement>> fetchAnnouncements() async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) return [];
    final rows = await _client.from('announcement_recipients').select('''
      is_read, created_at, announcements(id, title, content, priority, created_at)
    ''').eq('user_id', userId).order('created_at', ascending: false);
    return rows.map((e) => Announcement.fromJoin(e)).toList();
  }

  Future<void> markAnnouncementRead({required String announcementId}) async {
    final userId = _client.auth.currentUser?.id;
    if (userId == null) return;
    await _client.from('announcement_recipients').update({'is_read': true, 'read_at': DateTime.now().toIso8601String()}).eq('announcement_id', announcementId).eq('user_id', userId);
  }

  // REPORTS
  Future<List<ReportFile>> fetchReports({required String learnerId, required String learnerName}) async {
    try {
      final rows = await _client.from('reports').select('id, academic_year, academic_term, file_path, file_name').eq('learner_id', learnerId).order('academic_year', ascending: false).order('academic_term', ascending: false);
      if (rows.isEmpty) return [];
      final reports = <ReportFile>[];

      // Recursively search the bucket from the root
      Future<String?> findPdfRecursively(String path) async {
        try {
          final items = await _client.storage.from('reports').list(path: path);
          for (final item in items) {
            if (item.name.toLowerCase().endsWith('.pdf')) return path.isEmpty ? item.name : '$path/${item.name}';
            if (!item.name.contains('.')) {
              final subPath = path.isEmpty ? item.name : '$path/${item.name}';
              final found = await findPdfRecursively(subPath);
              if (found != null) return found;
            }
          }
        } catch (e) { debugPrint('Search error at $path: $e'); }
        return null;
      }

      final String? actualFilePath = await findPdfRecursively('');

      if (actualFilePath != null) {
        debugPrint('Found PDF at: $actualFilePath');
        final signedUrlResponse = await _client.storage.from('reports').createSignedUrl(actualFilePath, 3600);
        for (final row in rows) {
          reports.add(ReportFile(
            id: row['id'] as String, learnerId: learnerId,
            year: (row['academic_year'] as num?)?.toInt() ?? 2026,
            term: row['academic_term'] as String? ?? 'Unknown Term',
            fileName: row['file_name'] as String? ?? 'Report.pdf',
            fileUrl: signedUrlResponse, uploadedAt: null,
          ));
        }
      } else {
        debugPrint('No PDF found in storage.');
      }
      return reports;
    } catch (e) { debugPrint('fetchReports error: $e'); return []; }
  }

  // EVENTS
  // ⭐ TEMPORARY: Removed date and target filters so ALL events show up
  Future<List<SchoolEvent>> fetchEvents({required String? learnerGrade}) async {
    try {
      final learners = await fetchAllLearners();
      if (learners.isEmpty) return [];
      
      final learnerRow = await _client.from('learners').select('school_id').eq('id', learners.first.id).maybeSingle();
      if (learnerRow == null) return [];
      
      // Removed the .gte('event_date', today) filter
      final rows = await _client.from('events')
          .select('''
            id, event_name, description, event_date, start_time, end_time, venue, target_type, target_role, target_grade
          ''')
          .eq('school_id', learnerRow['school_id'])
          .order('event_date');

      // Return ALL events without any target filtering
      return rows.map((json) => SchoolEvent.fromJson(json)).toList();
      
    } catch (e) {
      debugPrint('❌ fetchEvents error: $e');
      return [];
    }
  }
}