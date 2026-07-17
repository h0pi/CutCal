using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CutCal.Services.Database;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => new { x.UserId, x.RoleId }).IsUnique();
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.Token).IsUnique();
    }
}

public class SalonConfiguration : IEntityTypeConfiguration<Salon>
{
    public void Configure(EntityTypeBuilder<Salon> builder)
    {
        builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.SalonCategory).WithMany(x => x.Salons).HasForeignKey(x => x.SalonCategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.City).WithMany(x => x.Salons).HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.AvgRating).HasColumnType("decimal(3,2)");
    }
}

public class SalonWorkingHoursConfiguration : IEntityTypeConfiguration<SalonWorkingHours>
{
    public void Configure(EntityTypeBuilder<SalonWorkingHours> builder)
    {
        builder.HasOne(x => x.Salon).WithMany(x => x.WorkingHours).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SalonGalleryConfiguration : IEntityTypeConfiguration<SalonGallery>
{
    public void Configure(EntityTypeBuilder<SalonGallery> builder)
    {
        builder.HasOne(x => x.Salon).WithMany(x => x.Gallery).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class SalonServiceConfiguration : IEntityTypeConfiguration<SalonService>
{
    public void Configure(EntityTypeBuilder<SalonService> builder)
    {
        builder.HasOne(x => x.Salon).WithMany(x => x.Services).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}

public class StaffConfiguration : IEntityTypeConfiguration<Staff>
{
    public void Configure(EntityTypeBuilder<Staff> builder)
    {
        builder.HasOne(x => x.Salon).WithMany(x => x.Staff).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class StaffServiceConfiguration : IEntityTypeConfiguration<StaffService>
{
    public void Configure(EntityTypeBuilder<StaffService> builder)
    {
        builder.HasKey(x => new { x.StaffId, x.ServiceId });
        builder.HasOne(x => x.Staff).WithMany(x => x.StaffServices).HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Cascade);
        // Restrict here (not Cascade): Salon already cascades to both Staff and SalonService,
        // so a second cascade path into StaffServices via ServiceId would create a cycle SQL Server rejects.
        builder.HasOne(x => x.Service).WithMany(x => x.StaffServices).HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Salon).WithMany(x => x.Appointments).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Staff).WithMany(x => x.Appointments).HasForeignKey(x => x.StaffId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Service).WithMany(x => x.Appointments).HasForeignKey(x => x.ServiceId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.ApprovedBy).WithMany().HasForeignKey(x => x.ApprovedById).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasOne(x => x.Appointment).WithOne(x => x.Review).HasForeignKey<Review>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.AppointmentId).IsUnique();
        builder.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Salon).WithMany(x => x.Reviews).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.RemovedBy).WithMany().HasForeignKey(x => x.RemovedById).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(x => x.Appointment).WithOne(x => x.Payment).HasForeignKey<Payment>(x => x.AppointmentId).OnDelete(DeleteBehavior.Cascade);
        builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.HasKey(x => new { x.UserId, x.SalonId });
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Salon).WithMany(x => x.Favorites).HasForeignKey(x => x.SalonId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserSearchHistoryConfiguration : IEntityTypeConfiguration<UserSearchHistory>
{
    public void Configure(EntityTypeBuilder<UserSearchHistory> builder)
    {
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.SalonCategory).WithMany().HasForeignKey(x => x.SalonCategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}
