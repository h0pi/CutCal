import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';

class NotificationsScreen extends StatefulWidget {
  const NotificationsScreen({super.key});

  @override
  State<NotificationsScreen> createState() => _NotificationsScreenState();
}

class _NotificationsScreenState extends State<NotificationsScreen> {
  List<NotificationModel> _notifications = [];
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _load();
    // No manual refresh button by design: notifications auto-refresh every 30s.
    _pollTimer = Timer.periodic(const Duration(seconds: 30), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    final result = await context.read<NotificationProvider>().get(filter: {'pageSize': 50});
    if (!mounted) return;
    setState(() {
      _notifications = result.items;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final unreadCount = _notifications.where((n) => !n.isRead).length;

    return Scaffold(
      appBar: AppBar(
        title: Text(unreadCount > 0 ? 'Notifications ($unreadCount)' : 'Notifications'),
        actions: [
          if (unreadCount > 0)
            TextButton(
              onPressed: () async {
                await context.read<NotificationProvider>().markAllRead();
                _load();
              },
              child: const Text('Mark all read', style: TextStyle(color: Colors.white)),
            ),
        ],
      ),
      body: _isLoading
          ? const LoadingIndicator()
          : _notifications.isEmpty
              ? const Center(child: Text('No notifications yet.'))
              : ListView.builder(
                  itemCount: _notifications.length,
                  itemBuilder: (context, index) {
                    final n = _notifications[index];
                    return ListTile(
                      tileColor: n.isRead ? null : Colors.deepPurple.withValues(alpha: 0.06),
                      leading: Icon(n.isRead ? Icons.notifications_none : Icons.notifications_active, color: n.isRead ? Colors.grey : Colors.deepPurple),
                      title: Text(n.title, style: TextStyle(fontWeight: n.isRead ? FontWeight.normal : FontWeight.bold)),
                      subtitle: Text('${n.body}\n${DateFormat('MMM d, HH:mm').format(n.sentAt)}'),
                      isThreeLine: true,
                      trailing: n.isRead
                          ? null
                          : IconButton(
                              icon: const Icon(Icons.done),
                              onPressed: () async {
                                await context.read<NotificationProvider>().markRead(n.id);
                                _load();
                              },
                            ),
                    );
                  },
                ),
    );
  }
}
