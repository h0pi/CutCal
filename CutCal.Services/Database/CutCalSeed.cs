using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Database;

public static class CutCalSeed
{
    // Precomputed BCrypt hash of "test123". Must stay a fixed literal (not a live
    // CryptoService.HashPassword call): BCrypt embeds a fresh random salt on every
    // call, so re-hashing here would produce a different string on every build and
    // EF would perpetually think the seed data changed (PendingModelChangesWarning).
    private const string SeedPasswordHash = "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.";

    public static void Seed(ModelBuilder modelBuilder)
    {
        var passwordHash = SeedPasswordHash;
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Customer" },
            new Role { Id = 2, Name = "Staff" },
            new Role { Id = 3, Name = "SalonManager" },
            new Role { Id = 4, Name = "Admin" }
        );

        modelBuilder.Entity<SalonCategory>().HasData(
            new SalonCategory { Id = 1, Name = "Hair Salon" },
            new SalonCategory { Id = 2, Name = "Barbershop" },
            new SalonCategory { Id = 3, Name = "Beauty Studio" },
            new SalonCategory { Id = 4, Name = "Nail Studio" },
            new SalonCategory { Id = 5, Name = "Spa" }
        );

        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Sarajevo", Country = "BiH" },
            new City { Id = 2, Name = "Mostar", Country = "BiH" },
            new City { Id = 3, Name = "Banja Luka", Country = "BiH" },
            new City { Id = 4, Name = "Zagreb", Country = "HR" },
            new City { Id = 5, Name = "Beograd", Country = "RS" }
        );

        var users = new List<User>
        {
            new() { Id = 1, Username = "admin", FirstName = "Ana", LastName = "Adminovic", Email = "admin@cutcal.com", Phone = "+38761000001", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=1" },
            new() { Id = 2, Username = "admin2", FirstName = "Amar", LastName = "Adminovic", Email = "admin2@cutcal.com", Phone = "+38761000002", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=2" },
            new() { Id = 3, Username = "manager", FirstName = "Maja", LastName = "Menadzer", Email = "manager@cutcal.com", Phone = "+38761000003", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=3" },
            new() { Id = 4, Username = "manager2", FirstName = "Mirza", LastName = "Menadzer", Email = "manager2@cutcal.com", Phone = "+38761000004", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=4" },
            new() { Id = 5, Username = "manager3", FirstName = "Merima", LastName = "Menadzer", Email = "manager3@cutcal.com", Phone = "+38761000005", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=5" },
            new() { Id = 6, Username = "customer", FirstName = "Emina", LastName = "Kupac", Email = "customer@cutcal.com", Phone = "+38761000006", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=6" },
            new() { Id = 7, Username = "customer2", FirstName = "Faruk", LastName = "Kupac", Email = "customer2@cutcal.com", Phone = "+38761000007", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=7" },
            new() { Id = 8, Username = "customer3", FirstName = "Lamija", LastName = "Kupac", Email = "customer3@cutcal.com", Phone = "+38761000008", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=8" },
            new() { Id = 9, Username = "customer4", FirstName = "Haris", LastName = "Kupac", Email = "customer4@cutcal.com", Phone = "+38761000009", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=9" },
            new() { Id = 10, Username = "customer5", FirstName = "Amina", LastName = "Kupac", Email = "customer5@cutcal.com", Phone = "+38761000010", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "https://i.pravatar.cc/150?img=10" },
        };
        var staffFirstNames = new[] { "Selma", "Tarik", "Ajla", "Kenan", "Dino", "Nejra", "Ismar", "Belma" };
        var staffLastNames = new[] { "Hodzic", "Begic", "Suljic", "Delic", "Karic", "Osmic", "Kovac", "Halilovic" };
        for (var i = 0; i < 8; i++)
        {
            users.Add(new User
            {
                Id = 11 + i,
                Username = $"staff{i + 1}",
                FirstName = staffFirstNames[i],
                LastName = staffLastNames[i],
                Email = $"staff{i + 1}@cutcal.com",
                Phone = $"+387611000{11 + i}",
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = seedDate,
                ProfileImageUrl = $"https://i.pravatar.cc/150?img={11 + i}"
            });
        }
        modelBuilder.Entity<User>().HasData(users);

        var userRoles = new List<UserRole>
        {
            new() { Id = 1, UserId = 1, RoleId = 4 },
            new() { Id = 2, UserId = 2, RoleId = 4 },
            new() { Id = 3, UserId = 3, RoleId = 3 },
            new() { Id = 4, UserId = 4, RoleId = 3 },
            new() { Id = 5, UserId = 5, RoleId = 3 },
            new() { Id = 6, UserId = 6, RoleId = 1 },
            new() { Id = 7, UserId = 7, RoleId = 1 },
            new() { Id = 8, UserId = 8, RoleId = 1 },
            new() { Id = 9, UserId = 9, RoleId = 1 },
            new() { Id = 10, UserId = 10, RoleId = 1 },
        };
        for (var i = 0; i < 8; i++)
        {
            userRoles.Add(new UserRole { Id = 11 + i, UserId = 11 + i, RoleId = 2 });
        }
        modelBuilder.Entity<UserRole>().HasData(userRoles);

        var salonImageUrls = new[]
        {
            "https://images.unsplash.com/photo-1560066984-138dadb4c035",
            "https://images.unsplash.com/photo-1521590832167-7bcbfaa6381f",
            "https://images.unsplash.com/photo-1522337660859-02fbefca4702",
            "https://images.unsplash.com/photo-1516975080664-ed2fc6a32937",
            "https://images.unsplash.com/photo-1585747860715-2ba37e788b70",
        };
        var salonNames = new[] { "Bellissima Hair Studio", "Gentleman's Cut Barbershop", "Glow Beauty Studio", "Perfect Nails Studio", "Relax & Spa" };
        var salonOwners = new[] { 3, 4, 5, 3, 4 };
        var salonCities = new[] { 1, 2, 3, 4, 5 };
        var salonCategoriesArr = new[] { 1, 2, 3, 4, 5 };
        var salonLatLng = new (double lat, double lng)[]
        {
            (43.8563, 18.4131),
            (43.3438, 17.8078),
            (44.7722, 17.1910),
            (45.8150, 15.9819),
            (44.7866, 20.4489),
        };
        var salons = new List<Salon>();
        for (var i = 0; i < 5; i++)
        {
            salons.Add(new Salon
            {
                Id = i + 1,
                OwnerId = salonOwners[i],
                Name = salonNames[i],
                SalonCategoryId = salonCategoriesArr[i],
                Description = $"{salonNames[i]} - professional service since 2015.",
                Address = $"Main street {i + 1}",
                CityId = salonCities[i],
                Latitude = salonLatLng[i].lat,
                Longitude = salonLatLng[i].lng,
                Phone = $"+38762100{i:00}",
                Email = $"contact@salon{i + 1}.cutcal.com",
                ProfileImageUrl = salonImageUrls[i],
                AvgRating = 4.2 + i * 0.1,
                IsApproved = true,
                AutoConfirm = i % 2 == 0,
                CreatedAt = seedDate
            });
        }
        modelBuilder.Entity<Salon>().HasData(salons);

        var workingHours = new List<SalonWorkingHours>();
        var whId = 1;
        for (var s = 1; s <= 5; s++)
        {
            for (var day = 0; day <= 6; day++)
            {
                var isClosed = day == 6;
                workingHours.Add(new SalonWorkingHours
                {
                    Id = whId++,
                    SalonId = s,
                    DayOfWeek = day,
                    OpenTime = isClosed ? null : new TimeOnly(9, 0),
                    CloseTime = isClosed ? null : new TimeOnly(20, 0),
                    IsClosed = isClosed
                });
            }
        }
        modelBuilder.Entity<SalonWorkingHours>().HasData(workingHours);

        var galleryImages = new[]
        {
            "https://images.unsplash.com/photo-1562322140-8baeececf3df",
            "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38",
        };
        var gallery = new List<SalonGallery>();
        var galId = 1;
        for (var s = 1; s <= 5; s++)
        {
            for (var g = 0; g < 2; g++)
            {
                gallery.Add(new SalonGallery { Id = galId++, SalonId = s, ImageUrl = galleryImages[g], Caption = "Interior", UploadedAt = seedDate });
            }
        }
        modelBuilder.Entity<SalonGallery>().HasData(gallery);

        var serviceNamesBySalon = new Dictionary<int, string[]>
        {
            { 1, new[] { "Haircut", "Hair Coloring", "Blow Dry" } },
            { 2, new[] { "Beard Trim", "Classic Shave", "Kids Haircut" } },
            { 3, new[] { "Facial Treatment", "Makeup", "Eyebrow Shaping" } },
            { 4, new[] { "Manicure", "Pedicure", "Gel Nails" } },
            { 5, new[] { "Massage", "Sauna Session", "Body Scrub" } },
        };
        var salonServiceMap = new Dictionary<int, int[]>();
        var services = new List<SalonService>();
        var svcId = 1;
        for (var s = 1; s <= 5; s++)
        {
            var ids = new List<int>();
            foreach (var name in serviceNamesBySalon[s])
            {
                services.Add(new SalonService
                {
                    Id = svcId,
                    SalonId = s,
                    Name = name,
                    Description = $"{name} performed by our professionals.",
                    DurationMinutes = 30 + (svcId % 3) * 15,
                    Price = 20m + svcId * 5,
                    IsActive = true
                });
                ids.Add(svcId);
                svcId++;
            }
            salonServiceMap[s] = ids.ToArray();
        }
        modelBuilder.Entity<SalonService>().HasData(services);

        var salonStaffMap = new Dictionary<int, int[]>
        {
            { 1, new[] { 1, 2 } },
            { 2, new[] { 3, 4 } },
            { 3, new[] { 5, 6 } },
            { 4, new[] { 7 } },
            { 5, new[] { 8 } },
        };
        var staffList = new List<Staff>();
        var staffService = new List<StaffService>();
        var staffUserId = 11;
        foreach (var (salonId, staffIds) in salonStaffMap)
        {
            foreach (var staffId in staffIds)
            {
                staffList.Add(new Staff
                {
                    Id = staffId,
                    SalonId = salonId,
                    UserId = staffUserId,
                    Role = "Stylist",
                    Bio = "Experienced professional.",
                    ProfileImageUrl = $"https://i.pravatar.cc/150?img={10 + staffId}",
                    IsActive = true
                });
                foreach (var serviceId in salonServiceMap[salonId])
                {
                    staffService.Add(new StaffService { StaffId = staffId, ServiceId = serviceId });
                }
                staffUserId++;
            }
        }
        modelBuilder.Entity<Staff>().HasData(staffList);
        modelBuilder.Entity<StaffService>().HasData(staffService);

        // Appointments: 20 total -> 4 Pending, 3 Confirmed, 10 Completed, 3 Cancelled
        var appointments = new List<Appointment>();
        var completedAppointmentIds = new List<int>();
        var customerIds = new[] { 6, 7, 8, 9, 10 };
        for (var i = 0; i < 20; i++)
        {
            var salonId = (i % 5) + 1;
            var staffIds = salonStaffMap[salonId];
            var staffId = staffIds[i % staffIds.Length];
            var serviceId = salonServiceMap[salonId][i % salonServiceMap[salonId].Length];
            var customerId = customerIds[i % customerIds.Length];
            var scheduledAt = seedDate.AddDays(i - 10).AddHours(10 + i % 6);
            var paymentMethod = i % 2 == 0 ? "Cash" : "PayPal";

            string state;
            string paymentStatus;
            int? approvedById = null;
            DateTime? approvedAt = null;
            string? cancellationReason = null;

            if (i < 4)
            {
                state = "Pending";
                paymentStatus = "Unpaid";
            }
            else if (i < 7)
            {
                state = "Confirmed";
                paymentStatus = "Unpaid";
                approvedById = salonOwners[salonId - 1];
                approvedAt = scheduledAt.AddDays(-1);
            }
            else if (i < 17)
            {
                state = "Completed";
                paymentStatus = "Paid";
                approvedById = salonOwners[salonId - 1];
                approvedAt = scheduledAt.AddDays(-1);
            }
            else
            {
                state = "Cancelled";
                paymentStatus = "Unpaid";
                cancellationReason = "Customer requested cancellation.";
            }

            var appointmentId = i + 1;
            appointments.Add(new Appointment
            {
                Id = appointmentId,
                CustomerId = customerId,
                SalonId = salonId,
                StaffId = staffId,
                ServiceId = serviceId,
                ScheduledAt = scheduledAt,
                DurationMinutes = 45,
                Price = 20m + serviceId * 5,
                StateName = state,
                PaymentMethod = paymentMethod,
                PaymentStatus = paymentStatus,
                ApprovedById = approvedById,
                ApprovedAt = approvedAt,
                CancellationReason = cancellationReason,
                CreatedAt = scheduledAt.AddDays(-2)
            });

            if (state == "Completed")
            {
                completedAppointmentIds.Add(appointmentId);
            }
        }
        modelBuilder.Entity<Appointment>().HasData(appointments);

        var comments = new[]
        {
            "Great service, highly recommend!",
            "Very professional staff.",
            "Loved the atmosphere and result.",
            "Will definitely come back again.",
            "Friendly staff and clean salon.",
            "Good value for money.",
            "Exceeded my expectations.",
            "Quick and efficient service.",
            "Amazing experience overall.",
            "Solid work, minor wait time."
        };
        var reviews = new List<Review>();
        for (var i = 0; i < completedAppointmentIds.Count; i++)
        {
            var appointmentId = completedAppointmentIds[i];
            var appointment = appointments.First(a => a.Id == appointmentId);
            reviews.Add(new Review
            {
                Id = i + 1,
                AppointmentId = appointmentId,
                CustomerId = appointment.CustomerId,
                SalonId = appointment.SalonId,
                Rating = 3 + i % 3,
                Comment = comments[i],
                IsRemoved = false,
                CreatedAt = appointment.ScheduledAt.AddHours(2)
            });
        }
        modelBuilder.Entity<Review>().HasData(reviews);
    }
}
