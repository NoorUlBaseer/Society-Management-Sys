using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocietyMng.Migrations
{
    /// <inheritdoc />
    public partial class FixInitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "Description" },
                values: new object[] { "COMMERCIAL", "Commercial" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "Description" },
                values: new object[] { "LAND", "Land/Plot" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "AVAILABLE", "Available", 1, 4 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BOOKED", "Booked", 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "SOLD", "Sold", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "Description" },
                values: new object[] { "VILLA", "Villa" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "Description" },
                values: new object[] { "COMMERCIAL", "Commercial" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "LAND", "Land/Plot", 2, 3 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "AVAILABLE", "Available", 1 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BOOKED", "Booked", 2 });

            migrationBuilder.InsertData(
                table: "SystemCodeItems",
                columns: new[] { "Id", "Code", "Description", "IsActive", "SortOrder", "SystemCodeId" },
                values: new object[] { 14, "SOLD", "Sold", true, 3, 4 });
        }
    }
}
