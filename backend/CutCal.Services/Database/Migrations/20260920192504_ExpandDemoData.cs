using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class ExpandDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApprovedAt", "ApprovedById", "PaymentStatus", "StateName" },
                values: new object[] { new DateTime(2025, 12, 21, 10, 0, 0, 0, DateTimeKind.Utc), 3, "Paid", "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApprovedAt", "ApprovedById", "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { new DateTime(2025, 12, 22, 11, 0, 0, 0, DateTimeKind.Utc), 4, 30, "Paid", 16m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApprovedAt", "ApprovedById", "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { new DateTime(2025, 12, 23, 12, 0, 0, 0, DateTimeKind.Utc), 5, 30, "Paid", 20m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApprovedAt", "ApprovedById", "PaymentStatus", "Price", "StateName" },
                values: new object[] { new DateTime(2025, 12, 24, 13, 0, 0, 0, DateTimeKind.Utc), 3, "Paid", 30m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "PaymentStatus", "Price", "StateName" },
                values: new object[] { "Paid", 28m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { 30, "Paid", 20m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { 20, "Paid", 11m, "Completed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 50m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 75, 54m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 55m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 90, 60m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 14m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 45m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 42m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 15,
                column: "Price",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 16m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 20m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 19,
                column: "Price",
                value: 30m);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 20,
                column: "Price",
                value: 28m);

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "ApprovedAt", "ApprovedById", "CancellationReason", "CreatedAt", "CustomerId", "DurationMinutes", "PaymentMethod", "PaymentStatus", "PaypalCaptureId", "PaypalOrderId", "Price", "SalonId", "ScheduledAt", "ServiceId", "StaffId", "StateName" },
                values: new object[,]
                {
                    { 101, new DateTime(2026, 1, 5, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 4, 11, 0, 0, 0, DateTimeKind.Utc), 7, 45, "Cash", "Paid", null, null, 25m, 1, new DateTime(2026, 1, 6, 11, 0, 0, 0, DateTimeKind.Utc), 1, 1, "Completed" },
                    { 102, new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc), 10, 90, "PayPal", "Paid", null, null, 60m, 1, new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc), 2, 2, "Completed" },
                    { 105, new DateTime(2026, 3, 15, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 14, 15, 0, 0, 0, DateTimeKind.Utc), 8, 90, "Cash", "Paid", null, null, 60m, 1, new DateTime(2026, 3, 16, 15, 0, 0, 0, DateTimeKind.Utc), 2, 1, "Completed" },
                    { 106, new DateTime(2026, 3, 31, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 30, 16, 0, 0, 0, DateTimeKind.Utc), 20, 30, "PayPal", "Paid", null, null, 20m, 1, new DateTime(2026, 4, 1, 16, 0, 0, 0, DateTimeKind.Utc), 3, 2, "Completed" },
                    { 108, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 3, 11, 0, 0, 0, DateTimeKind.Utc), 6, 90, "PayPal", "Paid", null, null, 60m, 1, new DateTime(2026, 5, 5, 11, 0, 0, 0, DateTimeKind.Utc), 2, 2, "Completed" },
                    { 109, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 20, 12, 0, 0, 0, DateTimeKind.Utc), 9, 30, "Cash", "Unpaid", null, null, 20m, 1, new DateTime(2026, 5, 22, 12, 0, 0, 0, DateTimeKind.Utc), 3, 1, "Cancelled" },
                    { 110, new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc), 8, 20, "Cash", "Paid", null, null, 11m, 2, new DateTime(2026, 1, 8, 12, 0, 0, 0, DateTimeKind.Utc), 4, 3, "Completed" },
                    { 111, new DateTime(2026, 1, 25, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 24, 13, 0, 0, 0, DateTimeKind.Utc), 20, 30, "PayPal", "Paid", null, null, 16m, 2, new DateTime(2026, 1, 26, 13, 0, 0, 0, DateTimeKind.Utc), 5, 4, "Completed" },
                    { 113, new DateTime(2026, 3, 1, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 28, 15, 0, 0, 0, DateTimeKind.Utc), 6, 20, "PayPal", "Paid", null, null, 11m, 2, new DateTime(2026, 3, 2, 15, 0, 0, 0, DateTimeKind.Utc), 4, 4, "Completed" },
                    { 114, new DateTime(2026, 3, 16, 16, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 15, 16, 0, 0, 0, DateTimeKind.Utc), 9, 30, "Cash", "Paid", null, null, 16m, 2, new DateTime(2026, 3, 17, 16, 0, 0, 0, DateTimeKind.Utc), 5, 3, "Completed" },
                    { 117, new DateTime(2026, 5, 6, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 5, 12, 0, 0, 0, DateTimeKind.Utc), 7, 30, "PayPal", "Paid", null, null, 16m, 2, new DateTime(2026, 5, 7, 12, 0, 0, 0, DateTimeKind.Utc), 5, 4, "Completed" },
                    { 118, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 23, 13, 0, 0, 0, DateTimeKind.Utc), 10, 30, "Cash", "Unpaid", null, null, 14m, 2, new DateTime(2026, 5, 25, 13, 0, 0, 0, DateTimeKind.Utc), 6, 3, "Cancelled" },
                    { 119, new DateTime(2026, 1, 11, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 10, 13, 0, 0, 0, DateTimeKind.Utc), 9, 60, "Cash", "Paid", null, null, 45m, 3, new DateTime(2026, 1, 12, 13, 0, 0, 0, DateTimeKind.Utc), 7, 5, "Completed" },
                    { 122, new DateTime(2026, 3, 1, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 28, 16, 0, 0, 0, DateTimeKind.Utc), 7, 60, "PayPal", "Paid", null, null, 45m, 3, new DateTime(2026, 3, 2, 16, 0, 0, 0, DateTimeKind.Utc), 7, 6, "Completed" },
                    { 123, new DateTime(2026, 3, 18, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 17, 10, 0, 0, 0, DateTimeKind.Utc), 10, 60, "Cash", "Paid", null, null, 50m, 3, new DateTime(2026, 3, 19, 10, 0, 0, 0, DateTimeKind.Utc), 8, 5, "Completed" },
                    { 126, new DateTime(2026, 5, 10, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 9, 13, 0, 0, 0, DateTimeKind.Utc), 8, 60, "PayPal", "Paid", null, null, 50m, 3, new DateTime(2026, 5, 11, 13, 0, 0, 0, DateTimeKind.Utc), 8, 6, "Completed" },
                    { 127, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 24, 14, 0, 0, 0, DateTimeKind.Utc), 20, 30, "Cash", "Unpaid", null, null, 20m, 3, new DateTime(2026, 5, 26, 14, 0, 0, 0, DateTimeKind.Utc), 9, 5, "Cancelled" },
                    { 128, new DateTime(2026, 1, 11, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 10, 14, 0, 0, 0, DateTimeKind.Utc), 10, 45, "Cash", "Paid", null, null, 30m, 4, new DateTime(2026, 1, 12, 14, 0, 0, 0, DateTimeKind.Utc), 10, 7, "Completed" },
                    { 132, new DateTime(2026, 3, 22, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 21, 11, 0, 0, 0, DateTimeKind.Utc), 20, 60, "Cash", "Paid", null, null, 42m, 4, new DateTime(2026, 3, 23, 11, 0, 0, 0, DateTimeKind.Utc), 11, 7, "Completed" },
                    { 134, new DateTime(2026, 4, 23, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 22, 13, 0, 0, 0, DateTimeKind.Utc), 6, 45, "Cash", "Paid", null, null, 30m, 4, new DateTime(2026, 4, 24, 13, 0, 0, 0, DateTimeKind.Utc), 10, 7, "Completed" },
                    { 137, new DateTime(2026, 1, 13, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 12, 15, 0, 0, 0, DateTimeKind.Utc), 20, 60, "Cash", "Paid", null, null, 55m, 5, new DateTime(2026, 1, 14, 15, 0, 0, 0, DateTimeKind.Utc), 13, 8, "Completed" },
                    { 139, new DateTime(2026, 2, 16, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 15, 10, 0, 0, 0, DateTimeKind.Utc), 6, 45, "Cash", "Paid", null, null, 50m, 5, new DateTime(2026, 2, 17, 10, 0, 0, 0, DateTimeKind.Utc), 15, 8, "Completed" },
                    { 143, new DateTime(2026, 4, 26, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 25, 14, 0, 0, 0, DateTimeKind.Utc), 7, 60, "Cash", "Paid", null, null, 55m, 5, new DateTime(2026, 4, 27, 14, 0, 0, 0, DateTimeKind.Utc), 13, 8, "Completed" }
                });

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 2,
                column: "Caption",
                value: "Our space");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 4,
                column: "Caption",
                value: "Our space");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 6,
                column: "Caption",
                value: "Our space");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 8,
                column: "Caption",
                value: "Our space");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 10,
                column: "Caption",
                value: "Our space");

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 90, 60m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "Price",
                value: 20m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 20, 11m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 16m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Price",
                value: 14m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 45m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "Price",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "Price",
                value: 20m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 10,
                column: "Price",
                value: 30m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 11,
                column: "Price",
                value: 42m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 75, 54m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 55m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 28m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 50m });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(15, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(19, 0, 0), false, new TimeOnly(8, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(15, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 23,
                column: "CloseTime",
                value: new TimeOnly(19, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 24,
                column: "CloseTime",
                value: new TimeOnly(19, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 25,
                column: "CloseTime",
                value: new TimeOnly(19, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 26,
                column: "CloseTime",
                value: new TimeOnly(19, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 27,
                column: "CloseTime",
                value: new TimeOnly(19, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(16, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(18, 0, 0), new TimeOnly(12, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(21, 0, 0), false, new TimeOnly(10, 0, 0) });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Ferhadija 12", 4.12m, "Full-service hair studio in the heart of Sarajevo: cuts, colour and styling since 2015.", "contact@bellissima-hair-studio.cutcal.com", "+3876210001" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Kralja Tomislava 8", 3.86m, "Classic barbershop with hot-towel shaves and precise fades.", "contact@gentlemans-cut-barbershop.cutcal.com", "+3876210002" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Gospodska 21", 4.57m, "Facials, makeup and brow shaping in a calm, bright studio.", "contact@glow-beauty-studio.cutcal.com", "+3876210003" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Ilica 45", 4.12m, "Manicure, pedicure and gel nails with premium polish.", "contact@perfect-nails-studio.cutcal.com", "+3876210004" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Knez Mihailova 30", 4.57m, "Massage, sauna and body treatments to unwind after a long week.", "contact@relax-and-spa.cutcal.com", "+3876210005" });

            migrationBuilder.InsertData(
                table: "Salons",
                columns: new[] { "Id", "Address", "AutoConfirm", "AvgRating", "CityId", "CreatedAt", "Description", "Email", "IsApproved", "Latitude", "Longitude", "Name", "OwnerId", "Phone", "ProfileImageUrl", "SalonCategoryId" },
                values: new object[,]
                {
                    { 6, "Zmaja od Bosne 33", false, 3.8m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern hair salon known for balayage and keratin treatments.", "contact@salon-elegance.cutcal.com", true, 43.851399999999998, 18.392199999999999, "Salon Elegance", 5, "+3876210006", "/images/salons/salon-elegance-cover.jpg", 1 },
                    { 7, "Vlaska 60", true, 4.67m, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Colour specialists in Zagreb: creative colour, precise cuts.", "contact@cut-and-color-lab.cutcal.com", true, 45.813099999999999, 15.986000000000001, "Cut & Color Lab", 3, "+3876210007", "/images/salons/cut-and-color-lab-cover.jpg", 1 },
                    { 8, "Bascarsija 5", false, 4.6m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Barbershop in the old bazaar: skin fades, beard sculpting and straight-razor shaves.", "contact@old-town-barbers.cutcal.com", true, 43.8598, 18.4313, "Old Town Barbers", 4, "+3876210008", "/images/salons/old-town-barbers-cover.jpg", 2 },
                    { 9, "Skadarska 14", true, 3.8m, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Belgrade barbershop with a relaxed atmosphere and sharp results.", "contact@sharp-edge-barbershop.cutcal.com", true, 44.816499999999998, 20.460999999999999, "Sharp Edge Barbershop", 5, "+3876210009", "/images/salons/sharp-edge-barbershop-cover.jpg", 2 },
                    { 10, "Brace Fejica 4", false, 4.17m, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lash lifts, makeup and facials in central Mostar.", "contact@lumiere-beauty-bar.cutcal.com", true, 43.342500000000001, 17.812999999999999, "Lumiere Beauty Bar", 3, "+3876210010", "/images/salons/lumiere-beauty-bar-cover.jpg", 3 },
                    { 11, "Marsala Tita 18", true, 4m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brow and makeup studio with waxing and quick touch-ups.", "contact@blush-and-brow-studio.cutcal.com", true, 43.858199999999997, 18.428999999999998, "Blush & Brow Studio", 4, "+3876210011", "/images/salons/blush-and-brow-studio-cover.jpg", 3 },
                    { 12, "Titova 51", false, 3.6m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Creative nail art and long-lasting acrylic sets.", "contact@nail-art-boutique.cutcal.com", true, 43.856999999999999, 18.399999999999999, "Nail Art Boutique", 5, "+3876210012", "/images/salons/nail-art-boutique-cover.jpg", 4 },
                    { 13, "Kralja Petra I 12", true, 3.67m, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Friendly nail bar in Banja Luka for everyday manicures and pedicures.", "contact@polished-nail-bar.cutcal.com", true, 44.774000000000001, 17.193000000000001, "Polished Nail Bar", 3, "+3876210013", "/images/salons/polished-nail-bar-cover.jpg", 4 },
                    { 14, "Butmirska cesta 20", false, 4.6m, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Day spa in Ilidza with hot stone massage and relaxing facials.", "contact@serenity-day-spa.cutcal.com", true, 43.829799999999999, 18.309000000000001, "Serenity Day Spa", 4, "+3876210014", "/images/salons/serenity-day-spa-cover.jpg", 5 },
                    { 15, "Tkalciceva 30", true, 4.4m, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Wellness centre in Zagreb with sauna, scrubs and massage.", "contact@zen-garden-wellness.cutcal.com", true, 45.814799999999998, 15.9772, "Zen Garden Wellness", 5, "+3876210015", "/images/salons/zen-garden-wellness-cover.jpg", 5 }
                });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 1,
                column: "Bio",
                value: "Stylist with several years of experience.");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 2,
                column: "Bio",
                value: "Stylist with several years of experience.");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Barber with several years of experience.", "Barber" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Barber with several years of experience.", "Barber" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Beauty specialist with several years of experience.", "Beauty specialist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Beauty specialist with several years of experience.", "Beauty specialist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Nail technician with several years of experience.", "Nail technician" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Massage therapist with several years of experience.", "Massage therapist" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Phone", "ProfileImageUrl", "Username" },
                values: new object[,]
                {
                    { 21, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff9@cutcal.com", "Amra", true, "Mujic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100021", "/images/avatars/21.jpg", "staff9" },
                    { 22, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff10@cutcal.com", "Emir", true, "Cengic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100022", "/images/avatars/22.jpg", "staff10" },
                    { 23, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff11@cutcal.com", "Lejla", true, "Basic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100023", "/images/avatars/23.jpg", "staff11" },
                    { 24, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff12@cutcal.com", "Adnan", true, "Zukic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100024", "/images/avatars/24.jpg", "staff12" },
                    { 25, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff13@cutcal.com", "Mia", true, "Pavlovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100025", "/images/avatars/25.jpg", "staff13" },
                    { 26, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff14@cutcal.com", "Armin", true, "Ramic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100026", "/images/avatars/26.jpg", "staff14" },
                    { 27, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff15@cutcal.com", "Sara", true, "Softic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100027", "/images/avatars/27.jpg", "staff15" },
                    { 28, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff16@cutcal.com", "Damir", true, "Ibrahimovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100028", "/images/avatars/28.jpg", "staff16" },
                    { 29, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff17@cutcal.com", "Ivana", true, "Jukic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100029", "/images/avatars/29.jpg", "staff17" },
                    { 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff18@cutcal.com", "Nermin", true, "Hadzic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100030", "/images/avatars/30.jpg", "staff18" },
                    { 31, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff19@cutcal.com", "Jasmina", true, "Mesic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100031", "/images/avatars/31.jpg", "staff19" },
                    { 32, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff20@cutcal.com", "Kemal", true, "Babic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100032", "/images/avatars/32.jpg", "staff20" },
                    { 33, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff21@cutcal.com", "Lana", true, "Peric", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100033", "/images/avatars/33.jpg", "staff21" },
                    { 34, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff22@cutcal.com", "Eldin", true, "Kurtovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100034", "/images/avatars/34.jpg", "staff22" },
                    { 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff23@cutcal.com", "Vesna", true, "Salihovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100035", "/images/avatars/35.jpg", "staff23" },
                    { 36, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff24@cutcal.com", "Alen", true, "Tahirovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100036", "/images/avatars/36.jpg", "staff24" },
                    { 37, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff25@cutcal.com", "Zerina", true, "Bajric", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100037", "/images/avatars/37.jpg", "staff25" },
                    { 38, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff26@cutcal.com", "Mirza", true, "Memic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100038", "/images/avatars/38.jpg", "staff26" },
                    { 39, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff27@cutcal.com", "Aida", true, "Hasanovic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100039", "/images/avatars/39.jpg", "staff27" },
                    { 40, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff28@cutcal.com", "Anel", true, "Dzafic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100040", "/images/avatars/40.jpg", "staff28" },
                    { 41, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff29@cutcal.com", "Nina", true, "Krupic", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100041", "/images/avatars/41.jpg", "staff29" },
                    { 42, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "staff30@cutcal.com", "Elvir", true, "Music", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100042", "/images/avatars/42.jpg", "staff30" },
                    { 43, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer6@cutcal.com", "Sabina", true, "Kupac", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100043", "/images/avatars/43.jpg", "customer6" },
                    { 44, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer7@cutcal.com", "Damir", true, "Kupac", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100044", "/images/avatars/44.jpg", "customer7" },
                    { 45, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer8@cutcal.com", "Jasmin", true, "Kupac", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100045", "/images/avatars/45.jpg", "customer8" },
                    { 46, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer9@cutcal.com", "Elma", true, "Kupac", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100046", "/images/avatars/46.jpg", "customer9" },
                    { 47, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer10@cutcal.com", "Almir", true, "Kupac", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761100047", "/images/avatars/47.jpg", "customer10" }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "ApprovedAt", "ApprovedById", "CancellationReason", "CreatedAt", "CustomerId", "DurationMinutes", "PaymentMethod", "PaymentStatus", "PaypalCaptureId", "PaypalOrderId", "Price", "SalonId", "ScheduledAt", "ServiceId", "StaffId", "StateName" },
                values: new object[,]
                {
                    { 103, new DateTime(2026, 2, 8, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 7, 13, 0, 0, 0, DateTimeKind.Utc), 44, 30, "Cash", "Paid", null, null, 20m, 1, new DateTime(2026, 2, 9, 13, 0, 0, 0, DateTimeKind.Utc), 3, 1, "Completed" },
                    { 104, new DateTime(2026, 2, 25, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 24, 14, 0, 0, 0, DateTimeKind.Utc), 47, 45, "PayPal", "Paid", null, null, 25m, 1, new DateTime(2026, 2, 26, 14, 0, 0, 0, DateTimeKind.Utc), 1, 2, "Completed" },
                    { 107, new DateTime(2026, 4, 19, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 18, 10, 0, 0, 0, DateTimeKind.Utc), 45, 45, "Cash", "Paid", null, null, 25m, 1, new DateTime(2026, 4, 20, 10, 0, 0, 0, DateTimeKind.Utc), 1, 1, "Completed" },
                    { 112, new DateTime(2026, 2, 10, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 9, 14, 0, 0, 0, DateTimeKind.Utc), 45, 30, "Cash", "Paid", null, null, 14m, 2, new DateTime(2026, 2, 11, 14, 0, 0, 0, DateTimeKind.Utc), 6, 3, "Completed" },
                    { 115, new DateTime(2026, 4, 2, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 1, 10, 0, 0, 0, DateTimeKind.Utc), 43, 30, "PayPal", "Paid", null, null, 14m, 2, new DateTime(2026, 4, 3, 10, 0, 0, 0, DateTimeKind.Utc), 6, 4, "Completed" },
                    { 116, new DateTime(2026, 4, 19, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 18, 11, 0, 0, 0, DateTimeKind.Utc), 46, 20, "Cash", "Paid", null, null, 11m, 2, new DateTime(2026, 4, 20, 11, 0, 0, 0, DateTimeKind.Utc), 4, 3, "Completed" },
                    { 120, new DateTime(2026, 1, 26, 14, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 25, 14, 0, 0, 0, DateTimeKind.Utc), 43, 60, "PayPal", "Paid", null, null, 50m, 3, new DateTime(2026, 1, 27, 14, 0, 0, 0, DateTimeKind.Utc), 8, 6, "Completed" },
                    { 121, new DateTime(2026, 2, 12, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 11, 15, 0, 0, 0, DateTimeKind.Utc), 46, 30, "Cash", "Paid", null, null, 20m, 3, new DateTime(2026, 2, 13, 15, 0, 0, 0, DateTimeKind.Utc), 9, 5, "Completed" },
                    { 124, new DateTime(2026, 4, 5, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 4, 11, 0, 0, 0, DateTimeKind.Utc), 44, 30, "PayPal", "Paid", null, null, 20m, 3, new DateTime(2026, 4, 6, 11, 0, 0, 0, DateTimeKind.Utc), 9, 6, "Completed" },
                    { 125, new DateTime(2026, 4, 21, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 20, 12, 0, 0, 0, DateTimeKind.Utc), 47, 60, "Cash", "Paid", null, null, 45m, 3, new DateTime(2026, 4, 22, 12, 0, 0, 0, DateTimeKind.Utc), 7, 5, "Completed" },
                    { 130, new DateTime(2026, 2, 15, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 14, 16, 0, 0, 0, DateTimeKind.Utc), 47, 75, "Cash", "Paid", null, null, 54m, 4, new DateTime(2026, 2, 16, 16, 0, 0, 0, DateTimeKind.Utc), 12, 7, "Completed" },
                    { 136, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 26, 15, 0, 0, 0, DateTimeKind.Utc), 43, 75, "Cash", "Unpaid", null, null, 54m, 4, new DateTime(2026, 5, 28, 15, 0, 0, 0, DateTimeKind.Utc), 12, 7, "Cancelled" },
                    { 141, new DateTime(2026, 3, 22, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 21, 12, 0, 0, 0, DateTimeKind.Utc), 43, 45, "Cash", "Paid", null, null, 28m, 5, new DateTime(2026, 3, 23, 12, 0, 0, 0, DateTimeKind.Utc), 14, 8, "Completed" },
                    { 145, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 30, 16, 0, 0, 0, DateTimeKind.Utc), 44, 45, "Cash", "Unpaid", null, null, 50m, 5, new DateTime(2026, 6, 1, 16, 0, 0, 0, DateTimeKind.Utc), 15, 8, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "CustomerId", "IsRemoved", "Rating", "RemovedById", "SalonId", "SalonReply" },
                values: new object[,]
                {
                    { 11, 101, "Great attention to detail and fair prices.", new DateTime(2026, 1, 6, 13, 0, 0, 0, DateTimeKind.Utc), 7, false, 4, null, 1, null },
                    { 12, 102, "Decent value. I would try it again.", new DateTime(2026, 1, 23, 14, 0, 0, 0, DateTimeKind.Utc), 10, false, 3, null, 1, null },
                    { 14, 105, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 3, 16, 17, 0, 0, 0, DateTimeKind.Utc), 8, false, 5, null, 1, null },
                    { 16, 108, "Decent value. I would try it again.", new DateTime(2026, 5, 5, 13, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, null, 1, null },
                    { 17, 110, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 1, 8, 14, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, null, 2, null },
                    { 19, 113, "Decent value. I would try it again.", new DateTime(2026, 3, 2, 17, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, null, 2, null },
                    { 24, 123, "Great attention to detail and fair prices.", new DateTime(2026, 3, 19, 12, 0, 0, 0, DateTimeKind.Utc), 10, false, 5, null, 3, null },
                    { 26, 126, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 5, 11, 15, 0, 0, 0, DateTimeKind.Utc), 8, false, 5, null, 3, null },
                    { 27, 128, "Best visit I have had in a long time, will be back.", new DateTime(2026, 1, 12, 16, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, null, 4, null },
                    { 30, 132, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 3, 23, 13, 0, 0, 0, DateTimeKind.Utc), 20, false, 4, null, 4, null },
                    { 31, 134, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 24, 15, 0, 0, 0, DateTimeKind.Utc), 6, false, 5, null, 4, null },
                    { 33, 137, "On time, friendly and the result looks fantastic.", new DateTime(2026, 1, 14, 17, 0, 0, 0, DateTimeKind.Utc), 20, false, 4, null, 5, null },
                    { 34, 139, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 2, 17, 12, 0, 0, 0, DateTimeKind.Utc), 6, false, 5, null, 5, null },
                    { 37, 143, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 4, 27, 16, 0, 0, 0, DateTimeKind.Utc), 7, false, 5, null, 5, null }
                });

            migrationBuilder.InsertData(
                table: "SalonGalleries",
                columns: new[] { "Id", "Caption", "ImageUrl", "SalonId", "UploadedAt" },
                values: new object[,]
                {
                    { 11, "Interior", "/images/salons/salon-elegance-1.jpg", 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "Our space", "/images/salons/salon-elegance-2.jpg", 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "Interior", "/images/salons/cut-and-color-lab-1.jpg", 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "Our space", "/images/salons/cut-and-color-lab-2.jpg", 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "Interior", "/images/salons/old-town-barbers-1.jpg", 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "Our space", "/images/salons/old-town-barbers-2.jpg", 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "Interior", "/images/salons/sharp-edge-barbershop-1.jpg", 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "Our space", "/images/salons/sharp-edge-barbershop-2.jpg", 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "Interior", "/images/salons/lumiere-beauty-bar-1.jpg", 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "Our space", "/images/salons/lumiere-beauty-bar-2.jpg", 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, "Interior", "/images/salons/blush-and-brow-studio-1.jpg", 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, "Our space", "/images/salons/blush-and-brow-studio-2.jpg", 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "Interior", "/images/salons/nail-art-boutique-1.jpg", 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, "Our space", "/images/salons/nail-art-boutique-2.jpg", 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, "Interior", "/images/salons/polished-nail-bar-1.jpg", 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, "Our space", "/images/salons/polished-nail-bar-2.jpg", 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, "Interior", "/images/salons/serenity-day-spa-1.jpg", 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, "Our space", "/images/salons/serenity-day-spa-2.jpg", 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, "Interior", "/images/salons/zen-garden-wellness-1.jpg", 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, "Our space", "/images/salons/zen-garden-wellness-2.jpg", 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SalonServices",
                columns: new[] { "Id", "Description", "DurationMinutes", "IsActive", "Name", "Price", "SalonId" },
                values: new object[,]
                {
                    { 101, "Haircut performed by our professionals.", 45, true, "Haircut", 29m, 6 },
                    { 102, "Balayage performed by our professionals.", 120, true, "Balayage", 126m, 6 },
                    { 103, "Keratin Treatment performed by our professionals.", 90, true, "Keratin Treatment", 92m, 6 },
                    { 104, "Haircut performed by our professionals.", 45, true, "Haircut", 31m, 7 },
                    { 105, "Hair Coloring performed by our professionals.", 90, true, "Hair Coloring", 75m, 7 },
                    { 106, "Kids Haircut performed by our professionals.", 30, true, "Kids Haircut", 19m, 7 },
                    { 107, "Haircut & Beard performed by our professionals.", 60, true, "Haircut & Beard", 32m, 8 },
                    { 108, "Fade Haircut performed by our professionals.", 45, true, "Fade Haircut", 22m, 8 },
                    { 109, "Beard Trim performed by our professionals.", 20, true, "Beard Trim", 12m, 8 },
                    { 110, "Classic Shave performed by our professionals.", 30, true, "Classic Shave", 17m, 9 },
                    { 111, "Fade Haircut performed by our professionals.", 45, true, "Fade Haircut", 21m, 9 },
                    { 112, "Kids Haircut performed by our professionals.", 30, true, "Kids Haircut", 14m, 9 },
                    { 113, "Lash Lift performed by our professionals.", 60, true, "Lash Lift", 50m, 10 },
                    { 114, "Makeup performed by our professionals.", 60, true, "Makeup", 45m, 10 },
                    { 115, "Facial Treatment performed by our professionals.", 60, true, "Facial Treatment", 40m, 10 },
                    { 116, "Eyebrow Shaping performed by our professionals.", 30, true, "Eyebrow Shaping", 21m, 11 },
                    { 117, "Waxing performed by our professionals.", 30, true, "Waxing", 26m, 11 },
                    { 118, "Makeup performed by our professionals.", 60, true, "Makeup", 52m, 11 },
                    { 119, "Nail Art performed by our professionals.", 60, true, "Nail Art", 44m, 12 },
                    { 120, "Manicure performed by our professionals.", 45, true, "Manicure", 28m, 12 },
                    { 121, "Acrylic Set performed by our professionals.", 90, true, "Acrylic Set", 66m, 12 },
                    { 122, "Manicure performed by our professionals.", 45, true, "Manicure", 24m, 13 },
                    { 123, "Pedicure performed by our professionals.", 60, true, "Pedicure", 33m, 13 },
                    { 124, "Gel Nails performed by our professionals.", 75, true, "Gel Nails", 43m, 13 },
                    { 125, "Relaxing Facial performed by our professionals.", 60, true, "Relaxing Facial", 60m, 14 },
                    { 126, "Hot Stone Massage performed by our professionals.", 75, true, "Hot Stone Massage", 90m, 14 },
                    { 127, "Massage performed by our professionals.", 60, true, "Massage", 60m, 14 },
                    { 128, "Massage performed by our professionals.", 60, true, "Massage", 65m, 15 },
                    { 129, "Body Scrub performed by our professionals.", 45, true, "Body Scrub", 58m, 15 },
                    { 130, "Sauna Session performed by our professionals.", 45, true, "Sauna Session", 32m, 15 }
                });

            migrationBuilder.InsertData(
                table: "SalonWorkingHours",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "IsClosed", "OpenTime", "SalonId" },
                values: new object[,]
                {
                    { 101, null, 0, true, null, 6 },
                    { 102, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 6 },
                    { 103, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 6 },
                    { 104, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 6 },
                    { 105, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 6 },
                    { 106, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 6 },
                    { 107, new TimeOnly(15, 0, 0), 6, false, new TimeOnly(9, 0, 0), 6 },
                    { 108, null, 0, true, null, 7 },
                    { 109, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 7 },
                    { 110, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 7 },
                    { 111, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 7 },
                    { 112, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 7 },
                    { 113, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 7 },
                    { 114, new TimeOnly(15, 0, 0), 6, false, new TimeOnly(9, 0, 0), 7 },
                    { 115, null, 0, true, null, 8 },
                    { 116, new TimeOnly(19, 0, 0), 1, false, new TimeOnly(8, 0, 0), 8 },
                    { 117, new TimeOnly(19, 0, 0), 2, false, new TimeOnly(8, 0, 0), 8 },
                    { 118, new TimeOnly(19, 0, 0), 3, false, new TimeOnly(8, 0, 0), 8 },
                    { 119, new TimeOnly(19, 0, 0), 4, false, new TimeOnly(8, 0, 0), 8 },
                    { 120, new TimeOnly(19, 0, 0), 5, false, new TimeOnly(8, 0, 0), 8 },
                    { 121, new TimeOnly(19, 0, 0), 6, false, new TimeOnly(8, 0, 0), 8 },
                    { 122, null, 0, true, null, 9 },
                    { 123, new TimeOnly(19, 0, 0), 1, false, new TimeOnly(8, 0, 0), 9 },
                    { 124, new TimeOnly(19, 0, 0), 2, false, new TimeOnly(8, 0, 0), 9 },
                    { 125, new TimeOnly(19, 0, 0), 3, false, new TimeOnly(8, 0, 0), 9 },
                    { 126, new TimeOnly(19, 0, 0), 4, false, new TimeOnly(8, 0, 0), 9 },
                    { 127, new TimeOnly(19, 0, 0), 5, false, new TimeOnly(8, 0, 0), 9 },
                    { 128, new TimeOnly(19, 0, 0), 6, false, new TimeOnly(8, 0, 0), 9 },
                    { 129, null, 0, true, null, 10 },
                    { 130, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 10 },
                    { 131, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 10 },
                    { 132, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 10 },
                    { 133, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 10 },
                    { 134, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 10 },
                    { 135, new TimeOnly(15, 0, 0), 6, false, new TimeOnly(9, 0, 0), 10 },
                    { 136, null, 0, true, null, 11 },
                    { 137, new TimeOnly(20, 0, 0), 1, false, new TimeOnly(9, 0, 0), 11 },
                    { 138, new TimeOnly(20, 0, 0), 2, false, new TimeOnly(9, 0, 0), 11 },
                    { 139, new TimeOnly(20, 0, 0), 3, false, new TimeOnly(9, 0, 0), 11 },
                    { 140, new TimeOnly(20, 0, 0), 4, false, new TimeOnly(9, 0, 0), 11 },
                    { 141, new TimeOnly(20, 0, 0), 5, false, new TimeOnly(9, 0, 0), 11 },
                    { 142, new TimeOnly(15, 0, 0), 6, false, new TimeOnly(9, 0, 0), 11 },
                    { 143, null, 0, true, null, 12 },
                    { 144, new TimeOnly(19, 0, 0), 1, false, new TimeOnly(9, 0, 0), 12 },
                    { 145, new TimeOnly(19, 0, 0), 2, false, new TimeOnly(9, 0, 0), 12 },
                    { 146, new TimeOnly(19, 0, 0), 3, false, new TimeOnly(9, 0, 0), 12 },
                    { 147, new TimeOnly(19, 0, 0), 4, false, new TimeOnly(9, 0, 0), 12 },
                    { 148, new TimeOnly(19, 0, 0), 5, false, new TimeOnly(9, 0, 0), 12 },
                    { 149, new TimeOnly(16, 0, 0), 6, false, new TimeOnly(9, 0, 0), 12 },
                    { 150, null, 0, true, null, 13 },
                    { 151, new TimeOnly(19, 0, 0), 1, false, new TimeOnly(9, 0, 0), 13 },
                    { 152, new TimeOnly(19, 0, 0), 2, false, new TimeOnly(9, 0, 0), 13 },
                    { 153, new TimeOnly(19, 0, 0), 3, false, new TimeOnly(9, 0, 0), 13 },
                    { 154, new TimeOnly(19, 0, 0), 4, false, new TimeOnly(9, 0, 0), 13 },
                    { 155, new TimeOnly(19, 0, 0), 5, false, new TimeOnly(9, 0, 0), 13 },
                    { 156, new TimeOnly(16, 0, 0), 6, false, new TimeOnly(9, 0, 0), 13 },
                    { 157, new TimeOnly(18, 0, 0), 0, false, new TimeOnly(12, 0, 0), 14 },
                    { 158, new TimeOnly(21, 0, 0), 1, false, new TimeOnly(10, 0, 0), 14 },
                    { 159, new TimeOnly(21, 0, 0), 2, false, new TimeOnly(10, 0, 0), 14 },
                    { 160, new TimeOnly(21, 0, 0), 3, false, new TimeOnly(10, 0, 0), 14 },
                    { 161, new TimeOnly(21, 0, 0), 4, false, new TimeOnly(10, 0, 0), 14 },
                    { 162, new TimeOnly(21, 0, 0), 5, false, new TimeOnly(10, 0, 0), 14 },
                    { 163, new TimeOnly(21, 0, 0), 6, false, new TimeOnly(10, 0, 0), 14 },
                    { 164, new TimeOnly(18, 0, 0), 0, false, new TimeOnly(12, 0, 0), 15 },
                    { 165, new TimeOnly(21, 0, 0), 1, false, new TimeOnly(10, 0, 0), 15 },
                    { 166, new TimeOnly(21, 0, 0), 2, false, new TimeOnly(10, 0, 0), 15 },
                    { 167, new TimeOnly(21, 0, 0), 3, false, new TimeOnly(10, 0, 0), 15 },
                    { 168, new TimeOnly(21, 0, 0), 4, false, new TimeOnly(10, 0, 0), 15 },
                    { 169, new TimeOnly(21, 0, 0), 5, false, new TimeOnly(10, 0, 0), 15 },
                    { 170, new TimeOnly(21, 0, 0), 6, false, new TimeOnly(10, 0, 0), 15 }
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "Bio", "IsActive", "ProfileImageUrl", "Role", "SalonId", "UserId" },
                values: new object[,]
                {
                    { 9, "Nail technician with several years of experience.", true, "/images/avatars/21.jpg", "Nail technician", 4, 21 },
                    { 10, "Massage therapist with several years of experience.", true, "/images/avatars/22.jpg", "Massage therapist", 5, 22 },
                    { 11, "Stylist with several years of experience.", true, "/images/avatars/23.jpg", "Stylist", 6, 23 },
                    { 12, "Stylist with several years of experience.", true, "/images/avatars/24.jpg", "Stylist", 6, 24 },
                    { 13, "Stylist with several years of experience.", true, "/images/avatars/25.jpg", "Stylist", 7, 25 },
                    { 14, "Stylist with several years of experience.", true, "/images/avatars/26.jpg", "Stylist", 7, 26 },
                    { 15, "Barber with several years of experience.", true, "/images/avatars/27.jpg", "Barber", 8, 27 },
                    { 16, "Barber with several years of experience.", true, "/images/avatars/28.jpg", "Barber", 8, 28 },
                    { 17, "Barber with several years of experience.", true, "/images/avatars/29.jpg", "Barber", 9, 29 },
                    { 18, "Barber with several years of experience.", true, "/images/avatars/30.jpg", "Barber", 9, 30 },
                    { 19, "Beauty specialist with several years of experience.", true, "/images/avatars/31.jpg", "Beauty specialist", 10, 31 },
                    { 20, "Beauty specialist with several years of experience.", true, "/images/avatars/32.jpg", "Beauty specialist", 10, 32 },
                    { 21, "Beauty specialist with several years of experience.", true, "/images/avatars/33.jpg", "Beauty specialist", 11, 33 },
                    { 22, "Beauty specialist with several years of experience.", true, "/images/avatars/34.jpg", "Beauty specialist", 11, 34 },
                    { 23, "Nail technician with several years of experience.", true, "/images/avatars/35.jpg", "Nail technician", 12, 35 },
                    { 24, "Nail technician with several years of experience.", true, "/images/avatars/36.jpg", "Nail technician", 12, 36 },
                    { 25, "Nail technician with several years of experience.", true, "/images/avatars/37.jpg", "Nail technician", 13, 37 },
                    { 26, "Nail technician with several years of experience.", true, "/images/avatars/38.jpg", "Nail technician", 13, 38 },
                    { 27, "Massage therapist with several years of experience.", true, "/images/avatars/39.jpg", "Massage therapist", 14, 39 },
                    { 28, "Massage therapist with several years of experience.", true, "/images/avatars/40.jpg", "Massage therapist", 14, 40 },
                    { 29, "Massage therapist with several years of experience.", true, "/images/avatars/41.jpg", "Massage therapist", 15, 41 },
                    { 30, "Massage therapist with several years of experience.", true, "/images/avatars/42.jpg", "Massage therapist", 15, 42 }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 21, 2, 21 },
                    { 22, 2, 22 },
                    { 23, 2, 23 },
                    { 24, 2, 24 },
                    { 25, 2, 25 },
                    { 26, 2, 26 },
                    { 27, 2, 27 },
                    { 28, 2, 28 },
                    { 29, 2, 29 },
                    { 30, 2, 30 },
                    { 31, 2, 31 },
                    { 32, 2, 32 },
                    { 33, 2, 33 },
                    { 34, 2, 34 },
                    { 35, 2, 35 },
                    { 36, 2, 36 },
                    { 37, 2, 37 },
                    { 38, 2, 38 },
                    { 39, 2, 39 },
                    { 40, 2, 40 },
                    { 41, 2, 41 },
                    { 42, 2, 42 },
                    { 43, 1, 43 },
                    { 44, 1, 44 },
                    { 45, 1, 45 },
                    { 46, 1, 46 },
                    { 47, 1, 47 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "ApprovedAt", "ApprovedById", "CancellationReason", "CreatedAt", "CustomerId", "DurationMinutes", "PaymentMethod", "PaymentStatus", "PaypalCaptureId", "PaypalOrderId", "Price", "SalonId", "ScheduledAt", "ServiceId", "StaffId", "StateName" },
                values: new object[,]
                {
                    { 129, new DateTime(2026, 1, 28, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 27, 15, 0, 0, 0, DateTimeKind.Utc), 44, 60, "PayPal", "Paid", null, null, 42m, 4, new DateTime(2026, 1, 29, 15, 0, 0, 0, DateTimeKind.Utc), 11, 9, "Completed" },
                    { 131, new DateTime(2026, 3, 3, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 2, 10, 0, 0, 0, DateTimeKind.Utc), 8, 45, "PayPal", "Paid", null, null, 30m, 4, new DateTime(2026, 3, 4, 10, 0, 0, 0, DateTimeKind.Utc), 10, 9, "Completed" },
                    { 133, new DateTime(2026, 4, 6, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), 45, 75, "PayPal", "Paid", null, null, 54m, 4, new DateTime(2026, 4, 7, 12, 0, 0, 0, DateTimeKind.Utc), 12, 9, "Completed" },
                    { 135, new DateTime(2026, 5, 10, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 9, 14, 0, 0, 0, DateTimeKind.Utc), 9, 60, "PayPal", "Paid", null, null, 42m, 4, new DateTime(2026, 5, 11, 14, 0, 0, 0, DateTimeKind.Utc), 11, 9, "Completed" },
                    { 138, new DateTime(2026, 2, 1, 16, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 31, 16, 0, 0, 0, DateTimeKind.Utc), 45, 45, "PayPal", "Paid", null, null, 28m, 5, new DateTime(2026, 2, 2, 16, 0, 0, 0, DateTimeKind.Utc), 14, 10, "Completed" },
                    { 140, new DateTime(2026, 3, 5, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 4, 11, 0, 0, 0, DateTimeKind.Utc), 9, 60, "PayPal", "Paid", null, null, 55m, 5, new DateTime(2026, 3, 6, 11, 0, 0, 0, DateTimeKind.Utc), 13, 10, "Completed" },
                    { 142, new DateTime(2026, 4, 8, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 7, 13, 0, 0, 0, DateTimeKind.Utc), 46, 45, "PayPal", "Paid", null, null, 50m, 5, new DateTime(2026, 4, 9, 13, 0, 0, 0, DateTimeKind.Utc), 15, 10, "Completed" },
                    { 144, new DateTime(2026, 5, 12, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 11, 15, 0, 0, 0, DateTimeKind.Utc), 10, 45, "PayPal", "Paid", null, null, 28m, 5, new DateTime(2026, 5, 13, 15, 0, 0, 0, DateTimeKind.Utc), 14, 10, "Completed" },
                    { 146, new DateTime(2026, 1, 15, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 14, 16, 0, 0, 0, DateTimeKind.Utc), 43, 45, "Cash", "Paid", null, null, 29m, 6, new DateTime(2026, 1, 16, 16, 0, 0, 0, DateTimeKind.Utc), 101, 11, "Completed" },
                    { 147, new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Utc), 46, 120, "PayPal", "Paid", null, null, 126m, 6, new DateTime(2026, 2, 2, 10, 0, 0, 0, DateTimeKind.Utc), 102, 12, "Completed" },
                    { 148, new DateTime(2026, 2, 18, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 17, 11, 0, 0, 0, DateTimeKind.Utc), 7, 90, "Cash", "Paid", null, null, 92m, 6, new DateTime(2026, 2, 19, 11, 0, 0, 0, DateTimeKind.Utc), 103, 11, "Completed" },
                    { 149, new DateTime(2026, 3, 8, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 7, 12, 0, 0, 0, DateTimeKind.Utc), 10, 45, "PayPal", "Paid", null, null, 29m, 6, new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc), 101, 12, "Completed" },
                    { 150, new DateTime(2026, 3, 24, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 23, 13, 0, 0, 0, DateTimeKind.Utc), 44, 120, "Cash", "Paid", null, null, 126m, 6, new DateTime(2026, 3, 25, 13, 0, 0, 0, DateTimeKind.Utc), 102, 11, "Completed" },
                    { 151, new DateTime(2026, 4, 12, 14, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 11, 14, 0, 0, 0, DateTimeKind.Utc), 47, 90, "PayPal", "Paid", null, null, 92m, 6, new DateTime(2026, 4, 13, 14, 0, 0, 0, DateTimeKind.Utc), 103, 12, "Completed" },
                    { 152, new DateTime(2026, 4, 27, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 26, 15, 0, 0, 0, DateTimeKind.Utc), 8, 45, "Cash", "Paid", null, null, 29m, 6, new DateTime(2026, 4, 28, 15, 0, 0, 0, DateTimeKind.Utc), 101, 11, "Completed" },
                    { 153, new DateTime(2026, 5, 14, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 13, 16, 0, 0, 0, DateTimeKind.Utc), 20, 120, "PayPal", "Paid", null, null, 126m, 6, new DateTime(2026, 5, 15, 16, 0, 0, 0, DateTimeKind.Utc), 102, 12, "Completed" },
                    { 154, null, null, "Customer requested cancellation.", new DateTime(2026, 5, 30, 10, 0, 0, 0, DateTimeKind.Utc), 45, 90, "Cash", "Unpaid", null, null, 92m, 6, new DateTime(2026, 6, 1, 10, 0, 0, 0, DateTimeKind.Utc), 103, 11, "Cancelled" },
                    { 155, new DateTime(2026, 1, 18, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 17, 10, 0, 0, 0, DateTimeKind.Utc), 44, 45, "Cash", "Paid", null, null, 31m, 7, new DateTime(2026, 1, 19, 10, 0, 0, 0, DateTimeKind.Utc), 104, 13, "Completed" },
                    { 156, new DateTime(2026, 2, 3, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 2, 11, 0, 0, 0, DateTimeKind.Utc), 47, 90, "PayPal", "Paid", null, null, 75m, 7, new DateTime(2026, 2, 4, 11, 0, 0, 0, DateTimeKind.Utc), 105, 14, "Completed" },
                    { 157, new DateTime(2026, 2, 22, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 21, 12, 0, 0, 0, DateTimeKind.Utc), 8, 30, "Cash", "Paid", null, null, 19m, 7, new DateTime(2026, 2, 23, 12, 0, 0, 0, DateTimeKind.Utc), 106, 13, "Completed" },
                    { 158, new DateTime(2026, 3, 9, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 8, 13, 0, 0, 0, DateTimeKind.Utc), 20, 45, "PayPal", "Paid", null, null, 31m, 7, new DateTime(2026, 3, 10, 13, 0, 0, 0, DateTimeKind.Utc), 104, 14, "Completed" },
                    { 159, new DateTime(2026, 3, 26, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 25, 14, 0, 0, 0, DateTimeKind.Utc), 45, 90, "Cash", "Paid", null, null, 75m, 7, new DateTime(2026, 3, 27, 14, 0, 0, 0, DateTimeKind.Utc), 105, 13, "Completed" },
                    { 160, new DateTime(2026, 4, 12, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 11, 15, 0, 0, 0, DateTimeKind.Utc), 6, 30, "PayPal", "Paid", null, null, 19m, 7, new DateTime(2026, 4, 13, 15, 0, 0, 0, DateTimeKind.Utc), 106, 14, "Completed" },
                    { 161, new DateTime(2026, 4, 29, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 28, 16, 0, 0, 0, DateTimeKind.Utc), 9, 45, "Cash", "Paid", null, null, 31m, 7, new DateTime(2026, 4, 30, 16, 0, 0, 0, DateTimeKind.Utc), 104, 13, "Completed" },
                    { 162, new DateTime(2026, 5, 17, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 16, 10, 0, 0, 0, DateTimeKind.Utc), 43, 90, "PayPal", "Paid", null, null, 75m, 7, new DateTime(2026, 5, 18, 10, 0, 0, 0, DateTimeKind.Utc), 105, 14, "Completed" },
                    { 163, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 1, 11, 0, 0, 0, DateTimeKind.Utc), 46, 30, "Cash", "Unpaid", null, null, 19m, 7, new DateTime(2026, 6, 3, 11, 0, 0, 0, DateTimeKind.Utc), 106, 13, "Cancelled" },
                    { 164, new DateTime(2026, 1, 19, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 18, 11, 0, 0, 0, DateTimeKind.Utc), 45, 60, "Cash", "Paid", null, null, 32m, 8, new DateTime(2026, 1, 20, 11, 0, 0, 0, DateTimeKind.Utc), 107, 15, "Completed" },
                    { 165, new DateTime(2026, 2, 5, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 4, 12, 0, 0, 0, DateTimeKind.Utc), 6, 45, "PayPal", "Paid", null, null, 22m, 8, new DateTime(2026, 2, 6, 12, 0, 0, 0, DateTimeKind.Utc), 108, 16, "Completed" },
                    { 166, new DateTime(2026, 2, 22, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 21, 13, 0, 0, 0, DateTimeKind.Utc), 9, 20, "Cash", "Paid", null, null, 12m, 8, new DateTime(2026, 2, 23, 13, 0, 0, 0, DateTimeKind.Utc), 109, 15, "Completed" },
                    { 167, new DateTime(2026, 3, 11, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 10, 14, 0, 0, 0, DateTimeKind.Utc), 43, 60, "PayPal", "Paid", null, null, 32m, 8, new DateTime(2026, 3, 12, 14, 0, 0, 0, DateTimeKind.Utc), 107, 16, "Completed" },
                    { 168, new DateTime(2026, 3, 29, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 28, 15, 0, 0, 0, DateTimeKind.Utc), 46, 45, "Cash", "Paid", null, null, 22m, 8, new DateTime(2026, 3, 30, 15, 0, 0, 0, DateTimeKind.Utc), 108, 15, "Completed" },
                    { 169, new DateTime(2026, 4, 14, 16, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 13, 16, 0, 0, 0, DateTimeKind.Utc), 7, 20, "PayPal", "Paid", null, null, 12m, 8, new DateTime(2026, 4, 15, 16, 0, 0, 0, DateTimeKind.Utc), 109, 16, "Completed" },
                    { 170, new DateTime(2026, 5, 3, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 2, 10, 0, 0, 0, DateTimeKind.Utc), 10, 60, "Cash", "Paid", null, null, 32m, 8, new DateTime(2026, 5, 4, 10, 0, 0, 0, DateTimeKind.Utc), 107, 15, "Completed" },
                    { 171, new DateTime(2026, 5, 18, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 17, 11, 0, 0, 0, DateTimeKind.Utc), 44, 45, "PayPal", "Paid", null, null, 22m, 8, new DateTime(2026, 5, 19, 11, 0, 0, 0, DateTimeKind.Utc), 108, 16, "Completed" },
                    { 172, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 3, 12, 0, 0, 0, DateTimeKind.Utc), 47, 20, "Cash", "Unpaid", null, null, 12m, 8, new DateTime(2026, 6, 5, 12, 0, 0, 0, DateTimeKind.Utc), 109, 15, "Cancelled" },
                    { 173, new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc), 46, 30, "Cash", "Paid", null, null, 17m, 9, new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc), 110, 17, "Completed" },
                    { 174, new DateTime(2026, 2, 8, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 7, 13, 0, 0, 0, DateTimeKind.Utc), 7, 45, "PayPal", "Paid", null, null, 21m, 9, new DateTime(2026, 2, 9, 13, 0, 0, 0, DateTimeKind.Utc), 111, 18, "Completed" },
                    { 175, new DateTime(2026, 2, 24, 14, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 23, 14, 0, 0, 0, DateTimeKind.Utc), 10, 30, "Cash", "Paid", null, null, 14m, 9, new DateTime(2026, 2, 25, 14, 0, 0, 0, DateTimeKind.Utc), 112, 17, "Completed" },
                    { 176, new DateTime(2026, 3, 15, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 14, 15, 0, 0, 0, DateTimeKind.Utc), 44, 30, "PayPal", "Paid", null, null, 17m, 9, new DateTime(2026, 3, 16, 15, 0, 0, 0, DateTimeKind.Utc), 110, 18, "Completed" },
                    { 177, new DateTime(2026, 3, 30, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 29, 16, 0, 0, 0, DateTimeKind.Utc), 47, 45, "Cash", "Paid", null, null, 21m, 9, new DateTime(2026, 3, 31, 16, 0, 0, 0, DateTimeKind.Utc), 111, 17, "Completed" },
                    { 178, new DateTime(2026, 4, 16, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 15, 10, 0, 0, 0, DateTimeKind.Utc), 8, 30, "PayPal", "Paid", null, null, 14m, 9, new DateTime(2026, 4, 17, 10, 0, 0, 0, DateTimeKind.Utc), 112, 18, "Completed" },
                    { 179, new DateTime(2026, 5, 3, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 2, 11, 0, 0, 0, DateTimeKind.Utc), 20, 30, "Cash", "Paid", null, null, 17m, 9, new DateTime(2026, 5, 4, 11, 0, 0, 0, DateTimeKind.Utc), 110, 17, "Completed" },
                    { 180, new DateTime(2026, 5, 20, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 19, 12, 0, 0, 0, DateTimeKind.Utc), 45, 45, "PayPal", "Paid", null, null, 21m, 9, new DateTime(2026, 5, 21, 12, 0, 0, 0, DateTimeKind.Utc), 111, 18, "Completed" },
                    { 181, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 6, 13, 0, 0, 0, DateTimeKind.Utc), 6, 30, "Cash", "Unpaid", null, null, 14m, 9, new DateTime(2026, 6, 8, 13, 0, 0, 0, DateTimeKind.Utc), 112, 17, "Cancelled" },
                    { 182, new DateTime(2026, 1, 25, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 24, 13, 0, 0, 0, DateTimeKind.Utc), 47, 60, "Cash", "Paid", null, null, 50m, 10, new DateTime(2026, 1, 26, 13, 0, 0, 0, DateTimeKind.Utc), 113, 19, "Completed" },
                    { 183, new DateTime(2026, 2, 9, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 8, 14, 0, 0, 0, DateTimeKind.Utc), 8, 60, "PayPal", "Paid", null, null, 45m, 10, new DateTime(2026, 2, 10, 14, 0, 0, 0, DateTimeKind.Utc), 114, 20, "Completed" },
                    { 184, new DateTime(2026, 2, 26, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 25, 15, 0, 0, 0, DateTimeKind.Utc), 20, 60, "Cash", "Paid", null, null, 40m, 10, new DateTime(2026, 2, 27, 15, 0, 0, 0, DateTimeKind.Utc), 115, 19, "Completed" },
                    { 185, new DateTime(2026, 3, 15, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 14, 16, 0, 0, 0, DateTimeKind.Utc), 45, 60, "PayPal", "Paid", null, null, 50m, 10, new DateTime(2026, 3, 16, 16, 0, 0, 0, DateTimeKind.Utc), 113, 20, "Completed" },
                    { 186, new DateTime(2026, 4, 1, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 31, 10, 0, 0, 0, DateTimeKind.Utc), 6, 60, "Cash", "Paid", null, null, 45m, 10, new DateTime(2026, 4, 2, 10, 0, 0, 0, DateTimeKind.Utc), 114, 19, "Completed" },
                    { 187, new DateTime(2026, 4, 19, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 18, 11, 0, 0, 0, DateTimeKind.Utc), 9, 60, "PayPal", "Paid", null, null, 40m, 10, new DateTime(2026, 4, 20, 11, 0, 0, 0, DateTimeKind.Utc), 115, 20, "Completed" },
                    { 188, new DateTime(2026, 5, 5, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 4, 12, 0, 0, 0, DateTimeKind.Utc), 43, 60, "Cash", "Paid", null, null, 50m, 10, new DateTime(2026, 5, 6, 12, 0, 0, 0, DateTimeKind.Utc), 113, 19, "Completed" },
                    { 189, new DateTime(2026, 5, 24, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 23, 13, 0, 0, 0, DateTimeKind.Utc), 46, 60, "PayPal", "Paid", null, null, 45m, 10, new DateTime(2026, 5, 25, 13, 0, 0, 0, DateTimeKind.Utc), 114, 20, "Completed" },
                    { 190, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 7, 14, 0, 0, 0, DateTimeKind.Utc), 7, 60, "Cash", "Unpaid", null, null, 40m, 10, new DateTime(2026, 6, 9, 14, 0, 0, 0, DateTimeKind.Utc), 115, 19, "Cancelled" },
                    { 191, new DateTime(2026, 1, 25, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 24, 14, 0, 0, 0, DateTimeKind.Utc), 6, 30, "Cash", "Paid", null, null, 21m, 11, new DateTime(2026, 1, 26, 14, 0, 0, 0, DateTimeKind.Utc), 116, 21, "Completed" },
                    { 192, new DateTime(2026, 2, 11, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 10, 15, 0, 0, 0, DateTimeKind.Utc), 9, 30, "PayPal", "Paid", null, null, 26m, 11, new DateTime(2026, 2, 12, 15, 0, 0, 0, DateTimeKind.Utc), 117, 22, "Completed" },
                    { 193, new DateTime(2026, 3, 1, 16, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 28, 16, 0, 0, 0, DateTimeKind.Utc), 43, 60, "Cash", "Paid", null, null, 52m, 11, new DateTime(2026, 3, 2, 16, 0, 0, 0, DateTimeKind.Utc), 118, 21, "Completed" },
                    { 194, new DateTime(2026, 3, 17, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 16, 10, 0, 0, 0, DateTimeKind.Utc), 46, 30, "PayPal", "Paid", null, null, 21m, 11, new DateTime(2026, 3, 18, 10, 0, 0, 0, DateTimeKind.Utc), 116, 22, "Completed" },
                    { 195, new DateTime(2026, 4, 5, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 4, 11, 0, 0, 0, DateTimeKind.Utc), 7, 30, "Cash", "Paid", null, null, 26m, 11, new DateTime(2026, 4, 6, 11, 0, 0, 0, DateTimeKind.Utc), 117, 21, "Completed" },
                    { 196, new DateTime(2026, 4, 20, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 19, 12, 0, 0, 0, DateTimeKind.Utc), 10, 60, "PayPal", "Paid", null, null, 52m, 11, new DateTime(2026, 4, 21, 12, 0, 0, 0, DateTimeKind.Utc), 118, 22, "Completed" },
                    { 197, new DateTime(2026, 5, 7, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 6, 13, 0, 0, 0, DateTimeKind.Utc), 44, 30, "Cash", "Paid", null, null, 21m, 11, new DateTime(2026, 5, 8, 13, 0, 0, 0, DateTimeKind.Utc), 116, 21, "Completed" },
                    { 198, new DateTime(2026, 5, 24, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 23, 14, 0, 0, 0, DateTimeKind.Utc), 47, 30, "PayPal", "Paid", null, null, 26m, 11, new DateTime(2026, 5, 25, 14, 0, 0, 0, DateTimeKind.Utc), 117, 22, "Completed" },
                    { 199, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 9, 15, 0, 0, 0, DateTimeKind.Utc), 8, 60, "Cash", "Unpaid", null, null, 52m, 11, new DateTime(2026, 6, 11, 15, 0, 0, 0, DateTimeKind.Utc), 118, 21, "Cancelled" },
                    { 200, new DateTime(2026, 1, 27, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 1, 26, 15, 0, 0, 0, DateTimeKind.Utc), 7, 60, "Cash", "Paid", null, null, 44m, 12, new DateTime(2026, 1, 28, 15, 0, 0, 0, DateTimeKind.Utc), 119, 23, "Completed" },
                    { 201, new DateTime(2026, 2, 15, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 14, 16, 0, 0, 0, DateTimeKind.Utc), 10, 45, "PayPal", "Paid", null, null, 28m, 12, new DateTime(2026, 2, 16, 16, 0, 0, 0, DateTimeKind.Utc), 120, 24, "Completed" },
                    { 202, new DateTime(2026, 3, 2, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 1, 10, 0, 0, 0, DateTimeKind.Utc), 44, 90, "Cash", "Paid", null, null, 66m, 12, new DateTime(2026, 3, 3, 10, 0, 0, 0, DateTimeKind.Utc), 121, 23, "Completed" },
                    { 203, new DateTime(2026, 3, 19, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 18, 11, 0, 0, 0, DateTimeKind.Utc), 47, 60, "PayPal", "Paid", null, null, 44m, 12, new DateTime(2026, 3, 20, 11, 0, 0, 0, DateTimeKind.Utc), 119, 24, "Completed" },
                    { 204, new DateTime(2026, 4, 5, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 4, 12, 0, 0, 0, DateTimeKind.Utc), 8, 45, "Cash", "Paid", null, null, 28m, 12, new DateTime(2026, 4, 6, 12, 0, 0, 0, DateTimeKind.Utc), 120, 23, "Completed" },
                    { 205, new DateTime(2026, 4, 22, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 21, 13, 0, 0, 0, DateTimeKind.Utc), 20, 90, "PayPal", "Paid", null, null, 66m, 12, new DateTime(2026, 4, 23, 13, 0, 0, 0, DateTimeKind.Utc), 121, 24, "Completed" },
                    { 206, new DateTime(2026, 5, 10, 14, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 9, 14, 0, 0, 0, DateTimeKind.Utc), 45, 60, "Cash", "Paid", null, null, 44m, 12, new DateTime(2026, 5, 11, 14, 0, 0, 0, DateTimeKind.Utc), 119, 23, "Completed" },
                    { 207, new DateTime(2026, 5, 26, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 25, 15, 0, 0, 0, DateTimeKind.Utc), 6, 45, "PayPal", "Paid", null, null, 28m, 12, new DateTime(2026, 5, 27, 15, 0, 0, 0, DateTimeKind.Utc), 120, 24, "Completed" },
                    { 208, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 13, 16, 0, 0, 0, DateTimeKind.Utc), 9, 90, "Cash", "Unpaid", null, null, 66m, 12, new DateTime(2026, 6, 15, 16, 0, 0, 0, DateTimeKind.Utc), 121, 23, "Cancelled" },
                    { 209, new DateTime(2026, 1, 29, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 1, 28, 16, 0, 0, 0, DateTimeKind.Utc), 8, 45, "Cash", "Paid", null, null, 24m, 13, new DateTime(2026, 1, 30, 16, 0, 0, 0, DateTimeKind.Utc), 122, 25, "Completed" },
                    { 210, new DateTime(2026, 2, 15, 10, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 2, 14, 10, 0, 0, 0, DateTimeKind.Utc), 20, 60, "PayPal", "Paid", null, null, 33m, 13, new DateTime(2026, 2, 16, 10, 0, 0, 0, DateTimeKind.Utc), 123, 26, "Completed" },
                    { 211, new DateTime(2026, 3, 4, 11, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 3, 11, 0, 0, 0, DateTimeKind.Utc), 45, 75, "Cash", "Paid", null, null, 43m, 13, new DateTime(2026, 3, 5, 11, 0, 0, 0, DateTimeKind.Utc), 124, 25, "Completed" },
                    { 212, new DateTime(2026, 3, 22, 12, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 3, 21, 12, 0, 0, 0, DateTimeKind.Utc), 6, 45, "PayPal", "Paid", null, null, 24m, 13, new DateTime(2026, 3, 23, 12, 0, 0, 0, DateTimeKind.Utc), 122, 26, "Completed" },
                    { 213, new DateTime(2026, 4, 7, 13, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 6, 13, 0, 0, 0, DateTimeKind.Utc), 9, 60, "Cash", "Paid", null, null, 33m, 13, new DateTime(2026, 4, 8, 13, 0, 0, 0, DateTimeKind.Utc), 123, 25, "Completed" },
                    { 214, new DateTime(2026, 4, 26, 14, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 4, 25, 14, 0, 0, 0, DateTimeKind.Utc), 43, 75, "PayPal", "Paid", null, null, 43m, 13, new DateTime(2026, 4, 27, 14, 0, 0, 0, DateTimeKind.Utc), 124, 26, "Completed" },
                    { 215, new DateTime(2026, 5, 11, 15, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 10, 15, 0, 0, 0, DateTimeKind.Utc), 46, 45, "Cash", "Paid", null, null, 24m, 13, new DateTime(2026, 5, 12, 15, 0, 0, 0, DateTimeKind.Utc), 122, 25, "Completed" },
                    { 216, new DateTime(2026, 5, 28, 16, 0, 0, 0, DateTimeKind.Utc), 3, null, new DateTime(2026, 5, 27, 16, 0, 0, 0, DateTimeKind.Utc), 7, 60, "PayPal", "Paid", null, null, 33m, 13, new DateTime(2026, 5, 29, 16, 0, 0, 0, DateTimeKind.Utc), 123, 26, "Completed" },
                    { 217, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 13, 10, 0, 0, 0, DateTimeKind.Utc), 10, 75, "Cash", "Unpaid", null, null, 43m, 13, new DateTime(2026, 6, 15, 10, 0, 0, 0, DateTimeKind.Utc), 124, 25, "Cancelled" },
                    { 218, new DateTime(2026, 2, 1, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Utc), 9, 60, "Cash", "Paid", null, null, 60m, 14, new DateTime(2026, 2, 2, 10, 0, 0, 0, DateTimeKind.Utc), 125, 27, "Completed" },
                    { 219, new DateTime(2026, 2, 17, 11, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 2, 16, 11, 0, 0, 0, DateTimeKind.Utc), 43, 75, "PayPal", "Paid", null, null, 90m, 14, new DateTime(2026, 2, 18, 11, 0, 0, 0, DateTimeKind.Utc), 126, 28, "Completed" },
                    { 220, new DateTime(2026, 3, 8, 12, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 7, 12, 0, 0, 0, DateTimeKind.Utc), 46, 60, "Cash", "Paid", null, null, 60m, 14, new DateTime(2026, 3, 9, 12, 0, 0, 0, DateTimeKind.Utc), 127, 27, "Completed" },
                    { 221, new DateTime(2026, 3, 23, 13, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 3, 22, 13, 0, 0, 0, DateTimeKind.Utc), 7, 60, "PayPal", "Paid", null, null, 60m, 14, new DateTime(2026, 3, 24, 13, 0, 0, 0, DateTimeKind.Utc), 125, 28, "Completed" },
                    { 222, new DateTime(2026, 4, 9, 14, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 8, 14, 0, 0, 0, DateTimeKind.Utc), 10, 75, "Cash", "Paid", null, null, 90m, 14, new DateTime(2026, 4, 10, 14, 0, 0, 0, DateTimeKind.Utc), 126, 27, "Completed" },
                    { 223, new DateTime(2026, 4, 26, 15, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 4, 25, 15, 0, 0, 0, DateTimeKind.Utc), 44, 60, "PayPal", "Paid", null, null, 60m, 14, new DateTime(2026, 4, 27, 15, 0, 0, 0, DateTimeKind.Utc), 127, 28, "Completed" },
                    { 224, new DateTime(2026, 5, 13, 16, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 12, 16, 0, 0, 0, DateTimeKind.Utc), 47, 60, "Cash", "Paid", null, null, 60m, 14, new DateTime(2026, 5, 14, 16, 0, 0, 0, DateTimeKind.Utc), 125, 27, "Completed" },
                    { 225, new DateTime(2026, 5, 31, 10, 0, 0, 0, DateTimeKind.Utc), 4, null, new DateTime(2026, 5, 30, 10, 0, 0, 0, DateTimeKind.Utc), 8, 75, "PayPal", "Paid", null, null, 90m, 14, new DateTime(2026, 6, 1, 10, 0, 0, 0, DateTimeKind.Utc), 126, 28, "Completed" },
                    { 226, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 15, 11, 0, 0, 0, DateTimeKind.Utc), 20, 60, "Cash", "Unpaid", null, null, 60m, 14, new DateTime(2026, 6, 17, 11, 0, 0, 0, DateTimeKind.Utc), 127, 27, "Cancelled" },
                    { 227, new DateTime(2026, 2, 2, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 1, 11, 0, 0, 0, DateTimeKind.Utc), 10, 60, "Cash", "Paid", null, null, 65m, 15, new DateTime(2026, 2, 3, 11, 0, 0, 0, DateTimeKind.Utc), 128, 29, "Completed" },
                    { 228, new DateTime(2026, 2, 19, 12, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 2, 18, 12, 0, 0, 0, DateTimeKind.Utc), 44, 45, "PayPal", "Paid", null, null, 58m, 15, new DateTime(2026, 2, 20, 12, 0, 0, 0, DateTimeKind.Utc), 129, 30, "Completed" },
                    { 229, new DateTime(2026, 3, 8, 13, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 7, 13, 0, 0, 0, DateTimeKind.Utc), 47, 45, "Cash", "Paid", null, null, 32m, 15, new DateTime(2026, 3, 9, 13, 0, 0, 0, DateTimeKind.Utc), 130, 29, "Completed" },
                    { 230, new DateTime(2026, 3, 25, 14, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 3, 24, 14, 0, 0, 0, DateTimeKind.Utc), 8, 60, "PayPal", "Paid", null, null, 65m, 15, new DateTime(2026, 3, 26, 14, 0, 0, 0, DateTimeKind.Utc), 128, 30, "Completed" },
                    { 231, new DateTime(2026, 4, 12, 15, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 11, 15, 0, 0, 0, DateTimeKind.Utc), 20, 45, "Cash", "Paid", null, null, 58m, 15, new DateTime(2026, 4, 13, 15, 0, 0, 0, DateTimeKind.Utc), 129, 29, "Completed" },
                    { 232, new DateTime(2026, 4, 28, 16, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 4, 27, 16, 0, 0, 0, DateTimeKind.Utc), 45, 45, "PayPal", "Paid", null, null, 32m, 15, new DateTime(2026, 4, 29, 16, 0, 0, 0, DateTimeKind.Utc), 130, 30, "Completed" },
                    { 233, new DateTime(2026, 5, 17, 10, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 16, 10, 0, 0, 0, DateTimeKind.Utc), 6, 60, "Cash", "Paid", null, null, 65m, 15, new DateTime(2026, 5, 18, 10, 0, 0, 0, DateTimeKind.Utc), 128, 29, "Completed" },
                    { 234, new DateTime(2026, 6, 1, 11, 0, 0, 0, DateTimeKind.Utc), 5, null, new DateTime(2026, 5, 31, 11, 0, 0, 0, DateTimeKind.Utc), 9, 45, "PayPal", "Paid", null, null, 58m, 15, new DateTime(2026, 6, 2, 11, 0, 0, 0, DateTimeKind.Utc), 129, 30, "Completed" },
                    { 235, null, null, "Customer requested cancellation.", new DateTime(2026, 6, 17, 12, 0, 0, 0, DateTimeKind.Utc), 43, 45, "Cash", "Unpaid", null, null, 32m, 15, new DateTime(2026, 6, 19, 12, 0, 0, 0, DateTimeKind.Utc), 130, 29, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "CustomerId", "IsRemoved", "Rating", "RemovedById", "SalonId", "SalonReply" },
                values: new object[,]
                {
                    { 13, 104, "Best visit I have had in a long time, will be back.", new DateTime(2026, 2, 26, 16, 0, 0, 0, DateTimeKind.Utc), 47, false, 5, null, 1, null },
                    { 15, 107, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 20, 12, 0, 0, 0, DateTimeKind.Utc), 45, false, 5, null, 1, null },
                    { 18, 112, "Best visit I have had in a long time, will be back.", new DateTime(2026, 2, 11, 16, 0, 0, 0, DateTimeKind.Utc), 45, false, 5, null, 2, null },
                    { 20, 115, "Best visit I have had in a long time, will be back.", new DateTime(2026, 4, 3, 12, 0, 0, 0, DateTimeKind.Utc), 43, false, 4, null, 2, null },
                    { 21, 116, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 4, 20, 13, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 2, null },
                    { 22, 120, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 1, 27, 16, 0, 0, 0, DateTimeKind.Utc), 43, false, 5, null, 3, null },
                    { 23, 121, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 2, 13, 17, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 3, null },
                    { 25, 124, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 6, 13, 0, 0, 0, DateTimeKind.Utc), 44, false, 5, null, 3, null }
                });

            migrationBuilder.InsertData(
                table: "StaffServices",
                columns: new[] { "ServiceId", "StaffId" },
                values: new object[,]
                {
                    { 10, 9 },
                    { 11, 9 },
                    { 12, 9 },
                    { 13, 10 },
                    { 14, 10 },
                    { 15, 10 },
                    { 101, 11 },
                    { 102, 11 },
                    { 103, 11 },
                    { 101, 12 },
                    { 102, 12 },
                    { 103, 12 },
                    { 104, 13 },
                    { 105, 13 },
                    { 106, 13 },
                    { 104, 14 },
                    { 105, 14 },
                    { 106, 14 },
                    { 107, 15 },
                    { 108, 15 },
                    { 109, 15 },
                    { 107, 16 },
                    { 108, 16 },
                    { 109, 16 },
                    { 110, 17 },
                    { 111, 17 },
                    { 112, 17 },
                    { 110, 18 },
                    { 111, 18 },
                    { 112, 18 },
                    { 113, 19 },
                    { 114, 19 },
                    { 115, 19 },
                    { 113, 20 },
                    { 114, 20 },
                    { 115, 20 },
                    { 116, 21 },
                    { 117, 21 },
                    { 118, 21 },
                    { 116, 22 },
                    { 117, 22 },
                    { 118, 22 },
                    { 119, 23 },
                    { 120, 23 },
                    { 121, 23 },
                    { 119, 24 },
                    { 120, 24 },
                    { 121, 24 },
                    { 122, 25 },
                    { 123, 25 },
                    { 124, 25 },
                    { 122, 26 },
                    { 123, 26 },
                    { 124, 26 },
                    { 125, 27 },
                    { 126, 27 },
                    { 127, 27 },
                    { 125, 28 },
                    { 126, 28 },
                    { 127, 28 },
                    { 128, 29 },
                    { 129, 29 },
                    { 130, 29 },
                    { 128, 30 },
                    { 129, 30 },
                    { 130, 30 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "AppointmentId", "Comment", "CreatedAt", "CustomerId", "IsRemoved", "Rating", "RemovedById", "SalonId", "SalonReply" },
                values: new object[,]
                {
                    { 28, 129, "Best visit I have had in a long time, will be back.", new DateTime(2026, 1, 29, 17, 0, 0, 0, DateTimeKind.Utc), 44, false, 5, null, 4, null },
                    { 29, 131, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 3, 4, 12, 0, 0, 0, DateTimeKind.Utc), 8, false, 5, null, 4, null },
                    { 32, 135, "Nice place, service was fine but nothing special.", new DateTime(2026, 5, 11, 16, 0, 0, 0, DateTimeKind.Utc), 9, false, 3, null, 4, null },
                    { 35, 140, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 3, 6, 13, 0, 0, 0, DateTimeKind.Utc), 9, false, 5, null, 5, null },
                    { 36, 142, "Best visit I have had in a long time, will be back.", new DateTime(2026, 4, 9, 15, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 5, null },
                    { 38, 147, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 2, 2, 12, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 6, null },
                    { 39, 148, "Good result overall, though I had to wait a bit.", new DateTime(2026, 2, 19, 13, 0, 0, 0, DateTimeKind.Utc), 7, false, 3, null, 6, null },
                    { 40, 150, "Great attention to detail and fair prices.", new DateTime(2026, 3, 25, 15, 0, 0, 0, DateTimeKind.Utc), 44, false, 4, null, 6, null },
                    { 41, 151, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 4, 13, 16, 0, 0, 0, DateTimeKind.Utc), 47, false, 4, null, 6, null },
                    { 42, 153, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 5, 15, 18, 0, 0, 0, DateTimeKind.Utc), 20, false, 4, null, 6, null },
                    { 43, 155, "On time, friendly and the result looks fantastic.", new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc), 44, false, 4, null, 7, null },
                    { 44, 156, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 2, 4, 13, 0, 0, 0, DateTimeKind.Utc), 47, false, 5, null, 7, null },
                    { 45, 158, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 3, 10, 15, 0, 0, 0, DateTimeKind.Utc), 20, false, 5, null, 7, null },
                    { 46, 159, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 3, 27, 16, 0, 0, 0, DateTimeKind.Utc), 45, false, 5, null, 7, null },
                    { 47, 161, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 30, 18, 0, 0, 0, DateTimeKind.Utc), 9, false, 5, null, 7, null },
                    { 48, 162, "Great attention to detail and fair prices.", new DateTime(2026, 5, 18, 12, 0, 0, 0, DateTimeKind.Utc), 43, false, 4, null, 7, null },
                    { 49, 164, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 1, 20, 13, 0, 0, 0, DateTimeKind.Utc), 45, false, 4, null, 8, null },
                    { 50, 166, "Great attention to detail and fair prices.", new DateTime(2026, 2, 23, 15, 0, 0, 0, DateTimeKind.Utc), 9, false, 5, null, 8, null },
                    { 51, 167, "On time, friendly and the result looks fantastic.", new DateTime(2026, 3, 12, 16, 0, 0, 0, DateTimeKind.Utc), 43, false, 4, null, 8, null },
                    { 52, 169, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 15, 18, 0, 0, 0, DateTimeKind.Utc), 7, false, 5, null, 8, null },
                    { 53, 170, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 5, 4, 12, 0, 0, 0, DateTimeKind.Utc), 10, false, 5, null, 8, null },
                    { 54, 174, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 2, 9, 15, 0, 0, 0, DateTimeKind.Utc), 7, false, 4, null, 9, null },
                    { 55, 175, "Great attention to detail and fair prices.", new DateTime(2026, 2, 25, 16, 0, 0, 0, DateTimeKind.Utc), 10, false, 5, null, 9, null },
                    { 56, 177, "Nice place, service was fine but nothing special.", new DateTime(2026, 3, 31, 18, 0, 0, 0, DateTimeKind.Utc), 47, false, 3, null, 9, null },
                    { 57, 178, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 17, 12, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, null, 9, null },
                    { 58, 180, "Nice place, service was fine but nothing special.", new DateTime(2026, 5, 21, 14, 0, 0, 0, DateTimeKind.Utc), 45, false, 3, null, 9, null },
                    { 59, 182, "On time, friendly and the result looks fantastic.", new DateTime(2026, 1, 26, 15, 0, 0, 0, DateTimeKind.Utc), 47, false, 4, null, 10, null },
                    { 60, 183, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 2, 10, 16, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, null, 10, null },
                    { 61, 185, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 3, 16, 18, 0, 0, 0, DateTimeKind.Utc), 45, false, 4, null, 10, null },
                    { 62, 186, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 4, 2, 12, 0, 0, 0, DateTimeKind.Utc), 6, false, 4, null, 10, null },
                    { 63, 188, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 5, 6, 14, 0, 0, 0, DateTimeKind.Utc), 43, false, 4, null, 10, null },
                    { 64, 189, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 5, 25, 15, 0, 0, 0, DateTimeKind.Utc), 46, false, 5, null, 10, null },
                    { 65, 191, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 1, 26, 16, 0, 0, 0, DateTimeKind.Utc), 6, false, 5, null, 11, null },
                    { 66, 193, "Decent value. I would try it again.", new DateTime(2026, 3, 2, 18, 0, 0, 0, DateTimeKind.Utc), 43, false, 3, null, 11, null },
                    { 67, 194, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 3, 18, 12, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 11, null },
                    { 68, 196, "Great attention to detail and fair prices.", new DateTime(2026, 4, 21, 14, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, null, 11, null },
                    { 69, 197, "Great attention to detail and fair prices.", new DateTime(2026, 5, 8, 15, 0, 0, 0, DateTimeKind.Utc), 44, false, 4, null, 11, null },
                    { 70, 201, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 2, 16, 18, 0, 0, 0, DateTimeKind.Utc), 10, false, 4, null, 12, null },
                    { 71, 202, "Best visit I have had in a long time, will be back.", new DateTime(2026, 3, 3, 12, 0, 0, 0, DateTimeKind.Utc), 44, false, 4, null, 12, null },
                    { 72, 204, "Best visit I have had in a long time, will be back.", new DateTime(2026, 4, 6, 14, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, null, 12, null },
                    { 73, 205, "Nice place, service was fine but nothing special.", new DateTime(2026, 4, 23, 15, 0, 0, 0, DateTimeKind.Utc), 20, false, 3, null, 12, null },
                    { 74, 207, "Decent value. I would try it again.", new DateTime(2026, 5, 27, 17, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, null, 12, null },
                    { 75, 209, "On time, friendly and the result looks fantastic.", new DateTime(2026, 1, 30, 18, 0, 0, 0, DateTimeKind.Utc), 8, false, 4, null, 13, null },
                    { 76, 210, "Good result overall, though I had to wait a bit.", new DateTime(2026, 2, 16, 12, 0, 0, 0, DateTimeKind.Utc), 20, false, 3, null, 13, null },
                    { 77, 212, "Nice place, service was fine but nothing special.", new DateTime(2026, 3, 23, 14, 0, 0, 0, DateTimeKind.Utc), 6, false, 3, null, 13, null },
                    { 78, 213, "Nice place, service was fine but nothing special.", new DateTime(2026, 4, 8, 15, 0, 0, 0, DateTimeKind.Utc), 9, false, 3, null, 13, null },
                    { 79, 215, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 5, 12, 17, 0, 0, 0, DateTimeKind.Utc), 46, false, 5, null, 13, null },
                    { 80, 216, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 5, 29, 18, 0, 0, 0, DateTimeKind.Utc), 7, false, 4, null, 13, null },
                    { 81, 218, "On time, friendly and the result looks fantastic.", new DateTime(2026, 2, 2, 12, 0, 0, 0, DateTimeKind.Utc), 9, false, 4, null, 14, null },
                    { 82, 220, "Clean, relaxed and very professional. Booking was easy too.", new DateTime(2026, 3, 9, 14, 0, 0, 0, DateTimeKind.Utc), 46, false, 4, null, 14, null },
                    { 83, 221, "Best visit I have had in a long time, will be back.", new DateTime(2026, 3, 24, 15, 0, 0, 0, DateTimeKind.Utc), 7, false, 5, null, 14, null },
                    { 84, 223, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 27, 17, 0, 0, 0, DateTimeKind.Utc), 44, false, 5, null, 14, null },
                    { 85, 224, "Great attention to detail and fair prices.", new DateTime(2026, 5, 14, 18, 0, 0, 0, DateTimeKind.Utc), 47, false, 5, null, 14, null },
                    { 86, 228, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 2, 20, 14, 0, 0, 0, DateTimeKind.Utc), 44, false, 4, null, 15, null },
                    { 87, 229, "On time, friendly and the result looks fantastic.", new DateTime(2026, 3, 9, 15, 0, 0, 0, DateTimeKind.Utc), 47, false, 4, null, 15, null },
                    { 88, 231, "Exactly what I asked for, and the team was lovely.", new DateTime(2026, 4, 13, 17, 0, 0, 0, DateTimeKind.Utc), 20, false, 5, null, 15, null },
                    { 89, 232, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 4, 29, 18, 0, 0, 0, DateTimeKind.Utc), 45, false, 5, null, 15, null },
                    { 90, 234, "Left happier than I arrived. Highly recommended!", new DateTime(2026, 6, 2, 13, 0, 0, 0, DateTimeKind.Utc), 9, false, 4, null, 15, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 171);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 172);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 173);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 176);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 179);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 184);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 187);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 190);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 195);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 198);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 199);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 200);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 203);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 214);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 217);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 219);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 222);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 225);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 226);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 227);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 230);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 133);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 136);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 138);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 149);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 152);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 154);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 157);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 163);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 165);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 168);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 10, 9 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 11, 9 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 12, 9 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 13, 10 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 14, 10 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 15, 10 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 101, 11 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 102, 11 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 103, 11 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 101, 12 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 102, 12 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 103, 12 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 104, 13 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 105, 13 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 106, 13 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 104, 14 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 105, 14 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 106, 14 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 107, 15 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 108, 15 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 109, 15 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 107, 16 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 108, 16 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 109, 16 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 110, 17 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 111, 17 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 112, 17 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 110, 18 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 111, 18 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 112, 18 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 113, 19 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 114, 19 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 115, 19 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 113, 20 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 114, 20 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 115, 20 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 116, 21 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 117, 21 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 118, 21 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 116, 22 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 117, 22 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 118, 22 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 119, 23 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 120, 23 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 121, 23 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 119, 24 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 120, 24 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 121, 24 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 122, 25 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 123, 25 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 124, 25 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 122, 26 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 123, 26 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 124, 26 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 125, 27 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 126, 27 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 127, 27 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 125, 28 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 126, 28 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 127, 28 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 128, 29 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 129, 29 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 130, 29 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 128, 30 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 129, 30 });

            migrationBuilder.DeleteData(
                table: "StaffServices",
                keyColumns: new[] { "ServiceId", "StaffId" },
                keyValues: new object[] { 130, 30 });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 131);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 132);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 134);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 135);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 137);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 139);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 140);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 150);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 151);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 153);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 155);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 156);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 158);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 159);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 164);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 166);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 167);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 169);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 170);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 174);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 175);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 177);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 178);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 180);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 185);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 186);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 188);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 189);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 193);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 194);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 196);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 197);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 204);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 212);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 213);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 215);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 216);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 218);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 220);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 221);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 223);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 224);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 228);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 229);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 126);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 128);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 127);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 129);

            migrationBuilder.DeleteData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 130);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ApprovedAt", "ApprovedById", "PaymentStatus", "StateName" },
                values: new object[] { null, null, "Unpaid", "Pending" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApprovedAt", "ApprovedById", "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { null, null, 45, "Unpaid", 45m, "Pending" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ApprovedAt", "ApprovedById", "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { null, null, 45, "Unpaid", 65m, "Pending" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ApprovedAt", "ApprovedById", "PaymentStatus", "Price", "StateName" },
                values: new object[] { null, null, "Unpaid", 70m, "Pending" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "PaymentStatus", "Price", "StateName" },
                values: new object[] { "Unpaid", 90m, "Confirmed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { 45, "Unpaid", 35m, "Confirmed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DurationMinutes", "PaymentStatus", "Price", "StateName" },
                values: new object[] { 45, "Unpaid", 40m, "Confirmed" });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 60m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 80m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 85m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 30m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 50m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 55m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 75m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 15,
                column: "Price",
                value: 95m);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 45m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 65m });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 19,
                column: "Price",
                value: 70m);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 20,
                column: "Price",
                value: 90m);

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 2,
                column: "Caption",
                value: "Interior");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 4,
                column: "Caption",
                value: "Interior");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 6,
                column: "Caption",
                value: "Interior");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 8,
                column: "Caption",
                value: "Interior");

            migrationBuilder.UpdateData(
                table: "SalonGalleries",
                keyColumn: "Id",
                keyValue: 10,
                column: "Caption",
                value: "Interior");

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 30m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 3,
                column: "Price",
                value: 35m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 40m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 45m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 6,
                column: "Price",
                value: 50m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 55m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 8,
                column: "Price",
                value: 60m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 9,
                column: "Price",
                value: 65m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 10,
                column: "Price",
                value: 70m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 11,
                column: "Price",
                value: 75m);

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 80m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 45, 85m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 60, 90m });

            migrationBuilder.UpdateData(
                table: "SalonServices",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "DurationMinutes", "Price" },
                values: new object[] { 30, 95m });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), false, new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 23,
                column: "CloseTime",
                value: new TimeOnly(20, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 24,
                column: "CloseTime",
                value: new TimeOnly(20, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 25,
                column: "CloseTime",
                value: new TimeOnly(20, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 26,
                column: "CloseTime",
                value: new TimeOnly(20, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 27,
                column: "CloseTime",
                value: new TimeOnly(20, 0, 0));

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "CloseTime", "OpenTime" },
                values: new object[] { new TimeOnly(20, 0, 0), new TimeOnly(9, 0, 0) });

            migrationBuilder.UpdateData(
                table: "SalonWorkingHours",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "CloseTime", "IsClosed", "OpenTime" },
                values: new object[] { null, true, null });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Main street 1", 4.2m, "Bellissima Hair Studio - professional service since 2015.", "contact@salon1.cutcal.com", "+3876210000" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Main street 2", 4.3m, "Gentleman's Cut Barbershop - professional service since 2015.", "contact@salon2.cutcal.com", "+3876210001" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Main street 3", 4.4m, "Glow Beauty Studio - professional service since 2015.", "contact@salon3.cutcal.com", "+3876210002" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Main street 4", 4.5m, "Perfect Nails Studio - professional service since 2015.", "contact@salon4.cutcal.com", "+3876210003" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Address", "AvgRating", "Description", "Email", "Phone" },
                values: new object[] { "Main street 5", 4.6m, "Relax & Spa - professional service since 2015.", "contact@salon5.cutcal.com", "+3876210004" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 1,
                column: "Bio",
                value: "Experienced professional.");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 2,
                column: "Bio",
                value: "Experienced professional.");

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });

            migrationBuilder.UpdateData(
                table: "Staff",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Bio", "Role" },
                values: new object[] { "Experienced professional.", "Stylist" });
        }
    }
}
