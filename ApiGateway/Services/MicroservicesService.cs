using ApiGateway.DTOs;
using System.Net.Http.Json;

namespace ApiGateway.Services
{
    public interface IMicroservicesService
    {
        Task<List<ClienteDto>> GetClientesAsync();
        Task<List<ProductoDto>> GetProductosAsync();
        Task<VentaDto> CreateVentaAsync(CrearVentaRequestDto request);
        Task<List<VentaDto>> GetVentasAsync();
    }

    public class MicroservicesService : IMicroservicesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientesServiceUrl = "http://localhost:5096";
        private readonly string _productosServiceUrl = "http://localhost:5001";
        private readonly string _ventasServiceUrl = "http://localhost:5000";

        public MicroservicesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ClienteDto>> GetClientesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_clientesServiceUrl}/api/clientes");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ClienteDto>>() ?? new List<ClienteDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting clientes: {ex.Message}");
                return new List<ClienteDto>();
            }
        }

        public async Task<List<ProductoDto>> GetProductosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_productosServiceUrl}/api/productos");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<ProductoDto>>() ?? new List<ProductoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting productos: {ex.Message}");
                return new List<ProductoDto>();
            }
        }

        public async Task<VentaDto> CreateVentaAsync(CrearVentaRequestDto request)
        {
            try
            {
                var venta = new
                {
                    clienteID = request.ClienteID,
                    fecha = DateTime.Now,
                    subtotal = request.Detalles.Sum(d => d.PrecioUnitario * d.Cantidad),
                    iva = request.Detalles.Sum(d => (d.PrecioUnitario * d.Cantidad) * 0.19m),
                    total = request.Detalles.Sum(d => (d.PrecioUnitario * d.Cantidad) * 1.19m)
                };

                var response = await _httpClient.PostAsJsonAsync($"{_ventasServiceUrl}/api/ventas", venta);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<VentaDto>() ?? new VentaDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating venta: {ex.Message}");
                throw;
            }
        }

        public async Task<List<VentaDto>> GetVentasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ventasServiceUrl}/api/ventas");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<VentaDto>>() ?? new List<VentaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ventas: {ex.Message}");
                return new List<VentaDto>();
            }
        }
    }
}
