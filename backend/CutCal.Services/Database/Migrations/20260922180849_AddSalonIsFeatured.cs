using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSalonIsFeatured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Salons",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsFeatured",
                value: false);

            migrationBuilder.UpdateData(
                table: "Salons",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsFeatured",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Salons");
        }
    }
}
