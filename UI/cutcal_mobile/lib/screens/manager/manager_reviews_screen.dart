import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/app_theme.dart';
import '../../utils/utils_widgets.dart';

class ManagerReviewsScreen extends StatefulWidget {
  final int salonId;

  const ManagerReviewsScreen({super.key, required this.salonId});

  @override
  State<ManagerReviewsScreen> createState() => _ManagerReviewsScreenState();
}

class _ManagerReviewsScreenState extends State<ManagerReviewsScreen> {
  List<ReviewModel> _reviews = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final result = await context.read<ReviewProvider>().get(filter: {'salonId': widget.salonId, 'pageSize': 200});
    if (!mounted) return;
    setState(() {
      _reviews = result.items;
      _isLoading = false;
    });
  }

  Future<void> _reply(ReviewModel review) async {
    final controller = TextEditingController(text: review.salonReply);
    final reply = await showDialog<String>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Reply to review'),
        content: TextField(controller: controller, maxLines: 3, autofocus: true, decoration: const InputDecoration(labelText: 'Your reply', border: OutlineInputBorder())),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Cancel')),
          FilledButton(onPressed: () => Navigator.of(context).pop(controller.text.trim()), child: const Text('Send')),
        ],
      ),
    );
    if (reply == null || reply.isEmpty) return;

    try {
      await context.read<ReviewProvider>().reply(review.id, reply);
      if (mounted) showSuccessSnackBar(context, 'Reply sent.');
      _load();
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    final avg = _reviews.isEmpty ? 0.0 : _reviews.map((r) => r.rating).reduce((a, b) => a + b) / _reviews.length;
    final counts = List<int>.filled(6, 0); // index 1..5 used
    for (final r in _reviews) {
      if (r.rating >= 1 && r.rating <= 5) counts[r.rating]++;
    }
    final maxCount = counts.reduce((a, b) => a > b ? a : b);

    return Scaffold(
      backgroundColor: AppColors.background,
      appBar: AppBar(title: const Text('Reviews')),
      body: _isLoading
          ? const LoadingIndicator()
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(18),
                    border: Border.all(color: const Color(0xFFEDEAF4)),
                  ),
                  child: Row(
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Column(
                        children: [
                          Text(avg.toStringAsFixed(1), style: const TextStyle(fontSize: 32, fontWeight: FontWeight.bold, color: AppColors.textPrimary)),
                          Row(
                            mainAxisSize: MainAxisSize.min,
                            children: List.generate(5, (i) => Icon(i < avg.round() ? Icons.star : Icons.star_border, size: 14, color: AppColors.starColor)),
                          ),
                          const SizedBox(height: 2),
                          Text('${_reviews.length} reviews', style: const TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                        ],
                      ),
                      const SizedBox(width: 20),
                      Expanded(
                        child: Column(
                          children: List.generate(5, (i) {
                            final star = 5 - i;
                            final count = counts[star];
                            final fraction = maxCount == 0 ? 0.0 : count / maxCount;
                            return Padding(
                              padding: const EdgeInsets.symmetric(vertical: 2),
                              child: Row(
                                children: [
                                  Text('$star', style: const TextStyle(fontSize: 11, color: AppColors.textSecondary)),
                                  const SizedBox(width: 4),
                                  Expanded(
                                    child: ClipRRect(
                                      borderRadius: BorderRadius.circular(4),
                                      child: LinearProgressIndicator(
                                        value: fraction,
                                        minHeight: 6,
                                        backgroundColor: AppColors.surfaceMuted,
                                        color: AppColors.starColor,
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 6),
                                  SizedBox(width: 18, child: Text('$count', style: const TextStyle(fontSize: 11, color: AppColors.textSecondary))),
                                ],
                              ),
                            );
                          }),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 20),
                if (_reviews.isEmpty)
                  const Padding(
                    padding: EdgeInsets.symmetric(vertical: 24),
                    child: Center(child: Text('No reviews yet.', style: TextStyle(color: AppColors.textSecondary))),
                  )
                else
                  ..._reviews.map((r) => Container(
                        margin: const EdgeInsets.only(bottom: 12),
                        padding: const EdgeInsets.all(14),
                        decoration: BoxDecoration(
                          color: Colors.white,
                          borderRadius: BorderRadius.circular(16),
                          border: Border.all(color: const Color(0xFFEDEAF4)),
                        ),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Row(
                              children: [
                                Expanded(child: Text(r.customerName ?? 'Customer', style: const TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary))),
                                Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: List.generate(5, (i) => Icon(i < r.rating ? Icons.star : Icons.star_border, size: 14, color: AppColors.starColor)),
                                ),
                              ],
                            ),
                            if (r.comment != null && r.comment!.isNotEmpty) ...[
                              const SizedBox(height: 6),
                              Text(r.comment!, style: const TextStyle(color: AppColors.textPrimary, fontSize: 13)),
                            ],
                            if (r.salonReply != null) ...[
                              const SizedBox(height: 10),
                              Container(
                                width: double.infinity,
                                padding: const EdgeInsets.all(10),
                                decoration: BoxDecoration(color: AppColors.primaryLight, borderRadius: BorderRadius.circular(10)),
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    const Text('Your reply', style: TextStyle(color: AppColors.primaryDark, fontWeight: FontWeight.bold, fontSize: 11)),
                                    const SizedBox(height: 2),
                                    Text(r.salonReply!, style: const TextStyle(color: AppColors.primaryDark, fontSize: 12)),
                                  ],
                                ),
                              ),
                            ],
                            const SizedBox(height: 8),
                            Align(
                              alignment: Alignment.centerRight,
                              child: TextButton(onPressed: () => _reply(r), child: Text(r.salonReply == null ? 'Reply' : 'Edit reply')),
                            ),
                          ],
                        ),
                      )),
              ],
            ),
    );
  }
}
