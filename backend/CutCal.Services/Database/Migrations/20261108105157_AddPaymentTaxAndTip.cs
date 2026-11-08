using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTaxAndTip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TipAmount",
                table: "Payments",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TipAmount",
                table: "Payments");
        }
    }
}
