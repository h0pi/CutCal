using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CutCal.Services.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentCancellationAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelledById",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 101,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 102,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 103,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 104,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 105,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 106,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 107,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 108,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 109,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 110,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 111,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 112,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 113,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 114,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 115,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 116,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 117,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 118,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 119,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 120,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 121,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 122,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 123,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 124,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 125,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 126,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 127,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 128,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 129,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 130,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 131,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 132,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 133,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 134,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 135,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 136,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 137,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 138,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 139,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 140,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 141,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 142,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 143,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 144,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 145,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 146,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 147,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 148,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 149,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 150,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 151,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 152,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 153,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 154,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 155,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 156,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 157,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 158,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 159,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 160,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 161,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 162,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 163,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 164,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 165,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 166,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 167,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 168,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 169,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 170,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 171,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 172,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 173,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 174,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 175,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 176,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 177,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 178,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 179,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 180,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 181,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 182,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 183,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 184,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 185,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 186,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 187,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 188,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 189,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 190,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 191,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 192,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 193,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 194,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 195,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 196,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 197,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 198,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 199,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 200,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 201,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 202,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 203,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 204,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 205,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 206,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 207,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 208,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 209,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 210,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 211,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 212,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 213,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 214,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 215,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 216,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 217,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 218,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 219,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 220,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 221,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 222,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 223,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 224,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 225,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 226,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 227,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 228,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 229,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 230,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 231,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 232,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 233,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 234,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 235,
                columns: new[] { "CancelledAt", "CancelledById" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CancelledById",
                table: "Appointments",
                column: "CancelledById");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_CancelledById",
                table: "Appointments",
                column: "CancelledById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_CancelledById",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CancelledById",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledById",
                table: "Appointments");
        }
    }
}
