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

class CountryProvider extends BaseProvider<CountryModel> {
  @override
  String getEndpoint() => 'Countries';

  @override
  CountryModel fromJson(json) => CountryModel.fromJson(json);
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

  Future<SalonGalleryModel> addGalleryImage(int salonId, String imageUrl, {String? caption}) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Gallery');
    final response = await http.post(uri, headers: createHeaders(), body: jsonEncode({'imageUrl': imageUrl, 'caption': caption}));
    final data = validateResponse(response);
    return SalonGalleryModel.fromJson(data);
  }

  Future<void> removeGalleryImage(int salonId, int imageId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Gallery/$imageId');
    final response = await http.delete(uri, headers: createHeaders());
    validateResponse(response, allowEmpty: true);
  }

  Future<SalonModel> approve(int salonId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Approve');
    final response = await http.post(uri, headers: createHeaders());
    final data = validateResponse(response);
    return fromJson(data);
  }

  Future<SalonModel> setFeatured(int salonId, bool featured) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Salons/$salonId/Feature');
    final response = await http.post(uri, headers: createHeaders(), body: jsonEncode({'featured': featured}));
    final data = validateResponse(response);
    return fromJson(data);
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

  Future<AppointmentModel> reassignStaff(int id, int staffId) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Appointments/$id/ReassignStaff');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode({'staffId': staffId}));
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

  Future<UserModel> setRole(int userId, String role) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Users/$userId/Role');
    final response = await http.put(uri, headers: createHeaders(), body: jsonEncode({'role': role}));
    final data = validateResponse(response);
    return fromJson(data);
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

  Future<GeocodeResultModel> reverse(double latitude, double longitude) async {
    final uri = Uri.parse('${AuthProvider.baseUrl}Geocoding/Reverse${getQueryString({'lat': latitude, 'lon': longitude})}');
    final response = await http.get(uri, headers: createHeaders());
    final data = validateResponse(response);
    return GeocodeResultModel.fromJson(data);
  }
}
