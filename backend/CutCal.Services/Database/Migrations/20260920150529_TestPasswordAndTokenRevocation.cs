using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class TestPasswordAndTokenRevocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RevokedTokens",
                columns: table => new
                {
                    Jti = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevokedTokens", x => x.Jti);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 16,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 17,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18,
                column: "PasswordHash",
                value: "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Phone", "ProfileImageUrl", "Username" },
                values: new object[,]
                {
                    { 19, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "desktop@cutcal.com", "Desktop", true, "Admin", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761000019", "/images/avatars/19.jpg", "desktop" },
                    { 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mobile@cutcal.com", "Mobile", true, "Customer", "$2a$12$ut/rfr3dCMbxCMFkrTvqx.ZrocbPowx8.j7Db82QinlgaSQsF1yBi", "+38761000020", "/images/avatars/20.jpg", "mobile" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "RoleId", "UserId" },
                values: new object[,]
                {
                    { 19, 4, 19 },
                    { 20, 1, 20 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RevokedTokens_ExpiresAt",
                table: "RevokedTokens",
                column: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RevokedTokens");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 11,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 12,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 13,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 14,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 15,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 16,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 17,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 18,
                column: "PasswordHash",
                value: "$2a$12$enAQ2S4KvMzecXNse01PGOPLjEntlavBY44cH68GLTAYzmO3Q1lr.");
        }
    }
}
