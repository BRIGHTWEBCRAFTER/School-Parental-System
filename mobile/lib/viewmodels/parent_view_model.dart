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
import 'package:image_picker/image_picker.dart';

import '../models/learner.dart';
import '../models/parent.dart';
import '../services/supabase_service.dart';

// Manages the state and logic for the parent profile and linked learners
class ParentViewModel with ChangeNotifier {
  final SupabaseService _api;

  // Constructor requires the Supabase service to fetch and update data
  ParentViewModel(this._api);

  // Internal state variables
  ParentProfile? _parent;
  List<Learner> _allLearners = [];
  Learner? _selectedLearner;
  bool _isLoading = false;
  String? _error;

  // Getters to expose state to the UI
  ParentProfile? get parent => _parent;
  List<Learner> get allLearners => _allLearners;
  Learner? get learner => _selectedLearner;
  bool get isLoading => _isLoading;
  String? get error => _error;
  
  // Checks if the parent has more than one linked learner
  bool get hasMultipleChildren => _allLearners.length > 1;

  // Fetches the parent profile and all linked learners
  Future<void> load() async {
    _isLoading = true;
    _error = null;
    notifyListeners();

    try {
      _parent = await _api.fetchParentProfile();
      _allLearners = await _api.fetchAllLearners();

      // Automatically select the first learner if the list is not empty
      if (_allLearners.isNotEmpty) {
        _selectedLearner = _allLearners.first;
      }
    } catch (e) {
      _error = e.toString();
    }

    _isLoading = false;
    notifyListeners();
  }

  // Changes the currently selected learner
  void selectLearner(Learner newLearner) {
    if (_selectedLearner?.id == newLearner.id) return;
    _selectedLearner = newLearner;
    notifyListeners();
  }

  // UPDATE INFO
  // Updates the parent's basic information and refreshes the local profile data
  Future<void> updateParentInfo({
    required String fullName,
    required String phoneNumber,
    required String email,
  }) async {
    await _api.updateParentInfo(
      fullName: fullName,
      phone: phoneNumber,
    );
    _parent = await _api.fetchParentProfile();
    notifyListeners();
  }

  // Updates the parent's email address and refreshes the local profile data
  Future<void> updateEmail({
    required String currentPassword,
    required String newEmail,
  }) async {
    await _api.updateEmail(
      currentPassword: currentPassword,
      newEmail: newEmail,
    );
    _parent = await _api.fetchParentProfile();
    notifyListeners();
  }

  // Uploads a new profile picture and refreshes the local profile data
  Future<void> updateProfilePicture(XFile image) async {
    // Read the image file as bytes
    final bytes = await image.readAsBytes();
    
    // Extract the file extension (e.g., 'jpg', 'png')
    final extension = image.name.split('.').last;
    
    await _api.updateProfilePicture(
      bytes: bytes,
      extension: extension,
    );
    _parent = await _api.fetchParentProfile();
    notifyListeners();
  }

  // Clears all data and resets the state
  void reset() {
    _parent = null;
    _allLearners = [];
    _selectedLearner = null;
    _isLoading = false;
    _error = null;
    notifyListeners();
  }
}