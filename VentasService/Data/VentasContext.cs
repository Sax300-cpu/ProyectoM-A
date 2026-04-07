using Microsoft.EntityFrameworkCore;
using VentasService.Models;

namespace VentasService.Data
{
    public class VentasContext : DbContext
    {
        public VentasContext(DbContextOptions<VentasContext> options) : base(options) { }

        public DbSet<Venta> Ventas { get; set; }
    }
}
