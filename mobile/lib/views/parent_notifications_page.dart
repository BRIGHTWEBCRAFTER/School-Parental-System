import 'package:flutter/material.dart';
import 'package:supabase_flutter/supabase_flutter.dart';

class ParentNotificationsPage extends StatefulWidget {
  const ParentNotificationsPage({super.key});

  @override
  State<ParentNotificationsPage> createState() =>
      _ParentNotificationsPageState();
}

class _ParentNotificationsPageState extends State<ParentNotificationsPage> {
  final SupabaseClient _supabase = Supabase.instance.client;
  late Future<List<Map<String, dynamic>>> _notificationsFuture;

  @override
  void initState() {
    super.initState();
    _notificationsFuture = _loadNotifications();
  }

  Future<List<Map<String, dynamic>>> _loadNotifications() async {
    final user = _supabase.auth.currentUser;
    if (user == null) {
      throw Exception('Please log in to view your notifications.');
    }

    final response = await _supabase
        .from('notifications')
        .select('id, title, message, notification_type, is_read, created_at')
        .eq('user_id', user.id)
        .order('created_at', ascending: false);

    return response.map((item) => Map<String, dynamic>.from(item)).toList();
  }

  Future<void> _refreshNotifications() async {
    final next = _loadNotifications();
    setState(() {
      _notificationsFuture = next;
    });
    try {
      await next;
    } catch (_) {}
  }

  Future<void> _markAsRead(Map<String, dynamic> notification) async {
    if (notification['is_read'] == true) return;
    final user = _supabase.auth.currentUser;
    if (user == null) return;

    await _supabase
        .from('notifications')
        .update({
          'is_read': true,
          'read_at': DateTime.now().toUtc().toIso8601String(),
        })
        .eq('id', notification['id'])
        .eq('user_id', user.id);

    await _refreshNotifications();
  }

  IconData _notificationIcon(String type) {
    switch (type.toLowerCase()) {
      case 'announcement':
        return Icons.campaign_outlined;
      case 'event':
        return Icons.event_outlined;
      case 'grade':
        return Icons.school_outlined;
      case 'attendance':
        return Icons.fact_check_outlined;
      default:
        return Icons.notifications_outlined;
    }
  }

  String _formatDate(dynamic value) {
    final date = DateTime.tryParse(value?.toString() ?? '');
    if (date == null) return '';
    final local = date.toLocal();
    return '${local.day}/${local.month}/${local.year} '
        '${local.hour.toString().padLeft(2, '0')}:'
        '${local.minute.toString().padLeft(2, '0')}';
  }

  Future<void> _openNotification(Map<String, dynamic> notification) async {
    try {
      await _markAsRead(notification);
    } catch (_) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Could not update notification status.')),
        );
      }
    }

    if (!mounted) return;

    showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      builder: (context) {
        return SafeArea(
          child: Padding(
            padding: const EdgeInsets.all(24),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  notification['title']?.toString() ?? 'Notification',
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: Color(0xFF073F32),
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  notification['message']?.toString() ?? '',
                  style: const TextStyle(fontSize: 15),
                ),
                const SizedBox(height: 16),
                Text(
                  _formatDate(notification['created_at']),
                  style: const TextStyle(color: Colors.grey),
                ),
                const SizedBox(height: 12),
              ],
            ),
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF5F7F4),
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.primary,
        foregroundColor: Colors.white,
        title: const Text('Notifications'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            tooltip: 'Refresh notifications',
            onPressed: _refreshNotifications,
          ),
        ],
      ),
      body: FutureBuilder<List<Map<String, dynamic>>>(
        future: _notificationsFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.error_outline, size: 48, color: Colors.red),
                    const SizedBox(height: 12),
                    Text(snapshot.error.toString(), textAlign: TextAlign.center),
                    const SizedBox(height: 12),
                    ElevatedButton(
                      onPressed: _refreshNotifications,
                      child: const Text('Try Again'),
                    ),
                  ],
                ),
              ),
            );
          }

          final notifications = snapshot.data ?? [];

          if (notifications.isEmpty) {
            return RefreshIndicator(
              onRefresh: _refreshNotifications,
              child: ListView(
                physics: const AlwaysScrollableScrollPhysics(),
                children: const [
                  SizedBox(height: 120),
                  Icon(Icons.notifications_none, size: 70, color: Colors.grey),
                  SizedBox(height: 16),
                  Center(
                    child: Text('No notifications yet',
                        style: TextStyle(fontSize: 18, fontWeight: FontWeight.w600)),
                  ),
                  SizedBox(height: 10),
                  Center(child: Text('School messages will appear here.')),
                ],
              ),
            );
          }

          return RefreshIndicator(
            onRefresh: _refreshNotifications,
            child: ListView.builder(
              padding: const EdgeInsets.all(12),
              itemCount: notifications.length,
              itemBuilder: (context, index) {
                final item = notifications[index];
                final isRead = item['is_read'] == true;
                final type = item['notification_type']?.toString() ?? '';

                return Card(
                  elevation: isRead ? 1 : 3,
                  color: isRead ? Colors.white : const Color(0xFFFFF8E5),
                  margin: const EdgeInsets.only(bottom: 10),
                  child: ListTile(
                    onTap: () => _openNotification(item),
                    leading: CircleAvatar(
                      backgroundColor: const Color(0xFFE1F0E8),
                      child: Icon(_notificationIcon(type), color: const Color(0xFF073F32)),
                    ),
                    title: Text(
                      item['title']?.toString() ?? 'Notification',
                      style: TextStyle(
                        fontWeight: isRead ? FontWeight.normal : FontWeight.bold,
                      ),
                    ),
                    subtitle: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const SizedBox(height: 6),
                        Text(
                          item['message']?.toString() ?? '',
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                        const SizedBox(height: 8),
                        Text(
                          _formatDate(item['created_at']),
                          style: const TextStyle(fontSize: 12, color: Colors.grey),
                        ),
                      ],
                    ),
                    trailing: isRead
                        ? null
                        : const Icon(Icons.circle, color: Color(0xFFE6AD28), size: 12),
                  ),
                );
              },
            ),
          );
        },
      ),
    );
  }
}