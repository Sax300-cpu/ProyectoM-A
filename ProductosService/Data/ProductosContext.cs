using Microsoft.EntityFrameworkCore;
using ProductosService.Models;

namespace ProductosService.Data
{
    public class ProductosContext : DbContext
    {
        public ProductosContext(DbContextOptions<ProductosContext> options) 
            : base(options) { }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índice único para el nombre del producto (opcional)
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Nombre)
                .IsUnique();

            // Seed data inicial
            modelBuilder.Entity<Producto>().HasData(
                new Producto 
                { 
                    ProductoID = 1, 
                    Nombre = "Laptop HP", 
                    Precio = 850.99m, 
                    Stock = 10,
                    Descripcion = "Laptop HP 15.6 pulgadas, 8GB RAM, 256GB SSD"
                },
                new Producto 
                { 
                    ProductoID = 2, 
                    Nombre = "Mouse Inalámbrico", 
                    Precio = 25.50m, 
                    Stock = 50,
                    Descripcion = "Mouse ergonómico inalámbrico"
                },
                new Producto 
                { 
                    ProductoID = 3, 
                    Nombre = "Teclado Mecánico", 
                    Precio = 75.00m, 
                    Stock = 30,
                    Descripcion = "Teclado mecánico RGB"
                }
            );
        }
    }
}