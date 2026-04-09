using ApiGateway.DTOs;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly IMicroservicesService _microservicesService;

        public GatewayController(IMicroservicesService microservicesService)
        {
            _microservicesService = microservicesService;
        }

        [HttpGet("clientes")]
        public async Task<ActionResult<List<ClienteDto>>> GetClientes()
        {
            var clientes = await _microservicesService.GetClientesAsync();
            return Ok(clientes);
        }

        [HttpGet("clientes/{id:int}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _microservicesService.GetClienteAsync(id);
            return cliente == null ? NotFound() : Ok(cliente);
        }

        [HttpPost("clientes")]
        public async Task<ActionResult<ClienteDto>> CreateCliente([FromBody] ClienteCreateDto request)
        {
            var created = await _microservicesService.CreateClienteAsync(request);
            return Ok(created);
        }

        [HttpPut("clientes/{id:int}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] ClienteUpdateDto request)
        {
            await _microservicesService.UpdateClienteAsync(id, request);
            return NoContent();
        }

        [HttpDelete("clientes/{id:int}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            await _microservicesService.DeleteClienteAsync(id);
            return NoContent();
        }

        [HttpGet("productos")]
        public async Task<ActionResult<List<ProductoDto>>> GetProductos()
        {
            var productos = await _microservicesService.GetProductosAsync();
            return Ok(productos);
        }

        [HttpGet("productos/{id:int}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            var producto = await _microservicesService.GetProductoAsync(id);
            return producto == null ? NotFound() : Ok(producto);
        }

        [HttpPost("productos")]
        public async Task<ActionResult<ProductoDto>> CreateProducto([FromBody] ProductoCreateDto request)
        {
            var created = await _microservicesService.CreateProductoAsync(request);
            return Ok(created);
        }

        [HttpPut("productos/{id:int}")]
        public async Task<IActionResult> UpdateProducto(int id, [FromBody] ProductoUpdateDto request)
        {
            await _microservicesService.UpdateProductoAsync(id, request);
            return NoContent();
        }

        [HttpDelete("productos/{id:int}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            await _microservicesService.DeleteProductoAsync(id);
            return NoContent();
        }

        [HttpGet("ventas")]
        public async Task<ActionResult<List<VentaDto>>> GetVentas()
        {
            var ventas = await _microservicesService.GetVentasAsync();
            return Ok(ventas);
        }

        [HttpGet("ventas/estadisticas/productos-mas-vendidos")]
        public async Task<ActionResult<List<ProductoVendidoDto>>> GetProductosMasVendidos([FromQuery] int top = 10)
        {
            var rows = await _microservicesService.GetProductosMasVendidosAsync(top);
            return Ok(rows);
        }

        [HttpGet("ventas/{id:int}")]
        public async Task<ActionResult<VentaDto>> GetVenta(int id)
        {
            var venta = await _microservicesService.GetVentaAsync(id);
            return venta == null ? NotFound() : Ok(venta);
        }

        [HttpGet("ventas/cliente/{clienteId}")]
        public async Task<ActionResult<List<VentaDto>>> GetVentasPorCliente(int clienteId)
        {
            var ventas = await _microservicesService.GetVentasPorClienteAsync(clienteId);
            return Ok(ventas);
        }

        [HttpPost("ventas")]
        public async Task<ActionResult<VentaDto>> CreateVenta([FromBody] CrearVentaRequestDto request)
        {
            var venta = await _microservicesService.CreateVentaAsync(request);
            return Ok(venta);
        }

        [HttpPut("ventas/{id:int}")]
        public async Task<IActionResult> UpdateVenta(int id, [FromBody] VentaUpdateRequestDto request)
        {
            await _microservicesService.UpdateVentaAsync(id, request);
            return NoContent();
        }

        [HttpDelete("ventas/{id:int}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            await _microservicesService.DeleteVentaAsync(id);
            return NoContent();
        }
    }
}
