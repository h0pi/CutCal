using CutCal.Model.Common;

namespace CutCal.Model.SearchObjects;

public class UserSearchObject : BaseSearchObject
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public bool? IsActive { get; set; }
}

public class SalonSearchObject : BaseSearchObject
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? CityId { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public double? RadiusKm { get; set; }
    public bool? IsApproved { get; set; }
}

public class SalonServiceSearchObject : BaseSearchObject
{
    public int? SalonId { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

public class StaffSearchObject : BaseSearchObject
{
    public int? SalonId { get; set; }
    public bool? IsActive { get; set; }
    public string? Name { get; set; }
}

public class AppointmentSearchObject : BaseSearchObject
{
    public int? CustomerId { get; set; }
    public int? SalonId { get; set; }
    public int? StaffId { get; set; }
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class ReviewSearchObject : BaseSearchObject
{
    public int? SalonId { get; set; }
    public int? CustomerId { get; set; }
    public bool? IsRemoved { get; set; }
}

public class NotificationSearchObject : BaseSearchObject
{
    public bool? IsRead { get; set; }
}

public class SalonCategorySearchObject : BaseSearchObject
{
    public string? Name { get; set; }
}

public class CitySearchObject : BaseSearchObject
{
    public string? Name { get; set; }
}
