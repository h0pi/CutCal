namespace CutCal.Model.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public UserResponse User { get; set; } = null!;
}

public class SalonWorkingHoursResponse
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}

public class SalonGalleryResponse
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class SalonResponse
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public int SalonCategoryId { get; set; }
    public string? SalonCategoryName { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; } = null!;
    public int CityId { get; set; }
    public string? CityName { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public double AvgRating { get; set; }
    public bool IsApproved { get; set; }
    public bool AutoConfirm { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SalonWorkingHoursResponse> WorkingHours { get; set; } = new();
    public double? DistanceKm { get; set; }
}

public class SalonCategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class CityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
}

public class SalonServiceResponse
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class StaffResponse
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public int UserId { get; set; }
    public string? FullName { get; set; }
    public string Role { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<int> ServiceIds { get; set; } = new();
}

public class AppointmentResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int SalonId { get; set; }
    public string? SalonName { get; set; }
    public int StaffId { get; set; }
    public string? StaffName { get; set; }
    public int ServiceId { get; set; }
    public string? ServiceName { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string StateName { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public bool IsPaid { get; set; }
    public string? PaypalOrderId { get; set; }
    public string? PaypalCaptureId { get; set; }
    public string? CancellationReason { get; set; }
    public int? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasReview { get; set; }
}

public class ReviewResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int SalonId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? SalonReply { get; set; }
    public bool IsRemoved { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string? PaypalOrderId { get; set; }
    public string? PaypalCaptureId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class CreateOrderResponse
{
    public string OrderId { get; set; } = null!;
    public string ApprovalUrl { get; set; } = null!;
}

public class FavoriteResponse
{
    public int UserId { get; set; }
    public int SalonId { get; set; }
    public string? SalonName { get; set; }
    public DateTime SavedAt { get; set; }
}

public class RecommendationResponse
{
    public SalonResponse Salon { get; set; } = null!;
    public double Score { get; set; }
    public string Reason { get; set; } = null!;
}

public class AppointmentsReportResponse
{
    public int TotalAppointments { get; set; }
    public int ConfirmedCount { get; set; }
    public int CancelledCount { get; set; }
    public int CompletedCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<AppointmentResponse> Appointments { get; set; } = new();
}

public class ServiceReportItem
{
    public string ServiceName { get; set; } = null!;
    public int AppointmentCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
}

public class ServicesReportResponse
{
    public string? MostPopularService { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<ServiceReportItem> Services { get; set; } = new();
}
