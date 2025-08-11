using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocietyMng.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "BLOCK_A", "Block A", 1, 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BLOCK_B", "Block B", 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Description" },
                values: new object[] { "BLOCK_C", "Block C" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "Description" },
                values: new object[] { "BLOCK_D", "Block D" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "APARTMENT", "Apartment", 1, 3 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "COMMERCIAL", "Commercial", 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Code", "Description" },
                values: new object[] { "LAND", "Land/Plot" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "AVAILABLE", "Available", 1, 4 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BOOKED", "Booked", 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "SOLD", "Sold", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "Sales", "Society Sales", 3, 1 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BLOCK_A", "Block A", 1 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Code", "Description" },
                values: new object[] { "BLOCK_B", "Block B" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Code", "Description" },
                values: new object[] { "BLOCK_C", "Block C" });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "BLOCK_D", "Block D", 2, 2 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "APARTMENT", "Apartment", 1 });

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
                columns: new[] { "Code", "Description", "SortOrder", "SystemCodeId" },
                values: new object[] { "LAND", "Land/Plot", 2, 3 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "AVAILABLE", "Available", 1 });

            migrationBuilder.UpdateData(
                table: "SystemCodeItems",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "Code", "Description", "SortOrder" },
                values: new object[] { "BOOKED", "Booked", 2 });

            migrationBuilder.InsertData(
                table: "SystemCodeItems",
                columns: new[] { "Id", "Code", "Description", "IsActive", "SortOrder", "SystemCodeId" },
                values: new object[] { 13, "SOLD", "Sold", true, 3, 4 });
        }
    }
}
