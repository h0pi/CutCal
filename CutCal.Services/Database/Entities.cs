namespace CutCal.Services.Database;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<UserRole> UserRoles { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<UserRole> UserRoles { get; set; } = new();
}

public class UserRole
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SalonCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<Salon> Salons { get; set; } = new();
}

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;

    public List<Salon> Salons { get; set; } = new();
}

public class Salon
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int SalonCategoryId { get; set; }
    public SalonCategory SalonCategory { get; set; } = null!;
    public string? Description { get; set; }
    public string Address { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public double AvgRating { get; set; }
    public bool IsApproved { get; set; }
    public bool AutoConfirm { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<SalonWorkingHours> WorkingHours { get; set; } = new();
    public List<SalonGallery> Gallery { get; set; } = new();
    public List<SalonService> Services { get; set; } = new();
    public List<Staff> Staff { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
}

public class SalonWorkingHours
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public int DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}

public class SalonGallery
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

public class SalonService
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    public List<StaffService> StaffServices { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}

public class Staff
{
    public int Id { get; set; }
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public List<StaffService> StaffServices { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}

public class StaffService
{
    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;
    public int ServiceId { get; set; }
    public SalonService Service { get; set; } = null!;
}

public class Appointment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public int StaffId { get; set; }
    public Staff Staff { get; set; } = null!;
    public int ServiceId { get; set; }
    public SalonService Service { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string StateName { get; set; } = null!;
    public string PaymentMethod { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string? PaypalOrderId { get; set; }
    public string? PaypalCaptureId { get; set; }
    public string? CancellationReason { get; set; }
    public int? ApprovedById { get; set; }
    public User? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Review? Review { get; set; }
    public Payment? Payment { get; set; }
}

public class Review
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public int CustomerId { get; set; }
    public User Customer { get; set; } = null!;
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? SalonReply { get; set; }
    public bool IsRemoved { get; set; }
    public int? RemovedById { get; set; }
    public User? RemovedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Payment
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public Appointment Appointment { get; set; } = null!;
    public string? PaypalOrderId { get; set; }
    public string? PaypalCaptureId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string Type { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}

public class Favorite
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SalonId { get; set; }
    public Salon Salon { get; set; } = null!;
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}

public class UserSearchHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int SalonCategoryId { get; set; }
    public SalonCategory SalonCategory { get; set; } = null!;
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
}
