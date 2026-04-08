using Microsoft.EntityFrameworkCore;
using VentasService.Models;

namespace VentasService.Data
{
    public class VentasContext : DbContext
    {
        public VentasContext(DbContextOptions<VentasContext> options) : base(options)
        {
        }

        // 🔹 Tablas ya existentes
        public DbSet<Venta> Ventas { get; set; }

        // 🔹 Nueva tabla DetalleVenta
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔹 Configuración de DetalleVenta
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("DetalleVenta");
                entity.HasKey(d => d.DetalleID);

                // Relación con Venta (1 Venta → muchos Detalles)
                entity.HasOne(d => d.Venta)
                      .WithMany(v => v.Detalles)
                      .HasForeignKey(d => d.VentaID)
                      .OnDelete(DeleteBehavior.Cascade);

                // ProductoID es referencia a otro servicio (sin FK local)
                entity.Property(d => d.ProductoID).IsRequired();

                // Subtotal calculado como decimal(10,2)
                entity.Property(d => d.Subtotal)
                      .HasColumnType("decimal(10,2)");
            });
        }
    }
}
