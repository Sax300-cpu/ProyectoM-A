using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentasService.Data;
using VentasService.Models;
using VentasService.DTOs;
using AutoMapper;

namespace VentasService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        /// <summary>Tasa de IVA aplicada al subtotal (0.12 = 12%).</summary>
        private const decimal TasaIva = 0.12m;

        private readonly VentasContext _context;
        private readonly IMapper _mapper;

        public VentasController(VentasContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .ToListAsync();

            foreach (var venta in ventas)
                RecalcularTotalesDesdeDetalles(venta);

            return Ok(_mapper.Map<IEnumerable<VentaDto>>(ventas));
        }

        // GET: api/ventas/cliente/5
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentasPorCliente(int clienteId)
        {
            var ventas = await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.ClienteID == clienteId)
                .ToListAsync();

            foreach (var venta in ventas)
                RecalcularTotalesDesdeDetalles(venta);

            return Ok(_mapper.Map<IEnumerable<VentaDto>>(ventas));
        }

        /// <summary>
        /// Unidades vendidas por producto (agregado sobre DetalleVenta). No requiere cambios de esquema.
        /// </summary>
        [HttpGet("estadisticas/productos-mas-vendidos")]
        public async Task<ActionResult<IEnumerable<ProductoVendidoDto>>> GetProductosMasVendidos([FromQuery] int top = 10)
        {
            top = Math.Clamp(top, 1, 100);
            var rows = await _context.DetallesVenta
                .AsNoTracking()
                .GroupBy(d => d.ProductoID)
                .Select(g => new ProductoVendidoDto
                {
                    ProductoID = g.Key,
                    UnidadesVendidas = g.Sum(x => x.Cantidad),
                })
                .OrderByDescending(x => x.UnidadesVendidas)
                .Take(top)
                .ToListAsync();

            return Ok(rows);
        }

        // GET: api/ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.VentaID == id);

            if (venta == null)
                return NotFound();

            RecalcularTotalesDesdeDetalles(venta);
            return _mapper.Map<VentaDto>(venta);
        }

        /// <summary>
        /// Recalcula subtotal por línea, subtotal de venta, IVA y total a partir de los detalles cargados.
        /// </summary>
        private static void RecalcularTotalesDesdeDetalles(Venta venta)
        {
            foreach (var d in venta.Detalles)
                d.CalcularSubtotal();

            venta.Subtotal = venta.Detalles.Sum(d => d.Subtotal);
            venta.Iva = Math.Round(venta.Subtotal * TasaIva, 2, MidpointRounding.AwayFromZero);
            venta.Total = venta.Subtotal + venta.Iva;
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<VentaDto>> PostVenta(VentaCreateDto ventaCreateDto)
        {
            var venta = _mapper.Map<Venta>(ventaCreateDto);

            if (venta.Fecha == default)
            {
                venta.Fecha = DateTime.Now;
            }

            foreach (var detalle in venta.Detalles)
            {
                detalle.CalcularSubtotal();
            }

            RecalcularTotalesDesdeDetalles(venta);

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenta), new { id = venta.VentaID }, _mapper.Map<VentaDto>(venta));
        }

        // PUT: api/ventas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenta(int id, VentaUpdateDto ventaUpdateDto)
        {
            if (id != ventaUpdateDto.VentaID)
                return BadRequest("El ID de la venta no coincide.");

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound();

            _mapper.Map(ventaUpdateDto, venta);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ventas.Any(e => e.VentaID == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/ventas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
                return NotFound();

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
