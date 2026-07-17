import 'package:flutter/material.dart';
import 'package:flutter_rating_bar/flutter_rating_bar.dart';
import 'package:provider/provider.dart';

import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class AddReviewScreen extends StatefulWidget {
  final int appointmentId;

  const AddReviewScreen({super.key, required this.appointmentId});

  @override
  State<AddReviewScreen> createState() => _AddReviewScreenState();
}

class _AddReviewScreenState extends State<AddReviewScreen> {
  double _rating = 5;
  final _commentController = TextEditingController();
  bool _isLoading = false;

  Future<void> _submit() async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Submit review',
      message: 'Submit this review? You will not be able to edit it afterwards.',
      confirmLabel: 'Submit',
      isDestructive: false,
    );
    if (!confirmed) return;

    setState(() => _isLoading = true);
    try {
      await context.read<ReviewProvider>().insert({
        'appointmentId': widget.appointmentId,
        'rating': _rating.round(),
        'comment': _commentController.text,
      });
      if (mounted) {
        showSuccessSnackBar(context, 'Thanks for your review!');
        Navigator.of(context).pop();
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Leave a review')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const Text('How was your experience?'),
            const SizedBox(height: 16),
            RatingBar.builder(
              initialRating: _rating,
              minRating: 1,
              itemCount: 5,
              itemBuilder: (context, _) => const Icon(Icons.star, color: Colors.amber),
              onRatingUpdate: (value) => setState(() => _rating = value),
            ),
            const SizedBox(height: 24),
            TextField(
              controller: _commentController,
              maxLines: 4,
              decoration: const InputDecoration(labelText: 'Comment', border: OutlineInputBorder()),
            ),
            const SizedBox(height: 24),
            SizedBox(
              width: double.infinity,
              child: FilledButton(
                onPressed: _isLoading ? null : _submit,
                child: _isLoading ? const LoadingIndicator() : const Text('Submit review'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
