using Microsoft.EntityFrameworkCore;
using ClientesService.Models;

namespace ClientesService.Data
{
    public class ClientesContext : DbContext
    {
        public ClientesContext(DbContextOptions<ClientesContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
    }
}