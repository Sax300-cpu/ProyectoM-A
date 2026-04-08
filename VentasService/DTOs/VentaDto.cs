namespace VentasService.DTOs
{
    public class VentaDto
    {
        public int VentaID { get; set; }
        public int ClienteID { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

        public ICollection<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }
}
