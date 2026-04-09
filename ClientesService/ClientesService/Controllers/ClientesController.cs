using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClientesService.Data;
using ClientesService.Models;
using AutoMapper;
using ClientesService.DTOs;

namespace ClientesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesContext _context;
        private readonly IMapper _mapper;

        public ClientesController(ClientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ClienteDto>>(clientes));
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return _mapper.Map<ClienteDto>(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> PostCliente(ClienteCreateDto clienteCreateDto)
        {
            // Validar duplicados
            if (await _context.Clientes.AnyAsync(c => c.Cedula == clienteCreateDto.Cedula))
                return BadRequest(new { message = "Esta cédula ya está registrada por favor intente de nuevo" });

            if (!string.IsNullOrWhiteSpace(clienteCreateDto.Telefono) &&
                await _context.Clientes.AnyAsync(c => c.Telefono == clienteCreateDto.Telefono && c.Telefono != ""))
                return BadRequest(new { message = "Este teléfono ya está registrado por favor intente de nuevo" });

            if (await _context.Clientes.AnyAsync(c => c.Email == clienteCreateDto.Email))
                return BadRequest(new { message = "Este correo ya está registrado por favor intente de nuevo" });

            var cliente = _mapper.Map<Cliente>(clienteCreateDto);
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteID }, _mapper.Map<ClienteDto>(cliente));
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteUpdateDto clienteUpdateDto)
        {
            if (id != clienteUpdateDto.ClienteID)
                return BadRequest("El ID del cliente no coincide.");

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            _mapper.Map(clienteUpdateDto, cliente);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(e => e.ClienteID == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

