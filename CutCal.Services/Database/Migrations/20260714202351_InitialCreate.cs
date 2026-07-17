using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalonCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Salons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SalonCategoryId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvgRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    AutoConfirm = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Salons_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Salons_SalonCategories_SalonCategoryId",
                        column: x => x.SalonCategoryId,
                        principalTable: "SalonCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Salons_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSearchHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SalonCategoryId = table.Column<int>(type: "int", nullable: false),
                    SearchedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSearchHistories_SalonCategories_SalonCategoryId",
                        column: x => x.SalonCategoryId,
                        principalTable: "SalonCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSearchHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.UserId, x.SalonId });
                    table.ForeignKey(
                        name: "FK_Favorites_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalonGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalonGalleries_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalonServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalonServices_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalonWorkingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CloseTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalonWorkingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalonWorkingHours_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Staff_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Staff_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaypalOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalCaptureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_SalonServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "SalonServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffServices",
                columns: table => new
                {
                    StaffId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffServices", x => new { x.StaffId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_StaffServices_SalonServices_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "SalonServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffServices_Staff_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Staff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    PaypalOrderId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaypalCaptureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SalonId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalonReply = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    RemovedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Salons_SalonId",
                        column: x => x.SalonId,
                        principalTable: "Salons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_RemovedById",
                        column: x => x.RemovedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "Name" },
                values: new object[,]
                {
                    { 1, "BiH", "Sarajevo" },
                    { 2, "BiH", "Mostar" },
                    { 3, "BiH", "Banja Luka" },
                    { 4, "HR", "Zagreb" },
                    { 5, "RS", "Beograd" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Customer" },
                    { 2, "Staff" },
                    { 3, "SalonManager" },
                    { 4, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "SalonCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Hair Salon" },
                    { 2, "Barbershop" },
                    { 3, "Beauty Studio" },
                    { 4, "Nail Studio" },
                    { 5, "Spa" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Phone", "ProfileImageUrl", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@cutcal.com", "Ana", true, "Adminovic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000001", "https://i.pravatar.cc/150?img=1", "admin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin2@cutcal.com", "Amar", true, "Adminovic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000002", "https://i.pravatar.cc/150?img=2", "admin2" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@cutcal.com", "Maja", true, "Menadzer", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000003", "https://i.pravatar.cc/150?img=3", "manager" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager2@cutcal.com", "Mirza", true, "Menadzer", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000004", "https://i.pravatar.cc/150?img=4", "manager2" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager3@cutcal.com", "Merima", true, "Menadzer", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000005", "https://i.pravatar.cc/150?img=5", "manager3" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer@cutcal.com", "Emina", true, "Kupac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000006", "https://i.pravatar.cc/150?img=6", "customer" },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer2@cutcal.com", "Faruk", true, "Kupac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000007", "https://i.pravatar.cc/150?img=7", "customer2" },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer3@cutcal.com", "Lamija", true, "Kupac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000008", "https://i.pravatar.cc/150?img=8", "customer3" },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer4@cutcal.com", "Haris", true, "Kupac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000009", "https://i.pravatar.cc/150?img=9", "customer4" },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer5@cutcal.com", "Amina", true, "Kupac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761000010", "https://i.pravatar.cc/150?img=10", "customer5" },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff1@cutcal.com", "Selma", true, "Hodzic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100011", "https://i.pravatar.cc/150?img=11", "staff1" },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff2@cutcal.com", "Tarik", true, "Begic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100012", "https://i.pravatar.cc/150?img=12", "staff2" },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff3@cutcal.com", "Ajla", true, "Suljic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100013", "https://i.pravatar.cc/150?img=13", "staff3" },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff4@cutcal.com", "Kenan", true, "Delic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100014", "https://i.pravatar.cc/150?img=14", "staff4" },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff5@cutcal.com", "Dino", true, "Karic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100015", "https://i.pravatar.cc/150?img=15", "staff5" },
                    { 16, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff6@cutcal.com", "Nejra", true, "Osmic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100016", "https://i.pravatar.cc/150?img=16", "staff6" },
                    { 17, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff7@cutcal.com", "Ismar", true, "Kovac", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100017", "https://i.pravatar.cc/150?img=17", "staff7" },
                    { 18, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff8@cutcal.com", "Belma", true, "Halilovic", "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.", "+38761100018", "https://i.pravatar.cc/150?img=18", "staff8" }
                });

            migrationBuilder.InsertData(
                table: "Salons",
                columns: new[] { "Id", "Address", "AutoConfirm", "AvgRating", "CityId", "CreatedAt", "Description", "Email", "IsApproved", "Latitude", "Longitude", "Name", "OwnerId", "Phone", "ProfileImageUrl", "SalonCategoryId" },
                values: new object[,]
                {
                    { 1, "Main street 1", true, 4.2m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Bellissima Hair Studio - professional service since 2015.", "contact@salon1.cutcal.com", true, 43.856299999999997, 18.4131, "Bellissima Hair Studio", 3, "+3876210000", "https://images.unsplash.com/photo-1560066984-138dadb4c035", 1 },
                    { 2, "Main street 2", false, 4.3m, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gentleman's Cut Barbershop - professional service since 2015.", "contact@salon2.cutcal.com", true, 43.343800000000002, 17.8078, "Gentleman's Cut Barbershop", 4, "+3876210001", "https://images.unsplash.com/photo-1521590832167-7bcbfaa6381f", 2 },
                    { 3, "Main street 3", true, 4.4m, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Glow Beauty Studio - professional service since 2015.", "contact@salon3.cutcal.com", true, 44.772199999999998, 17.190999999999999, "Glow Beauty Studio", 5, "+3876210002", "https://images.unsplash.com/photo-1522337660859-02fbefca4702", 3 },
                    { 4, "Main street 4", false, 4.5m, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Perfect Nails Studio - professional service since 2015.", "contact@salon4.cutcal.com", true, 45.814999999999998, 15.9819, "Perfect Nails Studio", 3, "+3876210003", "https://images.unsplash.com/photo-1516975080664-ed2fc6a32937", 4 },
                    { 5, "Main street 5", true, 4.6m, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Relax & Spa - professional service since 2015.", "contact@salon5.cutcal.com", true, 44.7866, 20.448899999999998, "Relax & Spa", 4, "+3876210004", "https://images.unsplash.com/photo-1585747860715-2ba37e788b70", 5 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 4, 1 },
                    { 2, 4, 2 },
                    { 3, 3, 3 },
                    { 4, 3, 4 },
                    { 5, 3, 5 },
                    { 6, 1, 6 },
                    { 7, 1, 7 },
                    { 8, 1, 8 },
                    { 9, 1, 9 },
                    { 10, 1, 10 },
                    { 11, 2, 11 },
                    { 12, 2, 12 },
                    { 13, 2, 13 },
                    { 14, 2, 14 },
                    { 15, 2, 15 },
                    { 16, 2, 16 },
                    { 17, 2, 17 },
                    { 18, 2, 18 }
                });

            migrationBuilder.InsertData(
                table: "SalonGalleries",
                columns: new[] { "Id", "Caption", "ImageUrl", "SalonId", "UploadedAt" },
                values: new object[,]
                {
                    { 1, "Interior", "https://images.unsplash.com/photo-1562322140-8baeececf3df", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Interior", "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "Interior", "https://images.unsplash.com/photo-1562322140-8baeececf3df", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "Interior", "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "Interior", "https://images.unsplash.com/photo-1562322140-8baeececf3df", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "Interior", "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "Interior", "https://images.unsplash.com/photo-1562322140-8baeececf3df", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "Interior", "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "Interior", "https://images.unsplash.com/photo-1562322140-8baeececf3df", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "Interior", "https://images.unsplash.com/photo-1493256338651-d82f7acb2b38", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SalonServices",
                columns: new[] { "Id", "Description", "DurationMinutes", "IsActive", "Name", "Price", "SalonId" },
                values: new object[,]
                {
                    { 1, "Haircut performed by our professionals.", 45, true, "Haircut", 25m, 1 },
                    { 2, "Hair Coloring performed by our professionals.", 60, true, "Hair Coloring", 30m, 1 },
                    { 3, "Blow Dry performed by our professionals.", 30, true, "Blow Dry", 35m, 1 },
                    { 4, "Beard Trim performed by our professionals.", 45, true, "Beard Trim", 40m, 2 },
                    { 5, "Classic Shave performed by our professionals.", 60, true, "Classic Shave", 45m, 2 },
                    { 6, "Kids Haircut performed by our professionals.", 30, true, "Kids Haircut", 50m, 2 },
                    { 7, "Facial Treatment performed by our professionals.", 45, true, "Facial Treatment", 55m, 3 },
                    { 8, "Makeup performed by our professionals.", 60, true, "Makeup", 60m, 3 },
                    { 9, "Eyebrow Shaping performed by our professionals.", 30, true, "Eyebrow Shaping", 65m, 3 },
                    { 10, "Manicure performed by our professionals.", 45, true, "Manicure", 70m, 4 },
                    { 11, "Pedicure performed by our professionals.", 60, true, "Pedicure", 75m, 4 },
                    { 12, "Gel Nails performed by our professionals.", 30, true, "Gel Nails", 80m, 4 },
                    { 13, "Massage performed by our professionals.", 45, true, "Massage", 85m, 5 },
                    { 14, "Sauna Session performed by our professionals.", 60, true, "Sauna Session", 90m, 5 },
                    { 15, "Body Scrub performed by our professionals.", 30, true, "Body Scrub", 95m, 5 }
                });

            migrationBuilder.InsertData(
                table: "SalonWorkingHours",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "IsClosed", "OpenTime", "SalonId" },
                values: new object[,]
                {
                    { 1, new TimeOnly(20, 0, 0), 0, false, new TimeOnly(9, 0, 0), 1 },
                    { 2, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 1 },
                    { 3, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 1 },
                    { 4, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 1 },
                    { 5, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 1 },
                    { 6, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 1 },
                    { 7, null, 6, true, null, 1 },
                    { 8, new TimeOnly(20, 0, 0), 0, false, new TimeOnly(9, 0, 0), 2 },
                    { 9, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 2 },
                    { 10, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 2 },
                    { 11, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 2 },
                    { 12, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 2 },
                    { 13, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 2 },
                    { 14, null, 6, true, null, 2 },
                    { 15, new TimeOnly(20, 0, 0), 0, false, new TimeOnly(9, 0, 0), 3 },
                    { 16, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 3 },
                    { 17, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 3 },
                    { 18, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 3 },
                    { 19, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 3 },
                    { 20, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 3 },
                    { 21, null, 6, true, null, 3 },
                    { 22, new TimeOnly(20, 0, 0), 0, false, new TimeOnly(9, 0, 0), 4 },
                    { 23, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 4 },
                    { 24, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 4 },
                    { 25, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 4 },
                    { 26, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 4 },
                    { 27, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 4 },
                    { 28, null, 6, true, null, 4 },
                    { 29, new TimeOnly(20, 0, 0), 0, false, new TimeOnly(9, 0, 0), 5 },
                    { 30, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 5 },
                    { 31, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 5 },
                    { 32, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 5 },
                    { 33, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 5 },
                    { 34, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 5 },
                    { 35, null, 6, true, null, 5 }
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "Bio", "IsActive", "ProfileImageUrl", "Role", "SalonId", "UserId" },
                values: new object[,]
                {
                    { 1, "Experienced professional.", true, "https://i.pravatar.cc/150?img=11", "Stylist", 1, 11 },
                    { 2, "Experienced professional.", true, "https://i.pravatar.cc/150?img=12", "Stylist", 1, 12 },
                    { 3, "Experienced professional.", true, "https://i.pravatar.cc/150?img=13", "Stylist", 2, 13 },
                    { 4, "Experienced professional.", true, "https://i.pravatar.cc/150?img=14", "Stylist", 2, 14 },
                    { 5, "Experienced professional.", true, "https://i.pravatar.cc/150?img=15", "Stylist", 3, 15 },
                    { 6, "Experienced professional.", true, "https://i.pravatar.cc/150?img=16", "Stylist", 3, 16 },
                    { 7, "Experienced professional.", true, "https://i.pravatar.cc/150?img=17", "Stylist", 4, 17 },
                    { 8, "Experienced professional.", true, "https://i.pravatar.cc/150?img=18", "Stylist", 5, 18 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "ApprovedAt", "ApprovedById", "CancellationReason", "CreatedAt", "CustomerId", "DurationMinutes", "PaymentMethod", "PaymentStatus", "PaypalCaptureId", "PaypalOrderId", "Price", "SalonId", "ScheduledAt", "ServiceId", "StaffId", "StateName" },
                values: new object[,]
                {
                    { 1, null, null, null, new DateTime(2025, 12, 20, 10, 0, 0, 0, DateTimeKind.Utc), 6, 45, "Cash", "Unpaid", null, null, 25m, 1, new DateTime(2025, 12, 22, 10, 0, 0, 0, DateTimeKind.Utc), 1, 1, "Pending" },
                    { 2, null, null, null, new DateTime(2025, 12, 21, 11, 0, 0, 0, DateTimeKind.Utc), 7, 45, "PayPal", "Unpaid", null, null, 45m, 2, new DateTime(2025, 12, 23, 11, 0, 0, 0, DateTimeKind.Utc), 5, 4, "Pending" },
                    { 3, null, null, null, new DateTime(2025, 12, 22, 12, 0, 0, 0, DateTimeKind.Utc), 8, 45, "Cash", "Unpaid", null, null, 65m, 3, new DateTime(2025, 12, 24, 12, 0, 0, 0, DateTimeKind.Utc), 9, 5, "Pending" },
                    { 4, null, null, null, new DateTime(2025, 12, 23, 13, 0, 0, 0, DateTimeKind.Utc), 9, 45, "PayPal", "Unpaid", null, null, 70m, 4, new DateTime(2025, 12, 25, 13, 0, 0, 0, DateTimeKind.Utc), 10, 7, "Pending" },
                    { 5, new DateTime(2025, 12, 25, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2025, 12, 24, 14, 0, 0, 0, DateTimeKind.Utc), 10, 45, "Cash", "Unpaid", null, null, 90m, 5, new DateTime(2025, 12, 26, 14, 0, 0, 0, DateTimeKind.Utc), 14, 8, "Confirmed" },
                    { 6, new DateTime(2025, 12, 26, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2025, 12, 25, 15, 0, 0, 0, DateTimeKind.Utc), 6, 45, "PayPal", "Unpaid", null, null, 35m, 1, new DateTime(2025, 12, 27, 15, 0, 0, 0, DateTimeKind.Utc), 3, 2, "Confirmed" },
                    { 7, new DateTime(2025, 12, 27, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2025, 12, 26, 10, 0, 0, 0, DateTimeKind.Utc), 7, 45, "Cash", "Unpaid", null, null, 40m, 2, new DateTime(2025, 12, 28, 10, 0, 0, 0, DateTimeKind.Utc), 4, 3, "Confirmed" },
                    { 8, new DateTime(2025, 12, 28, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2025, 12, 27, 11, 0, 0, 0, DateTimeKind.Utc), 8, 45, "PayPal", "Paid", null, null, 60m, 3, new DateTime(2025, 12, 29, 11, 0, 0, 0, DateTimeKind.Utc), 8, 6, "Completed" },
                    { 9, new DateTime(2025, 12, 29, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2025, 12, 28, 12, 0, 0, 0, DateTimeKind.Utc), 9, 45, "Cash", "Paid", null, null, 80m, 4, new DateTime(2025, 12, 30, 12, 0, 0, 0, DateTimeKind.Utc), 12, 7, "Completed" },
                    { 10, new DateTime(2025, 12, 30, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2025, 12, 29, 13, 0, 0, 0, DateTimeKind.Utc), 10, 45, "PayPal", "Paid", null, null, 85m, 5, new DateTime(2025, 12, 31, 13, 0, 0, 0, DateTimeKind.Utc), 13, 8, "Completed" },
                    { 11, new DateTime(2025, 12, 31, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2025, 12, 30, 14, 0, 0, 0, DateTimeKind.Utc), 6, 45, "Cash", "Paid", null, null, 30m, 1, new DateTime(2026, 1, 1, 14, 0, 0, 0, DateTimeKind.Utc), 2, 1, "Completed" },
                    { 12, new DateTime(2026, 1, 1, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2025, 12, 31, 15, 0, 0, 0, DateTimeKind.Utc), 7, 45, "PayPal", "Paid", null, null, 50m, 2, new DateTime(2026, 1, 2, 15, 0, 0, 0, DateTimeKind.Utc), 6, 4, "Completed" },
                    { 13, new DateTime(2026, 1, 2, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 1, 10, 0, 0, 0, DateTimeKind.Utc), 8, 45, "Cash", "Paid", null, null, 55m, 3, new DateTime(2026, 1, 3, 10, 0, 0, 0, DateTimeKind.Utc), 7, 5, "Completed" },
                    { 14, new DateTime(2026, 1, 3, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 2, 11, 0, 0, 0, DateTimeKind.Utc), 9, 45, "PayPal", "Paid", null, null, 75m, 4, new DateTime(2026, 1, 4, 11, 0, 0, 0, DateTimeKind.Utc), 11, 7, "Completed" },
                    { 15, new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc), 10, 45, "Cash", "Paid", null, null, 95m, 5, new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc), 15, 8, "Completed" },
                    { 16, new DateTime(2026, 1, 5, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 4, 13, 0, 0, 0, DateTimeKind.Utc), 6, 45, "PayPal", "Paid", null, null, 25m, 1, new DateTime(2026, 1, 6, 13, 0, 0, 0, DateTimeKind.Utc), 1, 2, "Completed" },
                    { 17, new DateTime(2026, 1, 6, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 5, 14, 0, 0, 0, DateTimeKind.Utc), 7, 45, "Cash", "Paid", null, null, 45m, 2, new DateTime(2026, 1, 7, 14, 0, 0, 0, DateTimeKind.Utc), 5, 3, "Completed" },
                    { 18, null, null, "Customer requested cancellation.", new DateTime(2026, 1, 6, 15, 0, 0, 0, DateTimeKind.Utc), 8, 45, "PayPal", "Unpaid", null, null, 65m, 3, new DateTime(2026, 1, 8, 15, 0, 0, 0, DateTimeKind.Utc), 9, 6, "Cancelled" },
                    { 19, null, null, "Customer requested cancellation.", new DateTime(2026, 1, 7, 10, 0, 0, 0, DateTimeKind.Utc), 9, 45, "Cash", "Unpaid", null, null, 70m, 4, new DateTime(2026, 1, 9, 10, 0, 0, 0, DateTimeKind.Utc), 10, 7, "Cancelled" },
                    { 20, null, null, "Customer requested cancellation.", new DateTime(2026, 1, 8, 11, 0, 0, 0, DateTimeKind.Utc), 10, 45, "PayPal", "Unpaid", null, null, 90m, 5, new DateTime(2026, 1, 10, 11, 0, 0, 0, DateTimeKind.Utc), 14, 8, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "StaffServices",
                columns: new[] { "ServiceId", "StaffId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 3 },
                    { 5, 3 },
                    { 6, 3 },
                    { 4, 4 },
                    { 5, 4 },
                    { 6, 4 },
                    { 7, 5 },
                    { 8, 5 },
                    { 9, 5 },
                    { 7, 6 },
                    { 8, 6 },
                    { 9, 6 },
                    { 10, 7 },
                    { 11, 7 },
                    { 12, 7 },
                    { 13, 8 },
                    { 14, 8 },
                    { 15, 8 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "CustomerId", "IsRemoved", "Rating", "RemovedById", "SalonId", "SalonReply" },
                values: new object[,]
                {
                    { 1, 8, "Great service, highly recommend!", new DateTime(2025, 12, 29, 13, 0, 0, 0, DateTimeKind.Utc), 8, false, 3, null, 3, null },
                    { 2, 9, "Very professional staff.", new DateTime(2025, 12, 30, 14, 0, 0, 0, DateTimeKind.Utc), 9, false, 4, null, 4, null },
                    { 3, 10, "Loved the atmosphere and result.", new DateTime(2025, 12, 31, 15, 0, 0, 0, DateTimeKind.Utc), 10, false, 5, null, 5, null },
                    { 4, 11, "Will definitely come back again.", new DateTime(2026, 1, 1, 16, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, null, 1, null },
                    { 5, 12, "Friendly staff and clean salon.", new DateTime(2026, 1, 2, 17, 0, 0, 0, DateTimeKind.Utc), 7, false, 4, null, 2, null },
                    { 6, 13, "Good value for money.", new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc), 8, false, 5, null, 3, null },
                    { 7, 14, "Exceeded my expectations.", new DateTime(2026, 1, 4, 13, 0, 0, 0, DateTimeKind.Utc), 9, false, 3, null, 4, null },
                    { 8, 15, "Quick and efficient service.", new DateTime(2026, 1, 5, 14, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, null, 5, null },
                    { 9, 16, "Amazing experience overall.", new DateTime(2026, 1, 6, 15, 0, 0, 0, DateTimeKind.Utc), 6, false, 5, null, 1, null },
                    { 10, 17, "Solid work, minor wait time.", new DateTime(2026, 1, 7, 16, 0, 0, 0, DateTimeKind.Utc), 7, false, 3, null, 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ApprovedById",
                table: "Appointments",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SalonId",
                table: "Appointments",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StaffId",
                table: "Appointments",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_SalonId",
                table: "Favorites",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AppointmentId",
                table: "Payments",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AppointmentId",
                table: "Reviews",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RemovedById",
                table: "Reviews",
                column: "RemovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SalonId",
                table: "Reviews",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_SalonGalleries_SalonId",
                table: "SalonGalleries",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Salons_CityId",
                table: "Salons",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Salons_OwnerId",
                table: "Salons",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Salons_SalonCategoryId",
                table: "Salons",
                column: "SalonCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalonServices_SalonId",
                table: "SalonServices",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_SalonWorkingHours_SalonId",
                table: "SalonWorkingHours",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_SalonId",
                table: "Staff",
                column: "SalonId");

            migrationBuilder.CreateIndex(
                name: "IX_Staff_UserId",
                table: "Staff",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffServices_ServiceId",
                table: "StaffServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_RoleId",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSearchHistories_SalonCategoryId",
                table: "UserSearchHistories",
                column: "SalonCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSearchHistories_UserId",
                table: "UserSearchHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SalonGalleries");

            migrationBuilder.DropTable(
                name: "SalonWorkingHours");

            migrationBuilder.DropTable(
                name: "StaffServices");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSearchHistories");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "SalonServices");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Salons");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "SalonCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
