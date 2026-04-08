using System.ComponentModel.DataAnnotations;

namespace VentasService.Models
{
    public class Venta
    {
        [Key]
        public int VentaID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal Iva { get; set; }

        [Required]
        public decimal Total { get; set; }

        // 🔹 Relación con DetalleVenta (1 Venta → muchos Detalles)
        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}
