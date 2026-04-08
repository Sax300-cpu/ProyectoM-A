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

        [HttpGet("productos")]
        public async Task<ActionResult<List<ProductoDto>>> GetProductos()
        {
            var productos = await _microservicesService.GetProductosAsync();
            return Ok(productos);
        }

        [HttpGet("ventas")]
        public async Task<ActionResult<List<VentaDto>>> GetVentas()
        {
            var ventas = await _microservicesService.GetVentasAsync();
            return Ok(ventas);
        }

        [HttpPost("ventas")]
        public async Task<ActionResult<VentaDto>> CreateVenta([FromBody] CrearVentaRequestDto request)
        {
            var venta = await _microservicesService.CreateVentaAsync(request);
            return Ok(venta);
        }
    }
}
