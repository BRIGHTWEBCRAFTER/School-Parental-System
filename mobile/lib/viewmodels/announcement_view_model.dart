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

import '../models/announcement.dart';
import '../services/supabase_service.dart';

// Manages the state and logic for announcements
class AnnouncementViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor requires the Supabase service to fetch and update data
  AnnouncementViewModel(this._api);

  // Internal state variables
  List<Announcement> _items = [];
  bool _isLoading = false;
  String? _error;

  // Getters to expose state to the UI
  List<Announcement> get items => _items;
  bool get isLoading => _isLoading;
  String? get error => _error;
  
  // Calculates how many announcements have not been read yet
  int get unreadCount => _items.where((a) => !a.isRead).length;

  // Fetches the list of announcements from the database
  Future<void> load() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _items = await _api.fetchAnnouncements();
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  // Marks a specific announcement as read by its index in the list
  Future<void> markRead(int index) async {
    final item = _items[index];
    if (item.isRead) return;

    // Optimistically update the UI immediately
    _items[index] = item.copyWith(isRead: true);
    notifyListeners();

    try {
      // Save the change to the database
      await _api.markAnnouncementRead(announcementId: item.id);
    } catch (_) {
      // Revert the UI change if the database update fails
      _items[index] = item.copyWith(isRead: false);
      notifyListeners();
    }
  }

  // Marks an announcement as acknowledged and read
  Future<void> acknowledgeAnnouncement(String announcementId) async {
    try {
      // 1. Save the acknowledgment status to the database
      await _api.acknowledgeAnnouncement(announcementId: announcementId);
      
      // 2. Update the local state so the UI reflects the change instantly
      final index = _items.indexWhere((a) => a.id == announcementId);
      if (index != -1) {
        _items[index] = _items[index].copyWith(
          isAcknowledged: true,
          isRead: true, 
        );
        notifyListeners();
      }
    } catch (e) {
      // Rethrow the error so the UI can show an error message
      rethrow;
    }
  }

  // Clears all data and resets the state
  void reset() {
    _items = [];
    _isLoading = false;
    _error = null;
    notifyListeners();
  }
}