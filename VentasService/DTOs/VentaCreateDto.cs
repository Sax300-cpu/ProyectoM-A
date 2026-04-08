using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VentasService.DTOs
{
    public class VentaCreateDto
    {
        [Required]
        public int ClienteID { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        public List<DetalleVentaCreateDto> Detalles { get; set; } = new();
    }
}
