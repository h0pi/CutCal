import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../models/models.dart';
import '../providers/entity_providers.dart';
import '../utils/utils_widgets.dart';

class ReviewsScreen extends StatefulWidget {
  const ReviewsScreen({super.key});

  @override
  State<ReviewsScreen> createState() => _ReviewsScreenState();
}

class _ReviewsScreenState extends State<ReviewsScreen> {
  List<SalonModel> _salons = [];
  int? _salonId;
  int? _minRating;
  List<ReviewModel> _reviews = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _init();
  }

  Future<void> _init() async {
    final salons = await context.read<SalonProvider>().get(filter: {'pageSize': 100});
    setState(() => _salons = salons.items);
    await _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<ReviewProvider>().get(filter: {'salonId': _salonId, 'pageSize': 100});
    var items = result.items;
    if (_minRating != null) {
      items = items.where((r) => r.rating >= _minRating!).toList();
    }
    setState(() {
      _reviews = items;
      _isLoading = false;
    });
  }

  Future<void> _reply(ReviewModel review) async {
    final controller = TextEditingController(text: review.salonReply);
    final reply = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Reply to review'),
        content: TextField(controller: controller, maxLines: 3, decoration: const InputDecoration(labelText: 'Your reply')),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(controller.text), child: const Text('Send')),
        ],
      ),
    );
    if (reply == null || reply.isEmpty) return;

    await context.read<ReviewProvider>().reply(review.id, reply);
    if (mounted) showSuccessSnackBar(context, 'Reply sent.');
    _load();
  }

  Future<void> _remove(ReviewModel review) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Remove review',
      message: 'Remove this review for containing inappropriate content? This cannot be undone.',
    );
    if (!confirmed) return;
    await context.read<ReviewProvider>().remove(review.id);
    if (mounted) showSuccessSnackBar(context, 'Review removed.');
    _load();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(24),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Wrap(
            spacing: 12,
            runSpacing: 12,
            children: [
              SizedBox(
                width: 220,
                child: DropdownButtonFormField<int?>(
                  initialValue: _salonId,
                  isExpanded: true,
                  decoration: const InputDecoration(labelText: 'Salon', border: OutlineInputBorder(), isDense: true),
                  items: [
                    const DropdownMenuItem(value: null, child: Text('All salons')),
                    ..._salons.map((s) => DropdownMenuItem(value: s.id, child: Text(s.name, overflow: TextOverflow.ellipsis))),
                  ],
                  onChanged: (v) {
                    setState(() => _salonId = v);
                    _load();
                  },
                ),
              ),
              SizedBox(
                width: 180,
                child: DropdownButtonFormField<int?>(
                  initialValue: _minRating,
                  decoration: const InputDecoration(labelText: 'Min rating', border: OutlineInputBorder(), isDense: true),
                  items: const [
                    DropdownMenuItem(value: null, child: Text('Any')),
                    DropdownMenuItem(value: 1, child: Text('1+')),
                    DropdownMenuItem(value: 2, child: Text('2+')),
                    DropdownMenuItem(value: 3, child: Text('3+')),
                    DropdownMenuItem(value: 4, child: Text('4+')),
                    DropdownMenuItem(value: 5, child: Text('5')),
                  ],
                  onChanged: (v) {
                    setState(() => _minRating = v);
                    _load();
                  },
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: _isLoading
                ? const LoadingIndicator()
                : ListView.builder(
                    itemCount: _reviews.length,
                    itemBuilder: (context, index) {
                      final r = _reviews[index];
                      return Card(
                        child: ListTile(
                          title: Row(
                            children: List.generate(5, (i) => Icon(i < r.rating ? Icons.star : Icons.star_border, size: 16, color: Colors.amber)),
                          ),
                          subtitle: Text('${r.comment ?? ''}${r.salonReply != null ? '\nReply: ${r.salonReply}' : ''}'),
                          isThreeLine: r.salonReply != null,
                          trailing: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              IconButton(icon: const Icon(Icons.reply), onPressed: () => _reply(r)),
                              IconButton(icon: const Icon(Icons.delete, color: Colors.red), onPressed: () => _remove(r)),
                            ],
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
