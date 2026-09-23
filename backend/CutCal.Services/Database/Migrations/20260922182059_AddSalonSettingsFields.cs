using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonSettingsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Salons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Is24HourFormat",
                table: "Salons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Salons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "Currency", "Is24HourFormat", "TimeZone" },
                values: new object[] { "USD", true, "UTC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "Is24HourFormat",
                table: "Salons");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Salons");
        }
    }
}
