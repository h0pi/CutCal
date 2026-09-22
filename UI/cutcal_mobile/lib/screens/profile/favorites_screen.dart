import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/api_client_exception.dart';
import '../../utils/utils_widgets.dart';

class FavoritesScreen extends StatefulWidget {
  const FavoritesScreen({super.key});

  @override
  State<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends State<FavoritesScreen> {
  List<FavoriteModel> _favorites = [];
  bool _isLoading = true;
  String? _loadError;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _isLoading = true;
      _loadError = null;
    });
    try {
      final favorites = await context.read<FavoriteProvider>().getMine();
      if (!mounted) return;
      setState(() {
        _favorites = favorites;
        _isLoading = false;
      });
    } on ApiClientException catch (e) {
      if (!mounted) return;
      setState(() {
        _loadError = e.message;
        _isLoading = false;
      });
    }
  }

  Future<void> _remove(FavoriteModel favorite) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Remove favorite',
      message: 'Remove ${favorite.salonName ?? 'this salon'} from your favorites?',
    );
    if (!confirmed) return;

    try {
      await context.read<FavoriteProvider>().removeSalon(favorite.salonId);
      if (mounted) {
        showSuccessSnackBar(context, 'Removed from favorites.');
        _load();
      }
    } on ApiClientException catch (e) {
      if (mounted) showErrorSnackBar(context, e.message);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Favorites')),
      body: _isLoading
          ? const LoadingIndicator()
          : _loadError != null
              ? ErrorState(message: _loadError!, onRetry: _load)
              : _favorites.isEmpty
                  ? const Center(child: Text('No favorite salons yet.'))
                  : ListView.builder(
                  itemCount: _favorites.length,
                  itemBuilder: (context, index) {
                    final favorite = _favorites[index];
                    return ListTile(
                      leading: const Icon(Icons.favorite, color: Colors.red),
                      title: Text(favorite.salonName ?? 'Salon #${favorite.salonId}'),
                      trailing: IconButton(
                        icon: const Icon(Icons.delete_outline),
                        onPressed: () => _remove(favorite),
                      ),
                    );
                  },
                ),
    );
  }
}
