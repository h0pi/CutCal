using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Database;

public class CutCalDbContext : DbContext
{
    public CutCalDbContext(DbContextOptions<CutCalDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Salon> Salons => Set<Salon>();
    public DbSet<SalonWorkingHours> SalonWorkingHours => Set<SalonWorkingHours>();
    public DbSet<SalonGallery> SalonGalleries => Set<SalonGallery>();
    public DbSet<SalonCategory> SalonCategories => Set<SalonCategory>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<SalonService> SalonServices => Set<SalonService>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<StaffService> StaffServices => Set<StaffService>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<UserSearchHistory> UserSearchHistories => Set<UserSearchHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CutCalDbContext).Assembly);

        CutCalSeed.Seed(modelBuilder);
    }
}
