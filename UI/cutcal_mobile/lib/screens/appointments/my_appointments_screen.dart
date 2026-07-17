import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/auth_provider.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';
import 'appointment_detail_screen.dart';

class MyAppointmentsScreen extends StatefulWidget {
  const MyAppointmentsScreen({super.key});

  @override
  State<MyAppointmentsScreen> createState() => _MyAppointmentsScreenState();
}

class _MyAppointmentsScreenState extends State<MyAppointmentsScreen> with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  List<AppointmentModel> _all = [];
  bool _isLoading = true;
  Timer? _pollTimer;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
    _load();
    // Status changes (confirm/cancel/complete) can come from someone else (a salon
    // manager on desktop), so this keeps the list current without a manual refresh.
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _load(silent: true));
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    super.dispose();
  }

  Future<void> _load({bool silent = false}) async {
    if (!silent) setState(() => _isLoading = true);
    final userId = context.read<AuthProvider>().userId;
    final result = await context.read<AppointmentProvider>().get(filter: {'customerId': userId, 'pageSize': 100});
    if (!mounted) return;
    setState(() {
      _all = result.items;
      _isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final upcoming = _all.where((a) => a.stateName == 'Pending' || a.stateName == 'Confirmed').toList();
    final history = _all.where((a) => a.stateName == 'Completed' || a.stateName == 'Cancelled').toList();

    return Scaffold(
      appBar: AppBar(
        title: const Text('My Appointments'),
        bottom: TabBar(controller: _tabController, tabs: const [Tab(text: 'Upcoming'), Tab(text: 'History')]),
      ),
      body: _isLoading
          ? const LoadingIndicator()
          : TabBarView(
              controller: _tabController,
              children: [
                _AppointmentList(appointments: upcoming, onChanged: _load),
                _AppointmentList(appointments: history, onChanged: _load),
              ],
            ),
    );
  }
}

class _AppointmentList extends StatelessWidget {
  final List<AppointmentModel> appointments;
  final VoidCallback onChanged;

  const _AppointmentList({required this.appointments, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    if (appointments.isEmpty) {
      return const Center(child: Text('No appointments here yet.'));
    }
    return ListView.builder(
      itemCount: appointments.length,
      itemBuilder: (context, index) {
        final a = appointments[index];
        return Card(
          margin: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
          child: ListTile(
            title: Text(a.salonName ?? 'Salon #${a.salonId}'),
            subtitle: Text('${a.serviceName ?? ''}\n${DateFormat('MMM d, y • HH:mm').format(a.scheduledAt)}'),
            isThreeLine: true,
            trailing: StatusBadge(status: a.stateName),
            onTap: () async {
              await Navigator.of(context).push(
                MaterialPageRoute(builder: (_) => AppointmentDetailScreen(appointmentId: a.id)),
              );
              onChanged();
            },
          ),
        );
      },
    );
  }
}
