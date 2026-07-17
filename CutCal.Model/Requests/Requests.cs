namespace CutCal.Model.Requests;

public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginWithRefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class UserInsertRequest
{
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Password { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class UserUpdateRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ChangePasswordRequest
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public string ConfirmNewPassword { get; set; } = null!;
}

public class SalonWorkingHoursUpsertRequest
{
    public int DayOfWeek { get; set; }
    public TimeOnly? OpenTime { get; set; }
    public TimeOnly? CloseTime { get; set; }
    public bool IsClosed { get; set; }
}

public class SalonInsertRequest
{
    public string Name { get; set; } = null!;
    public int SalonCategoryId { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; } = null!;
    public int CityId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool AutoConfirm { get; set; }
    public List<SalonWorkingHoursUpsertRequest> WorkingHours { get; set; } = new();
}

public class SalonUpdateRequest
{
    public string Name { get; set; } = null!;
    public int SalonCategoryId { get; set; }
    public string? Description { get; set; }
    public string Address { get; set; } = null!;
    public int CityId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool AutoConfirm { get; set; }
    public List<SalonWorkingHoursUpsertRequest> WorkingHours { get; set; } = new();
}

public class SalonGalleryInsertRequest
{
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
}

public class SalonCategoryInsertRequest
{
    public string Name { get; set; } = null!;
}

public class SalonCategoryUpdateRequest
{
    public string Name { get; set; } = null!;
}

public class CityInsertRequest
{
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
}

public class CityUpdateRequest
{
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
}

public class SalonServiceInsertRequest
{
    public int SalonId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SalonServiceUpdateRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}

public class StaffInsertRequest
{
    public int SalonId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<int> ServiceIds { get; set; } = new();
}

public class StaffUpdateRequest
{
    public string Role { get; set; } = null!;
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; }
    public List<int> ServiceIds { get; set; } = new();
}

public class AppointmentInsertRequest
{
    public int SalonId { get; set; }
    public int StaffId { get; set; }
    public int ServiceId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string PaymentMethod { get; set; } = null!;
}

public class AppointmentCancelRequest
{
    public string Reason { get; set; } = null!;
}

public class ReviewInsertRequest
{
    public int AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewReplyRequest
{
    public string Reply { get; set; } = null!;
}

public class CreateOrderRequest
{
    public int AppointmentId { get; set; }
}

public class CaptureOrderRequest
{
    public string PaypalOrderId { get; set; } = null!;
    public int AppointmentId { get; set; }
}
