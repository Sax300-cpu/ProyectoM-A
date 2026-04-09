using System.ComponentModel.DataAnnotations;

namespace ProductosService.DTOs
{
    public class ReducirStockDto
    {
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}