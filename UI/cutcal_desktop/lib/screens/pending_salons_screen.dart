import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/api_client_exception.dart';
import '../utils/utils_widgets.dart';

class PendingSalonsScreen extends StatefulWidget {
  const PendingSalonsScreen({super.key});

  @override
  State<PendingSalonsScreen> createState() => _PendingSalonsScreenState();
}

class _PendingSalonsScreenState extends State<PendingSalonsScreen> {
  List<SalonModel> _salons = [];
  Map<int, String> _ownerNames = {};
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<SalonProvider>().get(filter: {'isApproved': false, 'pageSize': 100});
    final users = await context.read<UserProvider>().get(filter: {'pageSize': 100});
    setState(() {
      _salons = result.items;
      _ownerNames = {for (final u in users.items) u.id: '${u.firstName} ${u.lastName}'};
      _isLoading = false;
    });
  }

  Future<void> _approve(SalonModel salon) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Approve salon',
      message: 'Approve "${salon.name}"? It will become visible to customers.',
    );
    if (!confirmed) return;

    try {
      await context.read<SalonProvider>().approve(salon.id);
      if (mounted) showSuccessSnackBar(context, 'Salon approved.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Text('Pending salons', style: Theme.of(context).textTheme.titleMedium),
              const SizedBox(width: 12),
              IconButton(icon: const Icon(Icons.refresh), tooltip: 'Refresh', onPressed: _load),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : _salons.isEmpty
                    ? const Center(child: Text('No salons waiting for approval.'))
                    : ListView.builder(
                        itemCount: _salons.length,
                        itemBuilder: (context, index) {
                          final s = _salons[index];
                          return Card(
                            child: ListTile(
                              leading: const Icon(Icons.storefront_outlined),
                              title: Text(s.name),
                              subtitle: Text(
                                '${s.salonCategoryName ?? 'Uncategorized'} • ${s.cityName ?? 'Unknown city'} • ${s.address}\n'
                                'Owner: ${_ownerNames[s.ownerId] ?? 'Unknown'}',
                              ),
                              isThreeLine: true,
                              trailing: FilledButton.icon(
                                icon: const Icon(Icons.check),
                                label: const Text('Approve'),
                                onPressed: () => _approve(s),
                              ),
                            ),
                          );
                        },
                      ),
          ),
        ],
      ),
    );
  }
}
