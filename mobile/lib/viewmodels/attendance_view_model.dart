/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */

import 'package:flutter/material.dart';
import '../models/attendance.dart';
import '../models/grade.dart';
import '../services/supabase_service.dart';

// Manages the state and logic for viewing attendance records
class AttendanceViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor requires the Supabase service to fetch data
  AttendanceViewModel(this._api);

  // Internal state variables
  List<SubjectGrade> _subjects = [];
  List<AttendanceRecord> _records = [];
  DateTime _month = DateTime.now();
  
  String? _selectedSubjectId;
  String? _selectedSubjectName;
  
  bool _isLoading = false;
  String? _error;

  // Getters to expose state to the UI
  List<SubjectGrade> get subjects => _subjects;
  List<AttendanceRecord> get records => _records;
  DateTime get month => _month;
  String? get selectedSubjectId => _selectedSubjectId;
  String? get selectedSubjectName => _selectedSubjectName;
  bool get isLoading => _isLoading;
  String? get error => _error;

  // Filters the records to only include those from the currently selected month
  List<AttendanceRecord> get _currentMonthRecords {
    return _records.where((r) => 
      r.date.year == _month.year && r.date.month == _month.month
    ).toList();
  }

  // Returns a dynamic map to handle String statuses (Present, Absent, Late, Excused)
  Map<int, dynamic> get dayStatus => {
        for (final r in _currentMonthRecords) r.date.day: r.status,
      };

  // Calculate counts for all 4 statuses
  int get presentCount => _currentMonthRecords.where((r) => 
      r.status.toString().toLowerCase() == 'present' || r.status == true).length;

  int get absentCount => _currentMonthRecords.where((r) => 
      r.status.toString().toLowerCase() == 'absent' || r.status == false).length;

  int get lateCount => _currentMonthRecords.where((r) => 
      r.status.toString().toLowerCase() == 'late').length;

  int get excusedCount => _currentMonthRecords.where((r) => 
      r.status.toString().toLowerCase() == 'excused').length;

  int get totalMarked => _currentMonthRecords.length; 

  // Month Navigation
  bool get canGoForward {
    final now = DateTime.now();
    return _month.year < now.year ||
        (_month.year == now.year && _month.month < now.month);
  }

  String get monthLabel {
    const names = [
      'January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December',
    ];
    return '${names[_month.month - 1]} ${_month.year}';
  }

  // Load Subjects
  Future<void> load({required String learnerId}) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _subjects = await _api.fetchSubjectsOnly(learnerId: learnerId);
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  // Select Subject or Daily Register
  Future<void> selectSubject({
    required String learnerId,
    required String subjectId,
    required String subjectName,
  }) async {
    _selectedSubjectId = subjectId.isEmpty ? null : subjectId;
    _selectedSubjectName = subjectName;
    _records = [];
    _isLoading = true;
    notifyListeners();

    try {
      if (subjectId == 'DAILY_ONLY') {
        // ⭐ DAILY ONLY: Fetch strictly where subject_id is NULL
        final response = await _api.client
            .from('attendance')
            .select('id, attendance_date, status, check_in_time')
            .eq('learner_id', learnerId)
            .isFilter('subject_id', null) // The magic filter
            .gte('attendance_date', "${_month.year}-${_month.month.toString().padLeft(2, '0')}-01")
            .lt('attendance_date', DateTime(_month.year, _month.month + 1, 1).toIso8601String().substring(0, 10))
            .order('attendance_date');

        _records = response.map((e) => AttendanceRecord.fromJson(e)).toList();
      } else {
        // Normal Subject Fetch
        _records = await _api.fetchAttendance(
          learnerId: learnerId,
          month: _month,
          subjectId: _selectedSubjectId,
        );
      }
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  // Navigate Months
  Future<void> previousMonth({required String learnerId}) async {
    _month = DateTime(_month.year, _month.month - 1);
    notifyListeners();
    await _reloadMonth(learnerId);
  }

  Future<void> nextMonth({required String learnerId}) async {
    if (!canGoForward) return;
    _month = DateTime(_month.year, _month.month + 1);
    notifyListeners();
    await _reloadMonth(learnerId);
  }

  Future<void> _reloadMonth(String learnerId) async {
    _isLoading = true;
    _records = [];
    notifyListeners();

    try {
      if (_selectedSubjectId == 'DAILY_ONLY') {
        // ⭐ DAILY ONLY: Fetch strictly where subject_id is NULL
        final response = await _api.client
            .from('attendance')
            .select('id, attendance_date, status, check_in_time')
            .eq('learner_id', learnerId)
            .isFilter('subject_id', null) // The magic filter
            .gte('attendance_date', "${_month.year}-${_month.month.toString().padLeft(2, '0')}-01")
            .lt('attendance_date', DateTime(_month.year, _month.month + 1, 1).toIso8601String().substring(0, 10))
            .order('attendance_date');

        _records = response.map((e) => AttendanceRecord.fromJson(e)).toList();
      } else {
        // Normal Subject Fetch
        _records = await _api.fetchAttendance(
          learnerId: learnerId,
          month: _month,
          subjectId: _selectedSubjectId,
        );
      }
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  void reset() {
    _subjects = [];
    _records = [];
    _month = DateTime.now();
    _selectedSubjectId = null;
    _selectedSubjectName = null;
    _isLoading = false;
    _error = null;
    notifyListeners();
  }
}