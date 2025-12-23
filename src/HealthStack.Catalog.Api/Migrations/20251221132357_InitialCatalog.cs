using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthStack.Catalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("093bec4a-006f-41b6-a594-e4f236563c3e"), "Health and nutrition supplements", "Supplements" },
                    { new Guid("e978ea0e-68f1-4b62-928f-7034cd56ab7e"), "Personal hygiene and sanitation products", "Hygiene" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "CreatedAt", "Description", "IsActive", "Name", "Price", "Sku", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("78b983cb-d782-42ea-9eec-43b402463a56"), "HealthPlus", new Guid("093bec4a-006f-41b6-a594-e4f236563c3e"), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Boosts immunity", true, "Vitamin C 500mg", 12.99m, "HS1001", null },
                    { new Guid("813755d7-3f15-47a7-b479-908e281d69c3"), "NutriLife", new Guid("093bec4a-006f-41b6-a594-e4f236563c3e"), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Supports heart and brain health", true, "Omega-3 Fish Oil", 19.50m, "HS1002", null },
                    { new Guid("d7e749e9-f685-49f9-b855-b0a1684db9db"), "SafeHands", new Guid("e978ea0e-68f1-4b62-928f-7034cd56ab7e"), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kills 99.9% germs", true, "Hand Sanitizer 250ml", 5.75m, "HS1003", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
