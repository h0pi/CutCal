using CutCal.Model.Responses;
using CutCal.Services.Database;
using Mapster;

namespace CutCal.WebAPI.Mapping;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<User, UserResponse>.NewConfig()
            .Map(dest => dest.Roles, src => src.UserRoles.Select(ur => ur.Role.Name).ToList());

        TypeAdapterConfig<Staff, StaffResponse>.NewConfig()
            .Map(dest => dest.FullName, src => src.User != null ? src.User.FirstName + " " + src.User.LastName : null)
            .Map(dest => dest.ServiceIds, src => src.StaffServices.Select(ss => ss.ServiceId).ToList());

        TypeAdapterConfig<Salon, SalonResponse>.NewConfig()
            .Map(dest => dest.SalonCategoryName, src => src.SalonCategory != null ? src.SalonCategory.Name : null)
            .Map(dest => dest.CityName, src => src.City != null ? src.City.Name : null);

        TypeAdapterConfig<Appointment, AppointmentResponse>.NewConfig()
            .Map(dest => dest.CustomerName, src => src.Customer != null ? src.Customer.FirstName + " " + src.Customer.LastName : null)
            .Map(dest => dest.SalonName, src => src.Salon != null ? src.Salon.Name : null)
            .Map(dest => dest.StaffName, src => src.Staff != null && src.Staff.User != null ? src.Staff.User.FirstName + " " + src.Staff.User.LastName : null)
            .Map(dest => dest.ServiceName, src => src.Service != null ? src.Service.Name : null)
            .Map(dest => dest.IsPaid, src => src.PaymentStatus == "Paid")
            .Map(dest => dest.HasReview, src => src.Review != null);

        TypeAdapterConfig<Review, ReviewResponse>.NewConfig()
            .Map(dest => dest.CustomerName, src => src.Customer != null ? src.Customer.FirstName + " " + src.Customer.LastName : null);

        TypeAdapterConfig<Favorite, FavoriteResponse>.NewConfig()
            .Map(dest => dest.SalonName, src => src.Salon != null ? src.Salon.Name : null);
    }
}
