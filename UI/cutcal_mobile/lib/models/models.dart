class PagedResult<T> {
  final List<T> items;
  final int totalCount;

  PagedResult({required this.items, required this.totalCount});

  factory PagedResult.fromJson(dynamic json, T Function(dynamic) fromJsonT) {
    final itemsJson = (json['items'] ?? json['Items'] ?? []) as List;
    return PagedResult<T>(
      items: itemsJson.map((e) => fromJsonT(e)).toList(),
      totalCount: json['totalCount'] ?? json['TotalCount'] ?? itemsJson.length,
    );
  }
}

class UserModel {
  final int id;
  final String username;
  final String firstName;
  final String lastName;
  final String email;
  final String? phone;
  final String? profileImageUrl;
  final bool isActive;
  final List<String> roles;

  UserModel({
    required this.id,
    required this.username,
    required this.firstName,
    required this.lastName,
    required this.email,
    this.phone,
    this.profileImageUrl,
    this.isActive = true,
    this.roles = const [],
  });

  factory UserModel.fromJson(Map<String, dynamic> json) => UserModel(
        id: json['id'],
        username: json['username'] ?? '',
        firstName: json['firstName'] ?? '',
        lastName: json['lastName'] ?? '',
        email: json['email'] ?? '',
        phone: json['phone'],
        profileImageUrl: json['profileImageUrl'],
        isActive: json['isActive'] ?? true,
        roles: (json['roles'] as List?)?.map((e) => e.toString()).toList() ?? [],
      );
}

class SalonCategoryModel {
  final int id;
  final String name;

  SalonCategoryModel({required this.id, required this.name});

  factory SalonCategoryModel.fromJson(Map<String, dynamic> json) =>
      SalonCategoryModel(id: json['id'], name: json['name'] ?? '');

  Map<String, dynamic> toJson() => {'name': name};
}

class CityModel {
  final int id;
  final String name;
  final String country;

  CityModel({required this.id, required this.name, required this.country});

  factory CityModel.fromJson(Map<String, dynamic> json) =>
      CityModel(id: json['id'], name: json['name'] ?? '', country: json['country'] ?? '');

  Map<String, dynamic> toJson() => {'name': name, 'country': country};
}

class SalonWorkingHoursModel {
  final int? id;
  final int dayOfWeek;
  final String? openTime;
  final String? closeTime;
  final bool isClosed;

  SalonWorkingHoursModel({this.id, required this.dayOfWeek, this.openTime, this.closeTime, this.isClosed = false});

  factory SalonWorkingHoursModel.fromJson(Map<String, dynamic> json) => SalonWorkingHoursModel(
        id: json['id'],
        dayOfWeek: json['dayOfWeek'] ?? 0,
        openTime: json['openTime']?.toString(),
        closeTime: json['closeTime']?.toString(),
        isClosed: json['isClosed'] ?? false,
      );

  Map<String, dynamic> toJson() => {
        'dayOfWeek': dayOfWeek,
        'openTime': openTime,
        'closeTime': closeTime,
        'isClosed': isClosed,
      };
}

class SalonGalleryModel {
  final int id;
  final int salonId;
  final String imageUrl;
  final String? caption;

  SalonGalleryModel({required this.id, required this.salonId, required this.imageUrl, this.caption});

  factory SalonGalleryModel.fromJson(Map<String, dynamic> json) => SalonGalleryModel(
        id: json['id'],
        salonId: json['salonId'],
        imageUrl: json['imageUrl'] ?? '',
        caption: json['caption'],
      );
}

class SalonModel {
  final int id;
  final String name;
  final int salonCategoryId;
  final String? salonCategoryName;
  final String? description;
  final String address;
  final int cityId;
  final String? cityName;
  final double latitude;
  final double longitude;
  final String? phone;
  final String? email;
  final String? profileImageUrl;
  final double avgRating;
  final bool isApproved;
  final bool autoConfirm;
  final double? distanceKm;
  final List<SalonWorkingHoursModel> workingHours;

  SalonModel({
    required this.id,
    required this.name,
    required this.salonCategoryId,
    this.salonCategoryName,
    this.description,
    required this.address,
    required this.cityId,
    this.cityName,
    required this.latitude,
    required this.longitude,
    this.phone,
    this.email,
    this.profileImageUrl,
    this.avgRating = 0,
    this.isApproved = false,
    this.autoConfirm = false,
    this.distanceKm,
    this.workingHours = const [],
  });

  factory SalonModel.fromJson(Map<String, dynamic> json) => SalonModel(
        id: json['id'],
        name: json['name'] ?? '',
        salonCategoryId: json['salonCategoryId'] ?? 0,
        salonCategoryName: json['salonCategoryName'],
        description: json['description'],
        address: json['address'] ?? '',
        cityId: json['cityId'] ?? 0,
        cityName: json['cityName'],
        latitude: (json['latitude'] ?? 0).toDouble(),
        longitude: (json['longitude'] ?? 0).toDouble(),
        phone: json['phone'],
        email: json['email'],
        profileImageUrl: json['profileImageUrl'],
        avgRating: (json['avgRating'] ?? 0).toDouble(),
        isApproved: json['isApproved'] ?? false,
        autoConfirm: json['autoConfirm'] ?? false,
        distanceKm: json['distanceKm'] == null ? null : (json['distanceKm']).toDouble(),
        workingHours: (json['workingHours'] as List? ?? [])
            .map((e) => SalonWorkingHoursModel.fromJson(e))
            .toList(),
      );
}

class SalonServiceModel {
  final int id;
  final int salonId;
  final String name;
  final String? description;
  final int durationMinutes;
  final double price;
  final bool isActive;

  SalonServiceModel({
    required this.id,
    required this.salonId,
    required this.name,
    this.description,
    required this.durationMinutes,
    required this.price,
    this.isActive = true,
  });

  factory SalonServiceModel.fromJson(Map<String, dynamic> json) => SalonServiceModel(
        id: json['id'],
        salonId: json['salonId'],
        name: json['name'] ?? '',
        description: json['description'],
        durationMinutes: json['durationMinutes'] ?? 30,
        price: (json['price'] ?? 0).toDouble(),
        isActive: json['isActive'] ?? true,
      );
}

class StaffModel {
  final int id;
  final int salonId;
  final int userId;
  final String? fullName;
  final String role;
  final String? bio;
  final String? profileImageUrl;
  final bool isActive;
  final List<int> serviceIds;

  StaffModel({
    required this.id,
    required this.salonId,
    required this.userId,
    this.fullName,
    required this.role,
    this.bio,
    this.profileImageUrl,
    this.isActive = true,
    this.serviceIds = const [],
  });

  factory StaffModel.fromJson(Map<String, dynamic> json) => StaffModel(
        id: json['id'],
        salonId: json['salonId'],
        userId: json['userId'],
        fullName: json['fullName'],
        role: json['role'] ?? '',
        bio: json['bio'],
        profileImageUrl: json['profileImageUrl'],
        isActive: json['isActive'] ?? true,
        serviceIds: (json['serviceIds'] as List? ?? []).map((e) => e as int).toList(),
      );
}

class AppointmentModel {
  final int id;
  final int customerId;
  final String? customerName;
  final int salonId;
  final String? salonName;
  final int staffId;
  final String? staffName;
  final int serviceId;
  final String? serviceName;
  final DateTime scheduledAt;
  final int durationMinutes;
  final double price;
  final String stateName;
  final String paymentMethod;
  final String paymentStatus;
  final bool isPaid;
  final String? cancellationReason;
  final bool hasReview;

  AppointmentModel({
    required this.id,
    required this.customerId,
    this.customerName,
    required this.salonId,
    this.salonName,
    required this.staffId,
    this.staffName,
    required this.serviceId,
    this.serviceName,
    required this.scheduledAt,
    required this.durationMinutes,
    required this.price,
    required this.stateName,
    required this.paymentMethod,
    required this.paymentStatus,
    this.isPaid = false,
    this.cancellationReason,
    this.hasReview = false,
  });

  factory AppointmentModel.fromJson(Map<String, dynamic> json) => AppointmentModel(
        id: json['id'],
        customerId: json['customerId'],
        customerName: json['customerName'],
        salonId: json['salonId'],
        salonName: json['salonName'],
        staffId: json['staffId'],
        staffName: json['staffName'],
        serviceId: json['serviceId'],
        serviceName: json['serviceName'],
        scheduledAt: DateTime.parse(json['scheduledAt']),
        durationMinutes: json['durationMinutes'] ?? 30,
        price: (json['price'] ?? 0).toDouble(),
        stateName: json['stateName'] ?? 'Pending',
        paymentMethod: json['paymentMethod'] ?? 'Cash',
        paymentStatus: json['paymentStatus'] ?? 'Unpaid',
        isPaid: json['isPaid'] ?? false,
        cancellationReason: json['cancellationReason'],
        hasReview: json['hasReview'] ?? false,
      );
}

class ReviewModel {
  final int id;
  final int appointmentId;
  final int customerId;
  final String? customerName;
  final int salonId;
  final int rating;
  final String? comment;
  final String? salonReply;
  final bool isRemoved;

  ReviewModel({
    required this.id,
    required this.appointmentId,
    required this.customerId,
    this.customerName,
    required this.salonId,
    required this.rating,
    this.comment,
    this.salonReply,
    this.isRemoved = false,
  });

  factory ReviewModel.fromJson(Map<String, dynamic> json) => ReviewModel(
        id: json['id'],
        appointmentId: json['appointmentId'],
        customerId: json['customerId'],
        customerName: json['customerName'],
        salonId: json['salonId'],
        rating: json['rating'] ?? 0,
        comment: json['comment'],
        salonReply: json['salonReply'],
        isRemoved: json['isRemoved'] ?? false,
      );
}

class NotificationModel {
  final int id;
  final int userId;
  final String title;
  final String body;
  final String type;
  final bool isRead;
  final DateTime sentAt;

  NotificationModel({
    required this.id,
    required this.userId,
    required this.title,
    required this.body,
    required this.type,
    this.isRead = false,
    required this.sentAt,
  });

  factory NotificationModel.fromJson(Map<String, dynamic> json) => NotificationModel(
        id: json['id'],
        userId: json['userId'],
        title: json['title'] ?? '',
        body: json['body'] ?? '',
        type: json['type'] ?? '',
        isRead: json['isRead'] ?? false,
        sentAt: DateTime.parse(json['sentAt']),
      );
}

class FavoriteModel {
  final int userId;
  final int salonId;
  final String? salonName;

  FavoriteModel({required this.userId, required this.salonId, this.salonName});

  factory FavoriteModel.fromJson(Map<String, dynamic> json) => FavoriteModel(
        userId: json['userId'],
        salonId: json['salonId'],
        salonName: json['salonName'],
      );
}

class RecommendationModel {
  final SalonModel salon;
  final double score;
  final String reason;

  RecommendationModel({required this.salon, required this.score, required this.reason});

  factory RecommendationModel.fromJson(Map<String, dynamic> json) => RecommendationModel(
        salon: SalonModel.fromJson(json['salon']),
        score: (json['score'] ?? 0).toDouble(),
        reason: json['reason'] ?? '',
      );
}
