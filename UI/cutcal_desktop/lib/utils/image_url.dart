import '../providers/auth_provider.dart';

/// Seed and uploaded images are stored as server-relative paths ("/images/..."),
/// so they are resolved against the configured API address. Absolute URLs pass through.
String resolveImageUrl(String url) {
  if (!url.startsWith('/')) return url;
  final base = AuthProvider.baseUrl;
  return '${base.endsWith('/') ? base.substring(0, base.length - 1) : base}$url';
}
