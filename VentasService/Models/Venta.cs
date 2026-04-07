using System;

namespace VentasService.Models
{
    public class Venta
    {
        public int VentaID { get; set; }          // PK
        public int ClienteID { get; set; }        // FK hacia Clientes
        public DateTime Fecha { get; set; }       // Fecha de la venta
        public decimal Subtotal { get; set; }     // Valor antes de impuestos
        public decimal IVA { get; set; }          // Impuesto aplicado
        public decimal Total { get; set; }        // Valor final
    }
}
