using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosService.Data;
using ProductosService.Models;
using ProductosService.DTOs;

namespace ProductosService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ProductosContext _context;

        public ProductosController(ProductosContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new ProductoDTO
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Descripcion = p.Descripcion,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.ProductoID == id && p.Activo)
                .Select(p => new ProductoDTO
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Descripcion = p.Descripcion,
                    Activo = p.Activo
                })
                .FirstOrDefaultAsync();

            if (producto == null)
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });

            return Ok(producto);
        }

        // GET: api/productos/buscar/nombre?nombre=...
        [HttpGet("buscar/nombre")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> BuscarPorNombre([FromQuery] string nombre)
        {
            if (string.IsNullOrEmpty(nombre))
                return BadRequest(new { mensaje = "Debe proporcionar un nombre para buscar." });

            var productos = await _context.Productos
                .Where(p => p.Activo && p.Nombre.Contains(nombre))
                .Select(p => new ProductoDTO
                {
                    ProductoID = p.ProductoID,
                    Nombre = p.Nombre,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    Descripcion = p.Descripcion,
                    Activo = p.Activo
                })
                .ToListAsync();

            return Ok(productos);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoDTO>> CreateProducto(ProductoCreateDTO productoDTO)
        {
            // Validar que no exista un producto con el mismo nombre
            var existeProducto = await _context.Productos
                .AnyAsync(p => p.Nombre.ToLower() == productoDTO.Nombre.ToLower() && p.Activo);

            if (existeProducto)
                return BadRequest(new { mensaje = "Ya existe un producto con ese nombre." });

            var producto = new Producto
            {
                Nombre = productoDTO.Nombre,
                Precio = productoDTO.Precio,
                Stock = productoDTO.Stock,
                Descripcion = productoDTO.Descripcion,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var result = new ProductoDTO
            {
                ProductoID = producto.ProductoID,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Descripcion = producto.Descripcion,
                Activo = producto.Activo
            };

            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoID }, result);
        }

        // PUT: api/productos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProducto(int id, ProductoUpdateDTO productoDTO)
        {
            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });

            // Validar que el stock no sea negativo
            if (productoDTO.Stock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            // Validar que el precio sea mayor a 0
            if (productoDTO.Precio <= 0)
                return BadRequest(new { mensaje = "El precio debe ser mayor a 0." });

            // Validar que no exista otro producto con el mismo nombre
            var existeProducto = await _context.Productos
                .AnyAsync(p => p.Nombre.ToLower() == productoDTO.Nombre.ToLower() 
                            && p.ProductoID != id 
                            && p.Activo);

            if (existeProducto)
                return BadRequest(new { mensaje = "Ya existe otro producto con ese nombre." });

            producto.Nombre = productoDTO.Nombre;
            producto.Precio = productoDTO.Precio;
            producto.Stock = productoDTO.Stock;
            producto.Descripcion = productoDTO.Descripcion;

            _context.Entry(producto).State = EntityState.Modified;

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

        // DELETE: api/productos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });

            // Soft delete - solo marcamos como inactivo
            producto.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto eliminado exitosamente." });
        }

        // DELETE: api/productos/{id}/hard - Eliminación física (opcional)
        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Producto eliminado permanentemente." });
        }

        // PATCH: api/productos/{id}/stock - Actualizar stock específicamente
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] int nuevoStock)
        {
            if (nuevoStock < 0)
                return BadRequest(new { mensaje = "El stock no puede ser negativo." });

            var producto = await _context.Productos.FindAsync(id);
            
            if (producto == null)
                return NotFound(new { mensaje = $"Producto con ID {id} no encontrado." });

            producto.Stock = nuevoStock;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Stock actualizado exitosamente.", stock = producto.Stock });
        }
    }
}