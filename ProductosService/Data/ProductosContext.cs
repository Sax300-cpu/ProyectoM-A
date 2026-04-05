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

            // Configuración específica para el decimal
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.Precio)
                    .HasPrecision(18, 2)  // ← Esto también corrige la advertencia
                    .IsRequired();
                
                entity.HasIndex(p => p.Nombre).IsUnique();
            });

            // Seed data
            modelBuilder.Entity<Producto>().HasData(
                new Producto 
                { 
                    ProductoID = 1, 
                    Nombre = "Laptop HP", 
                    Precio = 850.99m, 
                    Stock = 10,
                    Descripcion = "Laptop HP 15.6 pulgadas"
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