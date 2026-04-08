namespace VentasService.DTOs
{
    public class DetalleVentaUpdateDto
    {
        public int DetalleID { get; set; }
        public int VentaID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        // 🔹 Subtotal también se recalcula automáticamente
    }
}
