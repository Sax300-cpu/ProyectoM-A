namespace ApiGateway.DTOs
{
    public class ProductoDto
    {
        public int ProductoID { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }
}
