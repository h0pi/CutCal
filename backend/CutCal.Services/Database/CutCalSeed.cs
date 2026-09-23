using CutCal.Model.Constants;
using Microsoft.EntityFrameworkCore;

namespace CutCal.Services.Database;

public static class CutCalSeed
{
    // Precomputed BCrypt hash of "test" (the password every seeded account uses). Must stay a fixed literal (not a live
    // CryptoService.HashPassword call): BCrypt embeds a fresh random salt on every
    // call, so re-hashing here would produce a different string on every build and
    // EF would perpetually think the seed data changed (PendingModelChangesWarning).
    private const string SeedPasswordHash = "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi";

    public static void Seed(ModelBuilder modelBuilder)
    {
        var passwordHash = SeedPasswordHash;
        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = RoleNames.Customer },
            new Role { Id = 2, Name = RoleNames.Staff },
            new Role { Id = 3, Name = RoleNames.SalonManager },
            new Role { Id = 4, Name = RoleNames.Admin }
        );

        modelBuilder.Entity<SalonCategory>().HasData(
            new SalonCategory { Id = 1, Name = "Hair Salon" },
            new SalonCategory { Id = 2, Name = "Barbershop" },
            new SalonCategory { Id = 3, Name = "Beauty Studio" },
            new SalonCategory { Id = 4, Name = "Nail Studio" },
            new SalonCategory { Id = 5, Name = "Spa" }
        );

        modelBuilder.Entity<Country>().HasData(
            new Country { Id = 1, Name = "BiH" },
            new Country { Id = 2, Name = "HR" },
            new Country { Id = 3, Name = "RS" }
        );

        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Sarajevo", CountryId = 1 },
            new City { Id = 2, Name = "Mostar", CountryId = 1 },
            new City { Id = 3, Name = "Banja Luka", CountryId = 1 },
            new City { Id = 4, Name = "Zagreb", CountryId = 2 },
            new City { Id = 5, Name = "Beograd", CountryId = 3 }
        );

        var users = new List<User>
        {
            new() { Id = 1, Username = "admin", FirstName = "Ana", LastName = "Adminovic", Email = "admin@cutcal.com", Phone = "+38761000001", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/1.jpg" },
            new() { Id = 2, Username = "admin2", FirstName = "Amar", LastName = "Adminovic", Email = "admin2@cutcal.com", Phone = "+38761000002", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/2.jpg" },
            new() { Id = 3, Username = "manager", FirstName = "Maja", LastName = "Menadzer", Email = "manager@cutcal.com", Phone = "+38761000003", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/3.jpg" },
            new() { Id = 4, Username = "manager2", FirstName = "Mirza", LastName = "Menadzer", Email = "manager2@cutcal.com", Phone = "+38761000004", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/4.jpg" },
            new() { Id = 5, Username = "manager3", FirstName = "Merima", LastName = "Menadzer", Email = "manager3@cutcal.com", Phone = "+38761000005", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/5.jpg" },
            new() { Id = 6, Username = "customer", FirstName = "Emina", LastName = "Kupac", Email = "customer@cutcal.com", Phone = "+38761000006", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/6.jpg" },
            new() { Id = 7, Username = "customer2", FirstName = "Faruk", LastName = "Kupac", Email = "customer2@cutcal.com", Phone = "+38761000007", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/7.jpg" },
            new() { Id = 8, Username = "customer3", FirstName = "Lamija", LastName = "Kupac", Email = "customer3@cutcal.com", Phone = "+38761000008", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/8.jpg" },
            new() { Id = 9, Username = "customer4", FirstName = "Haris", LastName = "Kupac", Email = "customer4@cutcal.com", Phone = "+38761000009", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/9.jpg" },
            new() { Id = 10, Username = "customer5", FirstName = "Amina", LastName = "Kupac", Email = "customer5@cutcal.com", Phone = "+38761000010", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/10.jpg" },
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
                ProfileImageUrl = $"/images/avatars/{11 + i}.jpg"
            });
        }
        users.Add(new User { Id = 19, Username = "desktop", FirstName = "Desktop", LastName = "Admin", Email = "desktop@cutcal.com", Phone = "+38761000019", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/19.jpg" });
        users.Add(new User { Id = 20, Username = "mobile", FirstName = "Mobile", LastName = "Customer", Email = "mobile@cutcal.com", Phone = "+38761000020", PasswordHash = passwordHash, IsActive = true, CreatedAt = seedDate, ProfileImageUrl = "/images/avatars/20.jpg" });
        // Extra demo accounts: staff users 21..42 (staff 9..30) and customers 43..47.
        var extraUserRoles = new List<UserRole>();
        var extraStaffFirstNames = new[] { "Amra", "Emir", "Lejla", "Adnan", "Mia", "Armin", "Sara", "Damir", "Ivana", "Nermin", "Jasmina", "Kemal", "Lana", "Eldin", "Vesna", "Alen", "Zerina", "Mirza", "Aida", "Anel", "Nina", "Elvir" };
        var extraStaffLastNames = new[] { "Mujic", "Cengic", "Basic", "Zukic", "Pavlovic", "Ramic", "Softic", "Ibrahimovic", "Jukic", "Hadzic", "Mesic", "Babic", "Peric", "Kurtovic", "Salihovic", "Tahirovic", "Bajric", "Memic", "Hasanovic", "Dzafic", "Krupic", "Music" };
        for (var k = 0; k < extraStaffFirstNames.Length; k++)
        {
            var userId = FirstExtraStaffUserId + k;
            var staffNumber = FirstExtraStaffId + k;
            users.Add(new User
            {
                Id = userId,
                Username = $"staff{staffNumber}",
                FirstName = extraStaffFirstNames[k],
                LastName = extraStaffLastNames[k],
                Email = $"staff{staffNumber}@cutcal.com",
                Phone = $"+3876110{userId:0000}",
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = seedDate,
                ProfileImageUrl = $"/images/avatars/{userId}.jpg"
            });
            extraUserRoles.Add(new UserRole { Id = userId, UserId = userId, RoleId = 2 });
        }
        var extraCustomerFirstNames = new[] { "Sabina", "Damir", "Jasmin", "Elma", "Almir" };
        for (var k = 0; k < extraCustomerFirstNames.Length; k++)
        {
            var userId = FirstExtraCustomerUserId + k;
            users.Add(new User
            {
                Id = userId,
                Username = $"customer{6 + k}",
                FirstName = extraCustomerFirstNames[k],
                LastName = "Kupac",
                Email = $"customer{6 + k}@cutcal.com",
                Phone = $"+3876110{userId:0000}",
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = seedDate,
                ProfileImageUrl = $"/images/avatars/{userId}.jpg"
            });
            extraUserRoles.Add(new UserRole { Id = userId, UserId = userId, RoleId = 1 });
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
        userRoles.Add(new UserRole { Id = 19, UserId = 19, RoleId = 4 });
        userRoles.Add(new UserRole { Id = 20, UserId = 20, RoleId = 1 });
        userRoles.AddRange(extraUserRoles);
        modelBuilder.Entity<UserRole>().HasData(userRoles);

        var salonSeeds = SalonSeeds;
        var salons = new List<Salon>();
        for (var i = 0; i < salonSeeds.Length; i++)
        {
            var seed = salonSeeds[i];
            var salonId = i + 1;
            salons.Add(new Salon
            {
                Id = salonId,
                OwnerId = seed.OwnerId,
                Name = seed.Name,
                SalonCategoryId = seed.CategoryId,
                Description = seed.Description,
                Address = seed.Address,
                CityId = seed.CityId,
                Latitude = seed.Latitude,
                Longitude = seed.Longitude,
                Phone = $"+3876210{salonId:000}",
                Email = $"contact@{seed.ImageSlug}.cutcal.com",
                ProfileImageUrl = $"/images/salons/{seed.ImageSlug}-cover.jpg",
                IsApproved = true,
                AutoConfirm = salonId % 2 == 1,
                CreatedAt = seedDate
            });
        }

        var workingHours = new List<SalonWorkingHours>();
        foreach (var salon in salons)
        {
            for (var day = 0; day <= 6; day++)
            {
                var (open, close) = HoursFor(salon.SalonCategoryId, day);
                workingHours.Add(new SalonWorkingHours
                {
                    Id = salon.Id <= OriginalSalonCount ? (salon.Id - 1) * 7 + day + 1 : ExtraIdStart + (salon.Id - OriginalSalonCount - 1) * 7 + day + 1,
                    SalonId = salon.Id,
                    DayOfWeek = day,
                    OpenTime = open,
                    CloseTime = close,
                    IsClosed = open is null
                });
            }
        }
        modelBuilder.Entity<SalonWorkingHours>().HasData(workingHours);

        var gallery = new List<SalonGallery>();
        var galId = 1;
        for (var i = 0; i < salonSeeds.Length; i++)
        {
            gallery.Add(new SalonGallery { Id = galId++, SalonId = i + 1, ImageUrl = $"/images/salons/{salonSeeds[i].ImageSlug}-1.jpg", Caption = "Interior", UploadedAt = seedDate });
            gallery.Add(new SalonGallery { Id = galId++, SalonId = i + 1, ImageUrl = $"/images/salons/{salonSeeds[i].ImageSlug}-2.jpg", Caption = "Our space", UploadedAt = seedDate });
        }
        modelBuilder.Entity<SalonGallery>().HasData(gallery);

        var services = new List<SalonService>();
        var salonServices = new Dictionary<int, List<SalonService>>();
        var originalServiceId = 1;
        var extraServiceId = ExtraIdStart + 1;
        foreach (var salon in salons)
        {
            var seed = salonSeeds[salon.Id - 1];
            var list = new List<SalonService>();
            foreach (var name in seed.Services)
            {
                var (minutes, basePrice) = ServiceCatalog[name];
                var service = new SalonService
                {
                    Id = salon.Id <= OriginalSalonCount ? originalServiceId++ : extraServiceId++,
                    SalonId = salon.Id,
                    Name = name,
                    Description = $"{name} performed by our professionals.",
                    DurationMinutes = minutes,
                    Price = Math.Round(basePrice * seed.PriceFactor),
                    IsActive = true
                };
                list.Add(service);
                services.Add(service);
            }
            salonServices[salon.Id] = list;
        }
        modelBuilder.Entity<SalonService>().HasData(services);

        // Staff ids 1..8 keep their original users (10 + id); ids 9..30 use the extra staff accounts.
        var salonStaffIds = new Dictionary<int, int[]>
        {
            { 1, new[] { 1, 2 } }, { 2, new[] { 3, 4 } }, { 3, new[] { 5, 6 } }, { 4, new[] { 7, 9 } }, { 5, new[] { 8, 10 } },
        };
        for (var salonId = OriginalSalonCount + 1; salonId <= salons.Count; salonId++)
        {
            var first = FirstExtraStaffId + 2 + (salonId - OriginalSalonCount - 1) * 2;
            salonStaffIds[salonId] = new[] { first, first + 1 };
        }
        var staffList = new List<Staff>();
        var staffService = new List<StaffService>();
        foreach (var (salonId, staffIds) in salonStaffIds)
        {
            var role = StaffRoleByCategory[salonSeeds[salonId - 1].CategoryId];
            foreach (var staffId in staffIds)
            {
                var userId = staffId <= OriginalStaffCount ? 10 + staffId : FirstExtraStaffUserId + (staffId - FirstExtraStaffId);
                staffList.Add(new Staff
                {
                    Id = staffId,
                    SalonId = salonId,
                    UserId = userId,
                    Role = role,
                    Bio = $"{role} with several years of experience.",
                    ProfileImageUrl = $"/images/avatars/{userId}.jpg",
                    IsActive = true
                });
                foreach (var service in salonServices[salonId])
                {
                    staffService.Add(new StaffService { StaffId = staffId, ServiceId = service.Id });
                }
            }
        }
        modelBuilder.Entity<Staff>().HasData(staffList);
        modelBuilder.Entity<StaffService>().HasData(staffService);

        var customerPool = new[] { 6, 7, 8, 9, 10, 20, 43, 44, 45, 46, 47 };
        var appointments = new List<Appointment>();
        Appointment NewAppointment(int id, int salonId, int staffId, SalonService service, int customerId, DateTime scheduledAt, string state, string paymentMethod)
        {
            var completed = state == AppointmentStateNames.Completed;
            var cancelled = state == AppointmentStateNames.Cancelled;
            return new Appointment
            {
                Id = id,
                CustomerId = customerId,
                SalonId = salonId,
                StaffId = staffId,
                ServiceId = service.Id,
                ScheduledAt = scheduledAt,
                DurationMinutes = service.DurationMinutes,
                Price = service.Price,
                StateName = state,
                PaymentMethod = paymentMethod,
                PaymentStatus = completed ? PaymentStatusNames.Paid : PaymentStatusNames.Unpaid,
                ApprovedById = cancelled ? null : salonSeeds[salonId - 1].OwnerId,
                ApprovedAt = cancelled ? null : scheduledAt.AddDays(-1),
                CancellationReason = cancelled ? "Customer requested cancellation." : null,
                CreatedAt = scheduledAt.AddDays(-2)
            };
        }

        // Original 20 appointments (ids 1..20, first five salons): 17 completed, 3 cancelled.
        for (var i = 0; i < 20; i++)
        {
            var salonId = (i % OriginalSalonCount) + 1;
            // Only the original staff take part here, so these rows keep pointing at staff that already exist in older databases.
            var staffIds = salonStaffIds[salonId].Where(id => id <= OriginalStaffCount).ToArray();
            var salonServiceList = salonServices[salonId];
            appointments.Add(NewAppointment(
                i + 1, salonId, staffIds[i % staffIds.Length], salonServiceList[i % salonServiceList.Count],
                customerPool[i % 5], seedDate.AddDays(i - 10).AddHours(10 + i % 6), i < 17 ? AppointmentStateNames.Completed : AppointmentStateNames.Cancelled, i % 2 == 0 ? PaymentMethodNames.Cash : PaymentMethodNames.PayPal));
        }

        // Booking history for every salon (ids 101+): 8 completed visits and 1 cancellation each.
        const int visitsPerSalon = 8;
        var historyId = ExtraIdStart + 1;
        var reviewedHistory = new List<Appointment>();
        foreach (var salon in salons)
        {
            var staffIds = salonStaffIds[salon.Id];
            var salonServiceList = salonServices[salon.Id];
            for (var j = 0; j <= visitsPerSalon; j++)
            {
                var day = seedDate.AddDays(3 + salon.Id * 2 + j * 17);
                while (day.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) day = day.AddDays(1);
                var scheduledAt = day.AddHours(10 + (j + salon.Id) % 7);
                var isCancelled = j == visitsPerSalon;
                var appointment = NewAppointment(
                    historyId++, salon.Id, staffIds[j % staffIds.Length], salonServiceList[j % salonServiceList.Count],
                    customerPool[(salon.Id + j * 3) % customerPool.Length], scheduledAt, isCancelled ? AppointmentStateNames.Cancelled : AppointmentStateNames.Completed, j % 2 == 0 ? PaymentMethodNames.Cash : PaymentMethodNames.PayPal);
                appointments.Add(appointment);
                if (!isCancelled && (j + salon.Id) % 3 != 0) reviewedHistory.Add(appointment);
            }
        }
        modelBuilder.Entity<Appointment>().HasData(appointments);

        var reviews = new List<Review>();
        // Original reviews (ids 1..10) stay attached to appointments 8..17 exactly as before.
        for (var i = 0; i < OriginalReviewComments.Length; i++)
        {
            var appointment = appointments.First(a => a.Id == 8 + i);
            reviews.Add(new Review
            {
                Id = i + 1,
                AppointmentId = appointment.Id,
                CustomerId = appointment.CustomerId,
                SalonId = appointment.SalonId,
                Rating = 3 + i % 3,
                Comment = OriginalReviewComments[i],
                IsRemoved = false,
                CreatedAt = appointment.ScheduledAt.AddHours(2)
            });
        }
        var random = new Random(20260101);
        var reviewId = reviews.Count + 1;
        foreach (var appointment in reviewedHistory)
        {
            var rating = (int)Math.Clamp(Math.Round(TargetRatingBySalon[appointment.SalonId - 1] + (random.NextDouble() * 2 - 1) * 1.1), 1, 5);
            var pool = rating >= 4 ? PositiveComments : rating == 3 ? NeutralComments : NegativeComments;
            reviews.Add(new Review
            {
                Id = reviewId++,
                AppointmentId = appointment.Id,
                CustomerId = appointment.CustomerId,
                SalonId = appointment.SalonId,
                Rating = rating,
                Comment = pool[random.Next(pool.Length)],
                IsRemoved = false,
                CreatedAt = appointment.ScheduledAt.AddHours(2)
            });
        }
        modelBuilder.Entity<Review>().HasData(reviews);

        // Salon ratings are the real average of the seeded reviews, so ratings and reviews never disagree.
        foreach (var salon in salons)
        {
            salon.AvgRating = Math.Round(reviews.Where(r => r.SalonId == salon.Id).Average(r => r.Rating), 2);
        }
        modelBuilder.Entity<Salon>().HasData(salons);
    }

    private const int OriginalSalonCount = 5;
    private const int OriginalStaffCount = 8;
    private const int FirstExtraStaffId = 9;
    private const int FirstExtraStaffUserId = 21;
    private const int FirstExtraCustomerUserId = 43;

    /// <summary>Rows added after the first seed use ids above this value, so they can never collide with rows created at runtime in an existing database.</summary>
    private const int ExtraIdStart = 100;

    private static (TimeOnly? Open, TimeOnly? Close) HoursFor(int categoryId, int day) => categoryId switch
    {
        2 => day == 0 ? (null, null) : (new TimeOnly(8, 0), new TimeOnly(19, 0)),
        5 => day == 0 ? (new TimeOnly(12, 0), new TimeOnly(18, 0)) : (new TimeOnly(10, 0), new TimeOnly(21, 0)),
        4 => day == 0 ? (null, null) : day == 6 ? (new TimeOnly(9, 0), new TimeOnly(16, 0)) : (new TimeOnly(9, 0), new TimeOnly(19, 0)),
        _ => day == 0 ? (null, null) : day == 6 ? (new TimeOnly(9, 0), new TimeOnly(15, 0)) : (new TimeOnly(9, 0), new TimeOnly(20, 0)),
    };

    private static readonly Dictionary<int, string> StaffRoleByCategory = new()
    {
        { 1, "Stylist" }, { 2, "Barber" }, { 3, "Beauty specialist" }, { 4, "Nail technician" }, { 5, "Massage therapist" },
    };

    private static readonly Dictionary<string, (int Minutes, decimal Price)> ServiceCatalog = new()
    {
        { "Haircut", (45, 25) }, { "Hair Coloring", (90, 60) }, { "Blow Dry", (30, 20) }, { "Balayage", (120, 110) },
        { "Keratin Treatment", (90, 80) }, { "Kids Haircut", (30, 15) },
        { "Beard Trim", (20, 12) }, { "Classic Shave", (30, 18) }, { "Haircut & Beard", (60, 32) }, { "Fade Haircut", (45, 22) },
        { "Facial Treatment", (60, 45) }, { "Makeup", (60, 50) }, { "Eyebrow Shaping", (30, 20) }, { "Lash Lift", (60, 55) }, { "Waxing", (30, 25) },
        { "Manicure", (45, 25) }, { "Pedicure", (60, 35) }, { "Gel Nails", (75, 45) }, { "Nail Art", (60, 40) }, { "Acrylic Set", (90, 60) },
        { "Massage", (60, 50) }, { "Hot Stone Massage", (75, 75) }, { "Sauna Session", (45, 25) }, { "Body Scrub", (45, 45) }, { "Relaxing Facial", (60, 50) },
    };

    private sealed record SalonSeed(
        string Name, int CategoryId, int CityId, string Address, double Latitude, double Longitude,
        int OwnerId, string Description, string ImageSlug, decimal PriceFactor, string[] Services);

    // Cities: 1 Sarajevo, 2 Mostar, 3 Banja Luka, 4 Zagreb, 5 Beograd. Categories: 1 Hair, 2 Barbershop, 3 Beauty, 4 Nails, 5 Spa.
    private static readonly SalonSeed[] SalonSeeds =
    {
        new("Bellissima Hair Studio", 1, 1, "Ferhadija 12", 43.8563, 18.4131, 3, "Full-service hair studio in the heart of Sarajevo: cuts, colour and styling since 2015.", "bellissima-hair-studio", 1.00m, new[] { "Haircut", "Hair Coloring", "Blow Dry" }),
        new("Gentleman's Cut Barbershop", 2, 2, "Kralja Tomislava 8", 43.3438, 17.8078, 4, "Classic barbershop with hot-towel shaves and precise fades.", "gentlemans-cut-barbershop", 0.90m, new[] { "Beard Trim", "Classic Shave", "Kids Haircut" }),
        new("Glow Beauty Studio", 3, 3, "Gospodska 21", 44.7722, 17.1910, 5, "Facials, makeup and brow shaping in a calm, bright studio.", "glow-beauty-studio", 1.00m, new[] { "Facial Treatment", "Makeup", "Eyebrow Shaping" }),
        new("Perfect Nails Studio", 4, 4, "Ilica 45", 45.8150, 15.9819, 3, "Manicure, pedicure and gel nails with premium polish.", "perfect-nails-studio", 1.20m, new[] { "Manicure", "Pedicure", "Gel Nails" }),
        new("Relax & Spa", 5, 5, "Knez Mihailova 30", 44.7866, 20.4489, 4, "Massage, sauna and body treatments to unwind after a long week.", "relax-and-spa", 1.10m, new[] { "Massage", "Sauna Session", "Body Scrub" }),
        new("Salon Elegance", 1, 1, "Zmaja od Bosne 33", 43.8514, 18.3922, 5, "Modern hair salon known for balayage and keratin treatments.", "salon-elegance", 1.15m, new[] { "Haircut", "Balayage", "Keratin Treatment" }),
        new("Cut & Color Lab", 1, 4, "Vlaska 60", 45.8131, 15.9860, 3, "Colour specialists in Zagreb: creative colour, precise cuts.", "cut-and-color-lab", 1.25m, new[] { "Haircut", "Hair Coloring", "Kids Haircut" }),
        new("Old Town Barbers", 2, 1, "Bascarsija 5", 43.8598, 18.4313, 4, "Barbershop in the old bazaar: skin fades, beard sculpting and straight-razor shaves.", "old-town-barbers", 1.00m, new[] { "Haircut & Beard", "Fade Haircut", "Beard Trim" }),
        new("Sharp Edge Barbershop", 2, 5, "Skadarska 14", 44.8165, 20.4610, 5, "Belgrade barbershop with a relaxed atmosphere and sharp results.", "sharp-edge-barbershop", 0.95m, new[] { "Classic Shave", "Fade Haircut", "Kids Haircut" }),
        new("Lumiere Beauty Bar", 3, 2, "Brace Fejica 4", 43.3425, 17.8130, 3, "Lash lifts, makeup and facials in central Mostar.", "lumiere-beauty-bar", 0.90m, new[] { "Lash Lift", "Makeup", "Facial Treatment" }),
        new("Blush & Brow Studio", 3, 1, "Marsala Tita 18", 43.8582, 18.4290, 4, "Brow and makeup studio with waxing and quick touch-ups.", "blush-and-brow-studio", 1.05m, new[] { "Eyebrow Shaping", "Waxing", "Makeup" }),
        new("Nail Art Boutique", 4, 1, "Titova 51", 43.8570, 18.4000, 5, "Creative nail art and long-lasting acrylic sets.", "nail-art-boutique", 1.10m, new[] { "Nail Art", "Manicure", "Acrylic Set" }),
        new("Polished Nail Bar", 4, 3, "Kralja Petra I 12", 44.7740, 17.1930, 3, "Friendly nail bar in Banja Luka for everyday manicures and pedicures.", "polished-nail-bar", 0.95m, new[] { "Manicure", "Pedicure", "Gel Nails" }),
        new("Serenity Day Spa", 5, 1, "Butmirska cesta 20", 43.8298, 18.3090, 4, "Day spa in Ilidza with hot stone massage and relaxing facials.", "serenity-day-spa", 1.20m, new[] { "Relaxing Facial", "Hot Stone Massage", "Massage" }),
        new("Zen Garden Wellness", 5, 4, "Tkalciceva 30", 45.8148, 15.9772, 5, "Wellness centre in Zagreb with sauna, scrubs and massage.", "zen-garden-wellness", 1.30m, new[] { "Massage", "Body Scrub", "Sauna Session" }),
    };

    private static readonly double[] TargetRatingBySalon = { 4.1, 4.4, 4.6, 4.3, 4.8, 4.0, 4.5, 4.7, 3.9, 4.6, 4.2, 4.4, 4.0, 4.8, 4.5 };

    private static readonly string[] OriginalReviewComments =
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

    private static readonly string[] PositiveComments =
    {
        "Exactly what I asked for, and the team was lovely.",
        "Clean, relaxed and very professional. Booking was easy too.",
        "Best visit I have had in a long time, will be back.",
        "Great attention to detail and fair prices.",
        "Left happier than I arrived. Highly recommended!",
        "On time, friendly and the result looks fantastic."
    };

    private static readonly string[] NeutralComments =
    {
        "Good result overall, though I had to wait a bit.",
        "Nice place, service was fine but nothing special.",
        "Decent value. I would try it again."
    };

    private static readonly string[] NegativeComments =
    {
        "Not what I expected and the visit ran late.",
        "Result was disappointing and communication could be better."
    };
}
