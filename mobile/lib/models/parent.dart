/* 
BrightWeb Crafters

L.P MOSHOEU    - 223046876
A SITHOLE      - 223000460
M.A NKUNA      - 224000274
S.T PHEKO      - 223050336
T.M.C MOTONE   - 224027806
P.A LUTHADA    - 222023335
 */



// Data model representing a parent's profile information
class ParentProfile {
  // Unique identifier for the parent
  final String id;
  
  // The parent's full name
  final String fullName;
  
  // The parent's email address
  final String email;
  
  // The parent's phone number
  final String phone;
  
  // The parent's role in the system (typically "Parent")
  final String role;
  
  // The parent's guardian number for school records
  final String guardianNumber;
  
  // The parent's gender (optional)
  final String? gender;
  
  // An alternative contact phone number (optional)
  final String? altPhone;
  
  // The parent's home address (optional)
  final String? address;
  
  // URL pointing to the parent's profile picture (optional)
  final String? profilePicture;

  // Constructor with required fields for the main content
  const ParentProfile({
    required this.id,
    required this.fullName,
    required this.email,
    required this.phone,
    required this.role,
    required this.guardianNumber,
    this.gender,
    this.altPhone,
    this.address,
    this.profilePicture,
  });

  // Factory constructor that parses the joined Supabase response
  factory ParentProfile.fromJoin(Map<String, dynamic> json) {
    // Extract the nested 'parents' object from the join
    final parents = json['parents'] as Map<String, dynamic>?;
    
    return ParentProfile(
      id: json['id'] as String? ?? '',
      fullName: json['full_name'] as String? ?? '',
      email: json['email'] as String? ?? '',
      phone: json['phone'] as String? ?? '',
      role: json['role'] as String? ?? 'Parent',
      // These fields come from the 'parents' table
      guardianNumber: parents?['guardian_number'] as String? ?? '',
      gender: parents?['gender'] as String?,
      altPhone: parents?['alternative_phone'] as String?,
      address: parents?['address'] as String?,
      // This field comes from the 'users' table
      profilePicture: json['profile_picture'] as String?,
    );
  }

  // Creates a new ParentProfile object with some fields replaced
  ParentProfile copyWith({
    String? id,
    String? fullName,
    String? email,
    String? phone,
    String? role,
    String? guardianNumber,
    String? gender,
    String? altPhone,
    String? address,
    String? profilePicture,
  }) {
    return ParentProfile(
      id: id ?? this.id,
      fullName: fullName ?? this.fullName,
      email: email ?? this.email,
      phone: phone ?? this.phone,
      role: role ?? this.role,
      guardianNumber: guardianNumber ?? this.guardianNumber,
      gender: gender ?? this.gender,
      altPhone: altPhone ?? this.altPhone,
      address: address ?? this.address,
      profilePicture: profilePicture ?? this.profilePicture,
    );
  }
}