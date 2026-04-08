using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentasService.Models
{
    [Table("DetalleVenta")] // 🔹 nombre exacto de la tabla en SQL
    public class DetalleVenta
    {
        [Key]
        public int DetalleID { get; set; }

        [Required]
        public int VentaID { get; set; }

        [Required]
        public int ProductoID { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        // 🔹 Relación con Venta (si la tienes en el mismo servicio)
        public Venta? Venta { get; set; }

        // 🔹 Lógica para calcular subtotal automáticamente
        public void CalcularSubtotal()
        {
            Subtotal = Cantidad * PrecioUnitario;
        }
    }
}
