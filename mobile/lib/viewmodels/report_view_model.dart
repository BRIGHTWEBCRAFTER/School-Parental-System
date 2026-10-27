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

import '../models/report.dart';
import '../services/supabase_service.dart';

// Manages the state and logic for viewing and filtering report files
class ReportViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor requires the Supabase service to fetch data
  ReportViewModel(this._api);

  // All reports fetched from the database (unfiltered)
  List<ReportFile> _allReports = [];

  // Filter state variables
  int? _selectedYear;
  String? _selectedTerm;

  // Loading and error state
  bool _isLoading = false;
  String? _error;

  // Getters to expose state to the UI
  List<ReportFile> get allReports => _allReports;
  int? get selectedYear => _selectedYear;
  String? get selectedTerm => _selectedTerm;
  bool get isLoading => _isLoading;
  String? get error => _error;
  bool get hasAnyReports => _allReports.isNotEmpty;

  // Filtered list of reports based on the selected year and term
  List<ReportFile> get filteredReports {
    return _allReports.where((r) {
      if (_selectedYear != null && r.year != _selectedYear) return false;
      if (_selectedTerm != null && r.term != _selectedTerm) return false;
      return true;
    }).toList();
  }

  // Extracts and sorts the available years from the reports (newest first)
  List<int> get availableYears {
    final years = _allReports.map((r) => r.year).toSet().toList();
    years.sort((a, b) => b.compareTo(a));
    return years;
  }

  // Extracts and sorts the available terms from the reports (Term 4, Term 3, etc.)
  List<String> get availableTerms {
    final terms = _allReports.map((r) => r.term).toSet().toList();
    terms.sort((a, b) => b.compareTo(a));
    return terms;
  }

  // Load all reports
  // Fetches all report files for the learner from the database
  Future<void> load({
    required String learnerId,
    required String learnerName,
  }) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _allReports = await _api.fetchReports(
        learnerId: learnerId,
        learnerName: learnerName,
      );
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  // Filters
  // Updates the selected year filter and notifies the UI
  void setYear(int? year) {
    _selectedYear = year;
    notifyListeners();
  }

  // Updates the selected term filter and notifies the UI
  void setTerm(String? term) {
    _selectedTerm = term;
    notifyListeners();
  }

  // Clears all active filters and resets the view to show all reports
  void clearFilters() {
    _selectedYear = null;
    _selectedTerm = null;
    notifyListeners();
  }

  // Clears all data and resets the state
  void reset() {
    _allReports = [];
    _selectedYear = null;
    _selectedTerm = null;
    _isLoading = false;
    _error = null;
    notifyListeners();
  }
}