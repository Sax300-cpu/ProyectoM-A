using System;
using System.ComponentModel.DataAnnotations;

namespace VentasService.DTOs
{
    public class VentaUpdateDto
    {
        [Required]
        public int VentaID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [Required]
        public decimal IVA { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}
