import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../models/models.dart';
import '../utils/api_client_exception.dart';
import 'auth_provider.dart';

abstract class BaseProvider<T> with ChangeNotifier {
  static String? baseUrl = AuthProvider.baseUrl;

  String getEndpoint();

  T fromJson(dynamic json);

  Future<PagedResult<T>> get({Map<String, dynamic>? filter}) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}${getEndpoint()}${getQueryString(filter ?? {})}');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response);
    return PagedResult<T>.fromJson(data, (item) => fromJson(item));
  }

  Future<T> getById(int id) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}${getEndpoint()}/$id');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<T> insert(dynamic request) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}${getEndpoint()}');
    final response = await http.post(uri, headers: createHeaders(), body: jsonEncode(request));
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<T> update(int id, dynamic request) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}${getEndpoint()}/$id');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode(request));
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<void> remove(int id) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}${getEndpoint()}/$id');
    final response = await http.delete(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }

  Map<String, String> createHeaders() {
    final headers = <String, String>{'Content-Type': 'application/json'};
    if (AuthProvider.accessToken != null) {
      headers['Authorization'] = 'Bearer ${AuthProvider.accessToken}';
    }
    return headers;
  }

  dynamic validateResponse(http.Response response, {bool allowEmpty = false}) {
    if (response.statusCode == 401) {
      throw ApiClientException('Session expired, please log in again.', statusCode: 401);
    }
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (allowEmpty || response.body.isEmpty) return null;
      return jsonDecode(response.body);
    }

    throw ApiClientException(_extractMessage(response), statusCode: response.statusCode);
  }

  String _extractMessage(http.Response response) {
    try {
      final json = jsonDecode(response.body);
      if (json is Map && json['message'] != null) return json['message'];
      if (json is Map && json['errors'] != null) return (json['errors'] as List).join(', ');
    } catch (_) {}
    return 'Request failed (${response.statusCode})';
  }

  String getQueryString(Map<String, dynamic> params) {
    final filtered = <String, String>{};
    params.forEach((key, value) {
      if (value != null) filtered[key] = value.toString();
    });
    if (filtered.isEmpty) return '';
    return '?${Uri(queryParameters: filtered).query}';
  }
}
