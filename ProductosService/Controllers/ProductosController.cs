using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosService.Data;
using ProductosService.Models;
using AutoMapper;
using ProductosService.DTOs;

namespace ProductosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosContext _context;
        private readonly IMapper _mapper;

        public ProductosController(ProductosContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            var productos = await _context.Productos.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<ProductoDto>>(productos));
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
                return NotFound();

            return _mapper.Map<ProductoDto>(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> PostProducto(ProductoCreateDto productoCreateDto)
        {
            var producto = _mapper.Map<Producto>(productoCreateDto);
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoID }, _mapper.Map<ProductoDto>(producto));
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, ProductoUpdateDto productoUpdateDto)
        {
            if (id != productoUpdateDto.ProductoID)
                return BadRequest("El ID del producto no coincide.");

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _mapper.Map(productoUpdateDto, producto);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Productos.Any(e => e.ProductoID == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
                return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}