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
import '/models/grade.dart';
import '/services/supabase_service.dart';

// Manages the state and logic for viewing grades and assessment history
class GradesViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor requires the Supabase service to fetch data
  GradesViewModel(this._api);

  // Internal state variables for the main grade list
  List<SubjectGrade> _allSubjects = [];
  final Map<String, SubjectAssessment?> _latestMarks = {};
  
  // State variables for the Subject History bottom sheet
  List<SubjectAssessment> _assessments = [];
  Map<String, Set<String>> _subjectHistory = {}; 
  
  String? _selectedYear;
  String? _selectedTerm;

  // Internal loading and error states
  bool _isLoading = false;
  bool _assessmentsLoading = false;
  String? _error;

  // Getters to expose state to the UI
  List<SubjectGrade> get subjects => _allSubjects;
  List<SubjectAssessment> get assessments => _assessments;
  Map<String, Set<String>> get subjectHistory => _subjectHistory;
  String? get selectedYear => _selectedYear;
  String? get selectedTerm => _selectedTerm;
  bool get isLoading => _isLoading;
  bool get assessmentsLoading => _assessmentsLoading;

  // Retrieves the most recent assessment for a specific subject
  SubjectAssessment? getLatestMark(String subjectId) => _latestMarks[subjectId];

  // Extracts and sorts the available years from the history data
  List<String> get availableYears {
    final years = _subjectHistory.keys.toList();
    years.sort((a, b) => b.compareTo(a));
    return years;
  }

  // Extracts and sorts the available terms for the currently selected year
  List<String> get availableTerms {
    if (_selectedYear == null) return [];
    final terms = _subjectHistory[_selectedYear!]?.toList() ?? [];
    terms.sort((a, b) => b.compareTo(a));
    return terms;
  }

  // Load Main Subjects
  // Fetches the list of subjects and their latest assessment marks for the learner
  Future<void> load({required String learnerId}) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      // Fetch the overall subject grades
      _allSubjects = await _api.fetchSubjectGrades(learnerId: learnerId);
      
      // Loop through each subject to fetch its latest individual assessment
      for (var subject in _allSubjects) {
        final latest = await _api.fetchLatestAssessment(
          learnerId: learnerId,
          subjectId: subject.subjectId,
        );
        _latestMarks[subject.subjectId] = latest;
      }
    } catch (e) {
      _error = e.toString();
    }
    _isLoading = false;
    notifyListeners();
  }

  // Load Subject History
  // Fetches the years and terms available for a specific subject and loads the first set of data
  Future<void> openSubjectHistory({
    required String learnerId,
    required String subjectId,
  }) async {
    _assessmentsLoading = true;
    _assessments = [];
    _subjectHistory = {};
    _selectedYear = null;
    _selectedTerm = null;
    notifyListeners();

    try {
      // Fetch the available years and terms for this subject
      _subjectHistory = await _api.fetchSubjectHistory(
        learnerId: learnerId,
        subjectId: subjectId,
      );

      // Select the most recent year and term automatically
      if (_subjectHistory.isNotEmpty) {
        _selectedYear = availableYears.first;
        if (availableTerms.isNotEmpty) {
          _selectedTerm = availableTerms.first;
        }
      }

      // Load the assessments for the selected year and term
      await _loadFilteredAssessments(learnerId, subjectId);
    } catch (e) {
      _error = e.toString();
    }
    _assessmentsLoading = false;
    notifyListeners();
  }

  // Updates the selected year and reloads the filtered assessments
  Future<void> setYear(String year, {required String learnerId, required String subjectId}) async {
    _selectedYear = year;
    _selectedTerm = availableTerms.isNotEmpty ? availableTerms.first : null;
    await _loadFilteredAssessments(learnerId, subjectId);
    notifyListeners();
  }

  // Updates the selected term and reloads the filtered assessments
  Future<void> setTerm(String term, {required String learnerId, required String subjectId}) async {
    _selectedTerm = term;
    await _loadFilteredAssessments(learnerId, subjectId);
    notifyListeners();
  }

  // Helper function to fetch assessments based on the currently selected year and term
  Future<void> _loadFilteredAssessments(String learnerId, String subjectId) async {
    _assessmentsLoading = true;
    notifyListeners();

    try {
      _assessments = await _api.fetchAssessments(
        learnerId: learnerId,
        subjectId: subjectId,
        year: int.tryParse(_selectedYear ?? ''),
        term: _selectedTerm,
      );
    } catch (e) {
      _error = e.toString();
    }
    _assessmentsLoading = false;
    notifyListeners();
  }
    
  // Clears all data and resets the state
  void reset() {
    _allSubjects = [];
    _latestMarks.clear();
    _assessments = [];
    _subjectHistory = {};
    _selectedYear = null;
    _selectedTerm = null;
    _isLoading = false;
    _assessmentsLoading = false;
    _error = null;
    notifyListeners();
  }
}