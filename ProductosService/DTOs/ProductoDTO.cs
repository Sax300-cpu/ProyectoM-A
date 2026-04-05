using System.ComponentModel.DataAnnotations;

namespace ProductosService.DTOs
{
    public class ProductoDTO
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    public class ProductoCreateDTO
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }
    }

    public class ProductoUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Precio { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }
    }
}