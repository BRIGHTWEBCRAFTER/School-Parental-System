import 'dart:typed_data';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';

import '../viewmodels/parent_view_model.dart';

// Screen that allows the parent to view and update their personal information
class PersonalInfoScreen extends StatefulWidget {
  const PersonalInfoScreen({super.key});

  @override
  State<PersonalInfoScreen> createState() => _PersonalInfoScreenState();
}

class _PersonalInfoScreenState extends State<PersonalInfoScreen> {
  // Form key for validation
  final _formKey = GlobalKey<FormState>();
  
  // Controllers for the text input fields
  late TextEditingController _nameCtrl;
  late TextEditingController _phoneCtrl;
  late TextEditingController _emailCtrl;
  final _passwordCtrl = TextEditingController(); 
  
  // State variables for loading and visibility
  bool _isSaving = false;
  bool _isUploadingImage = false;
  bool _obscurePassword = true; 
  
  // State for the selected profile image
  XFile? _selectedImage;
  Uint8List? _imageBytes;
  String? _currentImageUrl;

  @override
  void initState() {
    super.initState();
    // Load the current parent data into the text fields
    final parent = context.read<ParentViewModel>().parent;
    _nameCtrl = TextEditingController(text: parent?.fullName ?? '');
    _phoneCtrl = TextEditingController(text: parent?.phone ?? '');
    _emailCtrl = TextEditingController(text: parent?.email ?? '');
    _currentImageUrl = parent?.profilePicture;
  }

  @override
  void dispose() {
    // Clean up the controllers
    _nameCtrl.dispose();
    _phoneCtrl.dispose();
    _emailCtrl.dispose();
    _passwordCtrl.dispose(); 
    super.dispose();
  }

  // Opens the device gallery to pick a new profile image
  Future<void> _pickImage() async {
    final picker = ImagePicker();
    try {
      final pickedFile = await picker.pickImage(
        source: ImageSource.gallery,
        imageQuality: 50,
        maxWidth: 500,
      );

      if (pickedFile != null) {
        // Read the image bytes to allow for instant preview
        final bytes = await pickedFile.readAsBytes();
        setState(() {
          _selectedImage = pickedFile;
          _imageBytes = bytes;
        });
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Error picking image: $e'), backgroundColor: Colors.red),
      );
    }
  }

  // Handles saving all the updated information
  Future<void> _saveInfo() async {
    // Validate the form first
    if (!_formKey.currentState!.validate()) return;

    final parentVm = context.read<ParentViewModel>();
    final currentEmail = parentVm.parent?.email ?? '';
    final newEmail = _emailCtrl.text.trim();
    final emailChanged = currentEmail != newEmail;

    // If the email is being changed, a password is required
    if (emailChanged && _passwordCtrl.text.trim().isEmpty) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Please enter your current password to change email.'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    setState(() => _isSaving = true);

    try {
      // 1. Upload the new profile picture if one was selected
      if (_selectedImage != null) {
        setState(() => _isUploadingImage = true);
        await parentVm.updateProfilePicture(_selectedImage!);
        setState(() => _isUploadingImage = false);
      }

      // 2. Update the email with password verification if it changed
      if (emailChanged) {
        await parentVm.updateEmail(
          currentPassword: _passwordCtrl.text.trim(),
          newEmail: newEmail,
        );
      } else {
        // 3. Otherwise, just update the name and phone
        await parentVm.updateParentInfo(
          fullName: _nameCtrl.text.trim(),
          phoneNumber: _phoneCtrl.text.trim(),
          email: currentEmail,
        );
      }

      if (!mounted) return;
      
      // Show success message
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Personal information updated successfully!'),
          backgroundColor: Colors.green,
        ),
      );
      
      // Return to the previous screen
      Navigator.pop(context);
    } catch (e) {
      if (!mounted) return;
      // Show error message if the update fails
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Failed to update: $e'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    // Access theme colors for dynamic styling
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(title: const Text('Personal Information')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              // Profile Picture Picker
              Center(
                child: GestureDetector(
                  onTap: _pickImage,
                  child: Stack(
                    children: [
                      // Avatar container
                      Container(
                        width: 100,
                        height: 100,
                        decoration: BoxDecoration(
                          color: colorScheme.primaryContainer,
                          shape: BoxShape.circle,
                        ),
                        clipBehavior: Clip.antiAlias,
                        // Show new image if selected, else the existing image, else a default icon
                        child: _imageBytes != null
                            ? Image.memory(_imageBytes!, fit: BoxFit.cover)
                            : _currentImageUrl != null
                                ? Image.network(_currentImageUrl!, fit: BoxFit.cover)
                                : Icon(Icons.person, size: 50, color: colorScheme.onPrimaryContainer),
                      ),
                      // Camera icon overlay
                      Positioned(
                        bottom: 0,
                        right: 0,
                        child: Container(
                          padding: const EdgeInsets.all(6),
                          decoration: BoxDecoration(
                            color: colorScheme.primary,
                            shape: BoxShape.circle,
                            border: Border.all(color: theme.scaffoldBackgroundColor, width: 2),
                          ),
                          child: Icon(Icons.camera_alt, size: 16, color: colorScheme.onPrimary),
                        ),
                      ),
                      // Semi-transparent overlay shown while uploading
                      if (_isUploadingImage)
                        Positioned.fill(
                          child: Container(
                            decoration: const BoxDecoration(
                              color: Colors.black45,
                              shape: BoxShape.circle,
                            ),
                            alignment: Alignment.center,
                            child: const CircularProgressIndicator(color: Colors.white),
                          ),
                        ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 8),
              Center(
                child: Text(
                  'Tap to change profile picture',
                  style: TextStyle(fontSize: 12, color: colorScheme.onSurfaceVariant),
                ),
              ),
              const SizedBox(height: 24),

              // Full Name Field
              Text(
                'Full Name',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: colorScheme.onSurface),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _nameCtrl,
                style: TextStyle(color: colorScheme.onSurface),
                decoration: const InputDecoration(
                  hintText: 'Enter your full name',
                  border: OutlineInputBorder(),
                ),
                validator: (val) => val == null || val.isEmpty ? 'Name is required' : null,
              ),
              const SizedBox(height: 16),

              // Email Address Field
              Text(
                'Email Address',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: colorScheme.onSurface),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _emailCtrl,
                style: TextStyle(color: colorScheme.onSurface),
                keyboardType: TextInputType.emailAddress,
                decoration: const InputDecoration(
                  hintText: 'Enter your email',
                  border: OutlineInputBorder(),
                ),
                validator: (val) {
                  if (val == null || val.isEmpty) return 'Email is required';
                  if (!val.contains('@')) return 'Enter a valid email';
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Current Password Field (for email changes)
              Text(
                'Current Password (required to change email)',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: colorScheme.onSurface),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _passwordCtrl,
                obscureText: _obscurePassword,
                style: TextStyle(color: colorScheme.onSurface),
                decoration: InputDecoration(
                  hintText: 'Enter current password',
                  border: const OutlineInputBorder(),
                  suffixIcon: IconButton(
                    icon: Icon(
                      _obscurePassword ? Icons.visibility_off : Icons.visibility,
                      color: colorScheme.onSurfaceVariant,
                    ),
                    onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                  ),
                ),
              ),
              const SizedBox(height: 16),

              // Phone Number Field
              Text(
                'Phone Number',
                style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: colorScheme.onSurface),
              ),
              const SizedBox(height: 6),
              TextFormField(
                controller: _phoneCtrl,
                style: TextStyle(color: colorScheme.onSurface),
                keyboardType: TextInputType.phone,
                decoration: const InputDecoration(
                  hintText: 'Enter your phone number',
                  border: OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 30),

              // Save Button
              SizedBox(
                height: 50,
                child: ElevatedButton(
                  onPressed: _isSaving ? null : _saveInfo,
                  child: _isSaving
                      // Show a loading spinner while saving
                      ? const SizedBox(
                          width: 22,
                          height: 22,
                          child: CircularProgressIndicator(color: Colors.white, strokeWidth: 2.5),
                        )
                      // Show the button text otherwise
                      : const Text(
                          'Save Changes',
                          style: TextStyle(fontSize: 16, fontWeight: FontWeight.w600),
                        ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}