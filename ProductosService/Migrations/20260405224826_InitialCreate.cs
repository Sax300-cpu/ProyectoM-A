using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductosService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoID);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ProductoID", "Activo", "Descripcion", "FechaCreacion", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, true, "Laptop HP 15.6 pulgadas, 8GB RAM, 256GB SSD", new DateTime(2026, 4, 5, 17, 48, 25, 185, DateTimeKind.Local).AddTicks(3772), "Laptop HP", 850.99m, 10 },
                    { 2, true, "Mouse ergonómico inalámbrico", new DateTime(2026, 4, 5, 17, 48, 25, 188, DateTimeKind.Local).AddTicks(8250), "Mouse Inalámbrico", 25.50m, 50 },
                    { 3, true, "Teclado mecánico RGB", new DateTime(2026, 4, 5, 17, 48, 25, 188, DateTimeKind.Local).AddTicks(8314), "Teclado Mecánico", 75.00m, 30 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Nombre",
                table: "Productos",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
