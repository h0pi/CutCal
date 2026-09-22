import 'dart:convert';

import 'package:http/http.dart' as http;

import '../models/models.dart';
import 'auth_provider.dart';
import 'base_provider.dart';

class SalonCategoryProvider extends BaseProvider<SalonCategoryModel> {
  @override
  String getEndpoint() => 'SalonCategories';

  @override
  SalonCategoryModel fromJson(json) => SalonCategoryModel.fromJson(json);
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

  Future<void> logView(int salonId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/View');
    final response = await http.post(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
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

  Future<AppointmentModel> reschedule(int id, DateTime scheduledAt) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Appointments/$id/Reschedule');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode({'scheduledAt': scheduledAt.toIso8601String()}));
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

class AvailabilityProvider extends BaseProvider<AvailabilityDayModel> {
  @override
  String getEndpoint() => 'Salons';

  @override
  AvailabilityDayModel fromJson(json) => AvailabilityDayModel.fromJson(json);

  String _dateOnly(DateTime d) => '${d.year.toString().padLeft(4, '0')}-${d.month.toString().padLeft(2, '0')}-${d.day.toString().padLeft(2, '0')}';

  Future<List<AvailabilityDayModel>> getDays({
    required int salonId,
    required int serviceId,
    required int staffId,
    required DateTime from,
    required int days,
  }) async {
    final query = getQueryString({
      'serviceId': serviceId,
      'staffId': staffId,
      'from': _dateOnly(from),
      'days': days,
      'nowLocal': DateTime.now().toIso8601String(),
    });
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Availability/Days$query');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => AvailabilityDayModel.fromJson(e)).toList();
  }

  Future<List<AvailabilitySlotModel>> getSlots({
    required int salonId,
    required int serviceId,
    required int staffId,
    required DateTime date,
    int? excludeAppointmentId,
  }) async {
    final query = getQueryString({
      'serviceId': serviceId,
      'staffId': staffId,
      'date': _dateOnly(date),
      'excludeAppointmentId': excludeAppointmentId,
      'nowLocal': DateTime.now().toIso8601String(),
    });
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Availability/Slots$query');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => AvailabilitySlotModel.fromJson(e)).toList();
  }
}

class ReportProvider extends BaseProvider<Map<String, dynamic>> {
  @override
  String getEndpoint() => 'Reports';

  @override
  Map<String, dynamic> fromJson(json) => Map<String, dynamic>.from(json);

  Future<Map<String, dynamic>> getSummary({DateTime? dateFrom, DateTime? dateTo}) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Reports${getQueryString({
          'dateFrom': dateFrom?.toIso8601String(),
          'dateTo': dateTo?.toIso8601String(),
        })}');
    final response = await http.get(uri, headers: createHeaders());
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

class GeocodingProvider extends BaseProvider<GeocodeResultModel> {
  @override
  String getEndpoint() => 'Geocoding';

  @override
  GeocodeResultModel fromJson(json) => GeocodeResultModel.fromJson(json);

  Future<List<GeocodeResultModel>> search(String query) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Geocoding/Search${getQueryString({'query': query})}');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response) as List;
    return data.map((e) => GeocodeResultModel.fromJson(e)).toList();
  }
}
