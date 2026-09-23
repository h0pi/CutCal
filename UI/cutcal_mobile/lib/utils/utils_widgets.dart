import 'package:flutter/material.dart';

import 'app_theme.dart';

Future<bool> showConfirmationDialog(
  BuildContext context, {
  required String title,
  required String message,
  String confirmLabel = 'Confirm',
  bool isDestructive = true,
}) async {
  final result = await showDialog<bool>(
    context: context,
    builder: (context) => AlertDialog(
      title: Text(title),
      content: Text(message),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(false),
          child: const Text('Cancel'),
        ),
        TextButton(
          style: TextButton.styleFrom(
            foregroundColor: isDestructive ? Colors.red : null,
          ),
          onPressed: () => Navigator.of(context).pop(true),
          child: Text(confirmLabel),
        ),
      ],
    ),
  );
  return result ?? false;
}

/// Shared "why are you cancelling" dialog: blocks closing until a non-empty
/// reason is entered, showing an inline validation error instead of doing nothing.
Future<String?> showCancelReasonDialog(BuildContext context) async {
  final reasonController = TextEditingController();
  String? errorText;

  return showDialog<String>(
    context: context,
    builder: (context) => StatefulBuilder(
      builder: (context, setDialogState) => AlertDialog(
        title: const Text('Cancel appointment'),
        content: TextField(
          controller: reasonController,
          autofocus: true,
          decoration: InputDecoration(
            labelText: 'Reason for cancellation',
            border: const OutlineInputBorder(),
            errorText: errorText,
          ),
          maxLines: 2,
          onChanged: (_) {
            if (errorText != null) setDialogState(() => errorText = null);
          },
        ),
        actions: [
          TextButton(onPressed: () => Navigator.of(context).pop(), child: const Text('Back')),
          TextButton(
            style: TextButton.styleFrom(foregroundColor: Colors.red),
            onPressed: () {
              if (reasonController.text.trim().isEmpty) {
                setDialogState(() => errorText = 'Please tell us why you are cancelling.');
                return;
              }
              Navigator.of(context).pop(reasonController.text.trim());
            },
            child: const Text('Cancel appointment'),
          ),
        ],
      ),
    ),
  );
}

void showSuccessSnackBar(BuildContext context, String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(content: Text(message), backgroundColor: Colors.green),
  );
}

void showErrorSnackBar(BuildContext context, String message) {
  ScaffoldMessenger.of(context).showSnackBar(
    SnackBar(content: Text(message), backgroundColor: Colors.red),
  );
}

class LoadingIndicator extends StatelessWidget {
  const LoadingIndicator({super.key});

  @override
  Widget build(BuildContext context) => const Center(child: CircularProgressIndicator());
}

/// Shown in place of a screen's content when its initial load fails (server
/// down, no connection, timeout), with a button to retry the same load call.
class ErrorState extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const ErrorState({super.key, required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(message, textAlign: TextAlign.center, style: const TextStyle(color: AppColors.textSecondary)),
            const SizedBox(height: 12),
            OutlinedButton(
              style: OutlinedButton.styleFrom(minimumSize: const Size(120, 44)),
              onPressed: onRetry,
              child: const Text('Try again'),
            ),
          ],
        ),
      ),
    );
  }
}

/// Small uppercase status pill (PENDING / CONFIRMED / COMPLETED / DECLINED / PAID / UPCOMING),
/// styled per the design reference: solid pale background, no border, bold uppercase text.
class StatusBadge extends StatelessWidget {
  final String status;

  const StatusBadge({super.key, required this.status});

  (Color, Color) _colorsFor(String status) {
    switch (status) {
      case 'Pending':
        return (AppColors.pendingText, AppColors.pendingBg);
      case 'Confirmed':
      case 'Upcoming':
        return (AppColors.confirmedText, AppColors.confirmedBg);
      case 'Completed':
      case 'Paid':
        return (AppColors.completedText, AppColors.completedBg);
      case 'Cancelled':
      case 'Declined':
        return (AppColors.declinedText, AppColors.declinedBg);
      default:
        return (AppColors.neutralText, AppColors.neutralBg);
    }
  }

  @override
  Widget build(BuildContext context) {
    final (text, bg) = _colorsFor(status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(color: bg, borderRadius: BorderRadius.circular(20)),
      child: Text(
        status.toUpperCase(),
        style: TextStyle(color: text, fontWeight: FontWeight.bold, fontSize: 11, letterSpacing: 0.3),
      ),
    );
  }
}

/// Equal-width pill segmented control (e.g. "Upcoming | Completed"), matching the
/// design reference: grey track, animated white pill behind the selected label.
class SegmentedTabs extends StatelessWidget {
  final List<String> labels;
  final int selectedIndex;
  final ValueChanged<int> onChanged;

  const SegmentedTabs({super.key, required this.labels, required this.selectedIndex, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(4),
      decoration: BoxDecoration(color: AppColors.surfaceMuted, borderRadius: BorderRadius.circular(16)),
      child: Row(
        children: List.generate(labels.length, (i) {
          final selected = i == selectedIndex;
          return Expanded(
            child: GestureDetector(
              onTap: () => onChanged(i),
              child: AnimatedContainer(
                duration: const Duration(milliseconds: 150),
                padding: const EdgeInsets.symmetric(vertical: 10),
                decoration: BoxDecoration(
                  color: selected ? Colors.white : Colors.transparent,
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: selected ? const [BoxShadow(color: Colors.black12, blurRadius: 4, offset: Offset(0, 1))] : null,
                ),
                alignment: Alignment.center,
                child: Text(
                  labels[i],
                  style: TextStyle(
                    fontWeight: selected ? FontWeight.bold : FontWeight.w500,
                    color: selected ? AppColors.textPrimary : AppColors.textSecondary,
                    fontSize: 13,
                  ),
                ),
              ),
            ),
          );
        }),
      ),
    );
  }
}

/// Scrollable filter chip row (e.g. "All / Pending / Confirmed / Completed / Declined"),
/// selected = solid near-black pill, unselected = white pill with a light grey border.
class FilterChipRow extends StatelessWidget {
  final List<String> labels;
  final int selectedIndex;
  final ValueChanged<int> onChanged;

  const FilterChipRow({super.key, required this.labels, required this.selectedIndex, required this.onChanged});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 40,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 4),
        itemCount: labels.length,
        separatorBuilder: (_, __) => const SizedBox(width: 8),
        itemBuilder: (context, i) {
          final selected = i == selectedIndex;
          return GestureDetector(
            onTap: () => onChanged(i),
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
              decoration: BoxDecoration(
                color: selected ? AppColors.textPrimary : Colors.white,
                borderRadius: BorderRadius.circular(20),
                border: selected ? null : Border.all(color: const Color(0xFFE5E1EF)),
              ),
              alignment: Alignment.center,
              child: Text(
                labels[i],
                style: TextStyle(
                  color: selected ? Colors.white : AppColors.textSecondary,
                  fontWeight: FontWeight.w600,
                  fontSize: 13,
                ),
              ),
            ),
          );
        },
      ),
    );
  }
}

/// "Featured" pill used as an overlay on salon cover photos for admin-promoted salons.
class FeaturedBadge extends StatelessWidget {
  const FeaturedBadge({super.key});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(color: AppColors.starColor, borderRadius: BorderRadius.circular(20)),
      child: const Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(Icons.bolt, size: 13, color: Colors.white),
          SizedBox(width: 4),
          Text('Featured', style: TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.bold)),
        ],
      ),
    );
  }
}

/// Dark "★ 4.8" pill used as an overlay on salon cover photos.
class RatingBadge extends StatelessWidget {
  final double rating;

  const RatingBadge({super.key, required this.rating});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 5),
      decoration: BoxDecoration(color: AppColors.ratingBadge.withValues(alpha: 0.85), borderRadius: BorderRadius.circular(20)),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.star, size: 13, color: AppColors.starColor),
          const SizedBox(width: 4),
          Text(rating.toStringAsFixed(1), style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.bold)),
        ],
      ),
    );
  }
}
