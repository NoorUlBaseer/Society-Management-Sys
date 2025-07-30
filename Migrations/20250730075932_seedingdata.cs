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
                name: "Staff",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BankAccount = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.Id);
                });

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
                    RoomCountId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_SystemCodeItems_RoomCountId",
                        column: x => x.RoomCountId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_SystemCodeItems_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SystemCodeItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
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
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Complaints_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Staff",
                columns: new[] { "Id", "BankAccount", "ContactNumber", "Email", "FullName", "HireDate", "IsActive", "Position", "Salary" },
                values: new object[,]
                {
                    { 1, "US1234567890123456", "555-0101", "john.smith@example.com", "John Smith", new DateTime(2018, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Manager", 65000.00m },
                    { 2, "US2345678901234567", "555-0102", "emily.j@example.com", "Emily Johnson", new DateTime(2019, 3, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Accountant", 55000.00m },
                    { 3, "US3456789012345678", "555-0103", "michael.w@example.com", "Michael Williams", new DateTime(2020, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Maintenance Supervisor", 48000.00m },
                    { 4, "US4567890123456789", "555-0104", "sarah.b@example.com", "Sarah Brown", new DateTime(2021, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Administrative Assistant", 42000.00m },
                    { 5, "US5678901234567890", "555-0105", "robert.d@example.com", "Robert Davis", new DateTime(2017, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Security Officer", 38000.00m },
                    { 6, "US6789012345678901", "555-0106", "jennifer.m@example.com", "Jennifer Miller", new DateTime(2022, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Community Manager", 58000.00m },
                    { 7, "US7890123456789012", "555-0107", "david.w@example.com", "David Wilson", new DateTime(2020, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Maintenance Technician", 45000.00m },
                    { 8, "US8901234567890123", "555-0108", "lisa.t@example.com", "Lisa Taylor", new DateTime(2019, 6, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Accountant", 53000.00m }
                });

            migrationBuilder.InsertData(
                table: "SystemCodes",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[,]
                {
                    { 1, "User_Role", "User roles in the system" },
                    { 2, "Room_No", "Property room counts" },
                    { 3, "Asset_Status", "Rental/Sale statuses" }
                });

            migrationBuilder.InsertData(
                table: "SystemCodeItems",
                columns: new[] { "Id", "Code", "Description", "IsActive", "SortOrder", "SystemCodeId" },
                values: new object[,]
                {
                    { 1, "Admin", "System Administrator", true, 1, 1 },
                    { 2, "Resident", "Society Resident", true, 2, 1 },
                    { 3, "Buyer", "Property Buyer", true, 3, 1 },
                    { 4, "1_ROOM", "1 Bedroom", true, 1, 2 },
                    { 5, "2_ROOMS", "2 Rooms, 1 Kitchen", true, 2, 2 },
                    { 6, "3_ROOMS", "3 Rooms, 1 Living room & 1 Kitchen", true, 3, 2 },
                    { 7, "Rental_avail", "Available for Rent", true, 1, 3 },
                    { 8, "Rented", "Rented", true, 2, 3 },
                    { 9, "Sale_avail", "Available for Sale", true, 3, 3 },
                    { 10, "Sold", "Sold", true, 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "Gender", "IsActive", "PasswordHash", "PhoneNumber", "RoleId" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@society.com", "Mishal Ali", "Female", true, "$2a$08$1m.DC2ZBSdrDYHzW/QiGJexNx9U7TlAuBaBsav6..pGLkh7zJT4Ym", "1122334455", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_RoomCountId",
                table: "Assets",
                column: "RoomCountId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_StatusId",
                table: "Assets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UploadedByUserId",
                table: "Assets",
                column: "UploadedByUserId");

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
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Complaints");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SystemCodeItems");

            migrationBuilder.DropTable(
                name: "SystemCodes");
        }
    }
}
