using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VentasService.Data;
using VentasService.Models;
using VentasService.DTOs;

namespace VentasService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleVentasController : ControllerBase
    {
        private readonly VentasContext _context;
        private readonly IMapper _mapper;

        public DetalleVentasController(VentasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🔹 GET: api/detalleventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleVentaDto>>> GetDetalles()
        {
            var detalles = await _context.DetallesVenta.ToListAsync();
            return _mapper.Map<List<DetalleVentaDto>>(detalles);
        }

        // 🔹 GET: api/detalleventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleVentaDto>> GetDetalle(int id)
        {
            var detalle = await _context.DetallesVenta.FindAsync(id);
            if (detalle == null) return NotFound();

            return _mapper.Map<DetalleVentaDto>(detalle);
        }

        // 🔹 POST: api/detalleventas
        [HttpPost]
        public async Task<ActionResult<DetalleVentaDto>> PostDetalle(DetalleVentaCreateDto dto)
        {
            var detalle = _mapper.Map<DetalleVenta>(dto);
            detalle.CalcularSubtotal(); // asegura coherencia

            _context.DetallesVenta.Add(detalle);
            await _context.SaveChangesAsync();

            var detalleDto = _mapper.Map<DetalleVentaDto>(detalle);
            return CreatedAtAction(nameof(GetDetalle), new { id = detalle.DetalleID }, detalleDto);
        }

        // 🔹 PUT: api/detalleventas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDetalle(int id, DetalleVentaUpdateDto dto)
        {
            if (id != dto.DetalleID) return BadRequest();

            var detalle = await _context.DetallesVenta.FindAsync(id);
            if (detalle == null) return NotFound();

            _mapper.Map(dto, detalle);
            detalle.CalcularSubtotal(); // recalcula subtotal

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 🔹 DELETE: api/detalleventas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDetalle(int id)
        {
            var detalle = await _context.DetallesVenta.FindAsync(id);
            if (detalle == null) return NotFound();

            _context.DetallesVenta.Remove(detalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
