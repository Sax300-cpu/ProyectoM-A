namespace VentasService.DTOs
{
    public class DetalleVentaCreateDto
    {
        public int VentaID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        // 🔹 Subtotal se calculará en el modelo/controlador
    }
}
