namespace ApiGateway.DTOs
{
    public class VentaDto
    {
        public int VentaID { get; set; }
        public int ClienteID { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaDto
    {
        public int DetalleID { get; set; }
        public int VentaID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CrearVentaRequestDto
    {
        public int ClienteID { get; set; }
        public List<DetalleVentaRequestDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaRequestDto
    {
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
