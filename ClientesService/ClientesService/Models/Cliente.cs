using System.ComponentModel.DataAnnotations;

namespace ClientesService.Models
{
    public class Cliente
    {
[Key]        
public string Cedula { get; set; } = string.Empty;
public string Nombre { get; set; } = string.Empty;
public string Apellido { get; set; } = string.Empty;
public string Direccion { get; set; } = string.Empty;
public string Telefono { get; set; } = string.Empty;
public string Email { get; set; } = string.Empty;
    }
}