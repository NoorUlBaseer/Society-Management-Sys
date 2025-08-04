using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SocietyMng.Migrations
{
    /// <inheritdoc />
    public partial class seedingdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemCodeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemCodeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemCodeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemCodeItems_SystemCodes_SystemCodeId",
                        column: x => x.SystemCodeId,
                        principalTable: "SystemCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_SystemCodeItems_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BlockId = table.Column<int>(type: "int", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_SystemCodeItems_BlockId",
                        column: x => x.BlockId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_SystemCodeItems_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_SystemCodeItems_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Complaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AssetId = table.Column<int>(type: "int", nullable: true),
                    BookingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Complaints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "SystemCodes",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "User_Role", "User roles in the system" },
                    { 2, "Block", "Society blocks" },
                    { 3, "Property_Type", "Property types" },
                    { 4, "Asset_Status", "Asset statuses" }
                });

            migrationBuilder.InsertData(
                table: "SystemCodeItems",
                columns: new[] { "Id", "Code", "Description", "IsActive", "SortOrder", "SystemCodeId" },
                values: new object[,]
                {
                    { 1, "Admin", "System Administrator", true, 1, 1 },
                    { 2, "Buyer", "Property Buyer", true, 2, 1 },
                    { 3, "Sales", "Society Sales", true, 3, 1 },
                    { 4, "BLOCK_A", "Block A", true, 1, 2 },
                    { 5, "BLOCK_B", "Block B", true, 2, 2 },
                    { 6, "BLOCK_C", "Block C", true, 2, 2 },
                    { 7, "BLOCK_D", "Block D", true, 2, 2 },
                    { 8, "APARTMENT", "Apartment", true, 1, 3 },
                    { 9, "COMMERCIAL", "Commercial", true, 2, 3 },
                    { 10, "LAND", "Land/Plot", true, 2, 3 },
                    { 11, "AVAILABLE", "Available", true, 1, 4 },
                    { 12, "BOOKED", "Booked", true, 2, 4 },
                    { 13, "SOLD", "Sold", true, 3, 4 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Gender", "IsActive", "PasswordHash", "PhoneNumber", "RoleId" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "mishal@society.com", "Mishal Ali", "Female", true, "$2a$08$1m.DC2ZBSdrDYHzW/QiGJexNx9U7TlAuBaBsav6..pGLkh7zJT4Ym", "1122334455", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_BlockId",
                table: "Assets",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_PropertyTypeId",
                table: "Assets",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_StatusId",
                table: "Assets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserId",
                table: "Assets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_AssetId",
                table: "Bookings",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_AssetId",
                table: "Complaints",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_BookingId",
                table: "Complaints",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_UserId",
                table: "Complaints",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemCodeItems_SystemCodeId",
                table: "SystemCodeItems",
                column: "SystemCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SystemCodeItems");

            migrationBuilder.DropTable(
                name: "SystemCodes");
        }
    }
}
