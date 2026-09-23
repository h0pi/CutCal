using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryReferenceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "BiH" },
                    { 2, "HR" },
                    { 3, "RS" }
                });

            // Any city whose existing free-text Country value isn't one of the three seeded
            // above (e.g. one an admin typed in before this migration ran) gets its own new
            // Country row here, so dropping the old column below never loses real data.
            migrationBuilder.Sql(@"
                INSERT INTO Countries (Name)
                SELECT DISTINCT Country FROM Cities WHERE Country NOT IN (SELECT Name FROM Countries);
            ");

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Cities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE ci
                SET ci.CountryId = co.Id
                FROM Cities ci
                JOIN Countries co ON co.Name = ci.Country;
            ");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Cities");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Countries_CountryId",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_CountryId",
                table: "Cities");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ci
                SET ci.Country = co.Name
                FROM Cities ci
                JOIN Countries co ON co.Id = ci.CountryId;
            ");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
