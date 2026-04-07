using System.ComponentModel.DataAnnotations;

namespace ClientesService.DTOs
{
    public class ClienteUpdateDto
    {
        [Required]
        public int ClienteID { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [StringLength(10, ErrorMessage = "La cédula no puede superar los 10 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cédula sólo debe contener números.")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(20)]
        public string Apellido { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "El teléfono no puede superar los 10 caracteres.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono sólo debe contener números.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [RegularExpression(@"^[^@\s]+@(gmail\.com|hotmail\.com|yahoo\.com|outlook\.com)$", ErrorMessage = "Solo se permiten correos de Gmail, Hotmail, Yahoo u Outlook.")]
        public string Email { get; set; } = string.Empty;
    }
}