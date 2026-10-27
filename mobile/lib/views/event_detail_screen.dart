import 'package:flutter/material.dart';

import '../models/event.dart';

// Displays the full details of a single school event
class EventDetailScreen extends StatelessWidget {
  // The event object passed from the previous screen
  final SchoolEvent event;
  
  const EventDetailScreen({super.key, required this.event});

  @override
  Widget build(BuildContext context) {
    // Access the theme color scheme for dynamic styling
    final colorScheme = Theme.of(context).colorScheme;
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Event Details')),
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Decorative header banner with a calendar icon
            Container(
              height: 160,
              width: double.infinity,
              color: colorScheme.primaryContainer,
              child: Icon(
                Icons.calendar_today,
                size: 60, 
                color: colorScheme.primary,
              ),
            ),
            
            // Main content padding
            Padding(
              padding: const EdgeInsets.all(20),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Event title
                  Text(
                    event.name,
                    style: TextStyle(
                      fontSize: 22, 
                      fontWeight: FontWeight.bold,
                      color: colorScheme.onSurface,
                    ),
                  ),
                  const SizedBox(height: 16),
                  
                  // Event date, time, and venue
                  _row(context, Icons.calendar_month, 'Date', event.day),
                  _row(context, Icons.access_time, 'Time', event.timeRange),
                  _row(context, Icons.location_on, 'Venue', event.venue ?? '—'),
                  
                  const SizedBox(height: 20),
                  
                  // Description section
                  Text(
                    'Description',
                    style: TextStyle(
                      fontSize: 16, 
                      fontWeight: FontWeight.bold,
                      color: colorScheme.onSurface,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    event.description ?? 'No description provided.',
                    style: TextStyle(
                      fontSize: 13, 
                      height: 1.5,
                      color: colorScheme.onSurfaceVariant,
                    ),
                  ),
                  
                  const SizedBox(height: 30),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Helper widget to build a row containing an icon, label, and value
  Widget _row(BuildContext context, IconData icon, String label, String value) {
    final colorScheme = Theme.of(context).colorScheme;
    
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 6),
      child: Row(
        children: [
          // Small leading icon
          Icon(icon, size: 20, color: colorScheme.primary),
          const SizedBox(width: 10),
          
          // Fixed-width label text
          SizedBox(
            width: 60,
            child: Text(
              label,
              style: TextStyle(
                fontSize: 12, 
                color: colorScheme.onSurfaceVariant,
              ),
            ),
          ),
          
          // Flexible value text that takes the remaining space
          Expanded(
            child: Text(
              value,
              style: TextStyle(
                fontSize: 13, 
                fontWeight: FontWeight.w600,
                color: colorScheme.onSurface,
              ),
            ),
          ),
        ],
      ),
    );
  }
}