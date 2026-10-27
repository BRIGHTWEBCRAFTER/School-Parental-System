import 'package:flutter/material.dart';
import '../models/event.dart';
import '../services/supabase_service.dart';

class EventViewModel with ChangeNotifier {
  final SupabaseService _api;
  EventViewModel(this._api);

  List<SchoolEvent> _events = [];
  bool _isLoading = false;
  String? _error;

  List<SchoolEvent> get events => _events;
  bool get isLoading => _isLoading;
  String? get error => _error;

  Future<void> load({required String? learnerGrade}) async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _events = await _api.fetchEvents(learnerGrade: learnerGrade);
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  void reset() {
    _events = [];
    _isLoading = false;
    _error = null;
    notifyListeners();
  }
}