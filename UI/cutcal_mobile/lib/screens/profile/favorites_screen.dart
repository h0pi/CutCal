import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../models/models.dart';
import '../../providers/entity_providers.dart';
import '../../utils/utils_widgets.dart';

class FavoritesScreen extends StatefulWidget {
  const FavoritesScreen({super.key});

  @override
  State<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends State<FavoritesScreen> {
  List<FavoriteModel> _favorites = [];
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() => _isLoading = true);
    final favorites = await context.read<FavoriteProvider>().getMine();
    setState(() {
      _favorites = favorites;
      _isLoading = false;
    });
  }

  Future<void> _remove(FavoriteModel favorite) async {
    final confirmed = await showConfirmationDialog(
      context,
      title: 'Remove favorite',
      message: 'Remove ${favorite.salonName ?? 'this salon'} from your favorites?',
    );
    if (!confirmed) return;

    await context.read<FavoriteProvider>().removeSalon(favorite.salonId);
    if (mounted) {
      showSuccessSnackBar(context, 'Removed from favorites.');
      _load();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Favorites')),
      body: _isLoading
          ? const LoadingIndicator()
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
