import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;
import 'package:jwt_decoder/jwt_decoder.dart';

import '../utils/api_client_exception.dart';

class AuthProvider with ChangeNotifier {
  /// API address, set at build/run time: `--dart-define=API_BASE_URL=http://host:port`.
  /// A trailing slash is optional; it is added when missing.
  static String baseUrl = _withTrailingSlash(const String.fromEnvironment('API_BASE_URL', defaultValue: 'http://10.0.2.2:5194'));

  static String _withTrailingSlash(String url) => url.endsWith('/') ? url : '$url/';

  static String? accessToken;
  static String? refreshToken;
  static Map<String, dynamic>? accessTokenDecoded;

  static const _roleClaimUri = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  int? get userId => accessTokenDecoded == null ? null : int.tryParse(accessTokenDecoded!['Id'].toString());
  String? get role => accessTokenDecoded?[_roleClaimUri] ?? accessTokenDecoded?['role'];
  bool get isLoggedIn => accessToken != null;

  Future<bool> login(String username, String password) async {
    final uri = Uri.parse('${baseUrl}Access/Login');
    final response = await http.post(
      uri,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'username': username, 'password': password}),
    );

    if (response.statusCode != 200) {
      throw ApiClientException(_extractMessage(response), statusCode: response.statusCode);
    }

    final json = jsonDecode(response.body);
    _applyLoginResponse(json);
    notifyListeners();
    return true;
  }

  Future<bool> register(Map<String, dynamic> request) async {
    final uri = Uri.parse('${baseUrl}Access/Register');
    final response = await http.post(
      uri,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode(request),
    );

    if (response.statusCode != 200) {
      throw ApiClientException(_extractMessage(response), statusCode: response.statusCode);
    }
    return true;
  }

  Future<bool> refreshTokenLogin() async {
    if (refreshToken == null) return false;

    final uri = Uri.parse('${baseUrl}Access/LoginWithRefreshToken');
    final response = await http.post(
      uri,
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'refreshToken': refreshToken}),
    );

    if (response.statusCode != 200) {
      _clearSession();
      return false;
    }

    final json = jsonDecode(response.body);
    _applyLoginResponse(json);
    notifyListeners();
    return true;
  }

  void _applyLoginResponse(Map<String, dynamic> json) {
    accessToken = json['accessToken'];
    refreshToken = json['refreshToken'];
    accessTokenDecoded = JwtDecoder.decode(accessToken!);
  }

  /// Ends the session locally right away, then tells the server to invalidate the tokens.
  Future<void> logout() async {
    final access = accessToken;
    final refresh = refreshToken;
    _clearSession();
    if (access == null) return;

    try {
      await http.post(
        Uri.parse('${baseUrl}Access/Logout'),
        headers: {'Content-Type': 'application/json', 'Authorization': 'Bearer $access'},
        body: jsonEncode({'refreshToken': refresh}),
      );
    } on Exception catch (e) {
      debugPrint('Server-side logout failed: $e');
    }
  }

  void _clearSession() {
    accessToken = null;
    refreshToken = null;
    accessTokenDecoded = null;
    notifyListeners();
  }

  String _extractMessage(http.Response response) {
    try {
      final json = jsonDecode(response.body);
      if (json is Map && json['message'] != null) return json['message'];
      if (json is Map && json['errors'] != null) return (json['errors'] as List).join(', ');
    } catch (_) {}
    return 'Request failed (${response.statusCode})';
  }
}
