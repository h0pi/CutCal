import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/models.dart';
import 'auth_provider.dart';
import 'base_provider.dart';

class ReportProvider {
  Map<String, String> _headers() {
    final headers = <String, String>{'Content-Type': 'application/json'};
    if (AuthProvider.accessToken != null) {
      headers['Authorization'] = 'Bearer ${AuthProvider.accessToken}';
    }
    return headers;
  }

  Future<Map<String, dynamic>> getSummary({int? salonId, DateTime? dateFrom, DateTime? dateTo}) async {
    final params = <String, String>{};
    if (salonId != null) params['salonId'] = salonId.toString();
    if (dateFrom != null) params['dateFrom'] = dateFrom.toIso8601String();
    if (dateTo != null) params['dateTo'] = dateTo.toIso8601String();
    final uri = Uri.parse('${AuthProvider.baseUrl}Reports').replace(queryParameters: params);
    final response = await http.get(uri, headers: _headers());
    return jsonDecode(response.body);
  }

  Future<List<int>> downloadAppointmentsPdf({int? salonId, DateTime? dateFrom, DateTime? dateTo}) async {
    final params = <String, String>{};
    if (salonId != null) params['salonId'] = salonId.toString();
    if (dateFrom != null) params['dateFrom'] = dateFrom.toIso8601String();
    if (dateTo != null) params['dateTo'] = dateTo.toIso8601String();
    final uri = Uri.parse('${AuthProvider.baseUrl}Reports/Pdf/Appointments').replace(queryParameters: params);
    final response = await http.get(uri, headers: _headers());
    return response.bodyBytes;
  }

  Future<List<int>> downloadServicesPdf({int? salonId, int? month, int? year}) async {
    final params = <String, String>{};
    if (salonId != null) params['salonId'] = salonId.toString();
    if (month != null) params['month'] = month.toString();
    if (year != null) params['year'] = year.toString();
    final uri = Uri.parse('${AuthProvider.baseUrl}Reports/Pdf/Services').replace(queryParameters: params);
    final response = await http.get(uri, headers: _headers());
    return response.bodyBytes;
  }
}

class SalonCategoryProvider extends BaseProvider<SalonCategoryModel> {
  @override
  String getEndpoint() => 'SalonCategories';

  @override
  SalonCategoryModel fromJson(json) => SalonCategoryModel.fromJson(json);
}

class CityProvider extends BaseProvider<CityModel> {
  @override
  String getEndpoint() => 'Cities';

  @override
  CityModel fromJson(json) => CityModel.fromJson(json);
}

class SalonProvider extends BaseProvider<SalonModel> {
  @override
  String getEndpoint() => 'Salons';

  @override
  SalonModel fromJson(json) => SalonModel.fromJson(json);

  Future<List<SalonGalleryModel>> getGallery(int salonId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Gallery');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => SalonGalleryModel.fromJson(e)).toList();
  }
}

class SalonServiceProvider extends BaseProvider<SalonServiceModel> {
  @override
  String getEndpoint() => 'SalonServices';

  @override
  SalonServiceModel fromJson(json) => SalonServiceModel.fromJson(json);
}

class StaffProvider extends BaseProvider<StaffModel> {
  @override
  String getEndpoint() => 'Staff';

  @override
  StaffModel fromJson(json) => StaffModel.fromJson(json);
}

class AppointmentProvider extends BaseProvider<AppointmentModel> {
  @override
  String getEndpoint() => 'Appointments';

  @override
  AppointmentModel fromJson(json) => AppointmentModel.fromJson(json);

  Future<AppointmentModel> cancel(int id, String reason) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Appointments/$id/Cancel');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode({'reason': reason}));
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<AppointmentModel> confirm(int id) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Appointments/$id/Confirm');
    final response = await http.put(uri, headers: createHeaders());
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<AppointmentModel> complete(int id) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Appointments/$id/Complete');
    final response = await http.put(uri, headers: createHeaders());
    final data = validateResponse(response);
    return fromJson(data);
  }
}

class ReviewProvider extends BaseProvider<ReviewModel> {
  @override
  String getEndpoint() => 'Reviews';

  @override
  ReviewModel fromJson(json) => ReviewModel.fromJson(json);

  Future<ReviewModel> reply(int id, String reply) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Reviews/$id/Reply');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode({'reply': reply}));
    final data = validateResponse(response);
    return fromJson(data);
  }
}

class NotificationProvider extends BaseProvider<NotificationModel> {
  @override
  String getEndpoint() => 'Notifications';

  @override
  NotificationModel fromJson(json) => NotificationModel.fromJson(json);

  Future<void> markRead(int id) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Notifications/$id/MarkRead');
    final response = await http.put(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }

  Future<void> markAllRead() async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Notifications/MarkAllRead');
    final response = await http.put(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }
}

class RecommendationProvider extends BaseProvider<RecommendationModel> {
  @override
  String getEndpoint() => 'Recommendations';

  @override
  RecommendationModel fromJson(json) => RecommendationModel.fromJson(json);

  Future<List<RecommendationModel>> getRecommendations({double? lat, double? lng}) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Recommendations${getQueryString({'lat': lat, 'lng': lng})}');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => RecommendationModel.fromJson(e)).toList();
  }
}

class PaymentProvider extends BaseProvider<Map<String, dynamic>> {
  @override
  String getEndpoint() => 'Payments';

  @override
  Map<String, dynamic> fromJson(json) => Map<String, dynamic>.from(json);

  Future<Map<String, dynamic>> createOrder(int appointmentId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Payments/CreateOrder');
    final response = await http.post(uri, headers: createHeaders(), body: jsonEncode({'appointmentId': appointmentId}));
    return Map<String, dynamic>.from(validateResponse(response));
  }

  Future<Map<String, dynamic>> captureOrder(String paypalOrderId, int appointmentId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Payments/CaptureOrder');
    final response = await http.post(
      uri,
      headers: createHeaders(),
      body: jsonEncode({'paypalOrderId': paypalOrderId, 'appointmentId': appointmentId}),
    );
    return Map<String, dynamic>.from(validateResponse(response));
  }
}

class FavoriteProvider extends BaseProvider<FavoriteModel> {
  @override
  String getEndpoint() => 'Favorites';

  @override
  FavoriteModel fromJson(json) => FavoriteModel.fromJson(json);

  Future<List<FavoriteModel>> getMine() async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Favorites');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => FavoriteModel.fromJson(e)).toList();
  }

  Future<void> add(int salonId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Favorites/$salonId');
    final response = await http.post(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }

  Future<void> removeSalon(int salonId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Favorites/$salonId');
    final response = await http.delete(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }
}

class UserProvider extends BaseProvider<UserModel> {
  @override
  String getEndpoint() => 'Users';

  @override
  UserModel fromJson(json) => UserModel.fromJson(json);

  Future<void> changePassword(int userId, String oldPassword, String newPassword) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Users/$userId/ChangePassword');
    final response = await http.put(
      uri,
      headers: createHeaders(),
      body: jsonEncode({
        'oldPassword': oldPassword,
        'newPassword': newPassword,
        'confirmNewPassword': newPassword,
      }),
    );
    validateResponse(response, allowEmpty: true);
  }
}
