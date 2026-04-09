using ApiGateway.DTOs;
using System.Net.Http.Json;

namespace ApiGateway.Services
{
    public interface IMicroservicesService
    {
        Task<List<ClienteDto>> GetClientesAsync();
        Task<ClienteDto?> GetClienteAsync(int id);
        Task<ClienteDto> CreateClienteAsync(ClienteCreateDto request);
        Task UpdateClienteAsync(int id, ClienteUpdateDto request);
        Task DeleteClienteAsync(int id);

        Task<List<ProductoDto>> GetProductosAsync();
        Task<ProductoDto?> GetProductoAsync(int id);
        Task<ProductoDto> CreateProductoAsync(ProductoCreateDto request);
        Task UpdateProductoAsync(int id, ProductoUpdateDto request);
        Task DeleteProductoAsync(int id);

        Task<VentaDto> CreateVentaAsync(CrearVentaRequestDto request);
        Task<List<VentaDto>> GetVentasAsync();
        Task<VentaDto?> GetVentaAsync(int id);
        Task<List<VentaDto>> GetVentasPorClienteAsync(int clienteId);
        Task UpdateVentaAsync(int id, VentaUpdateRequestDto request);
        Task DeleteVentaAsync(int id);
        Task<List<ProductoVendidoDto>> GetProductosMasVendidosAsync(int top);
    }

    public class MicroservicesService : IMicroservicesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientesServiceUrl;
        private readonly string _productosServiceUrl;
        private readonly string _ventasServiceUrl;

        public MicroservicesService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _clientesServiceUrl = configuration["Microservices:ClientesServiceUrl"] ?? "http://localhost:5096";
            _productosServiceUrl = configuration["Microservices:ProductosServiceUrl"] ?? "http://localhost:5001";
            _ventasServiceUrl = configuration["Microservices:VentasServiceUrl"] ?? "http://localhost:5000";
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

        public async Task<ClienteDto?> GetClienteAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_clientesServiceUrl}/api/clientes/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ClienteDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting cliente: {ex.Message}");
                return null;
            }
        }

        public async Task<ClienteDto> CreateClienteAsync(ClienteCreateDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_clientesServiceUrl}/api/clientes", request);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ClienteDto>()) ?? new ClienteDto();
        }

        public async Task UpdateClienteAsync(int id, ClienteUpdateDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_clientesServiceUrl}/api/clientes/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteClienteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_clientesServiceUrl}/api/clientes/{id}");
            response.EnsureSuccessStatusCode();
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

        public async Task<ProductoDto?> GetProductoAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_productosServiceUrl}/api/productos/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProductoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting producto: {ex.Message}");
                return null;
            }
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoCreateDto request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_productosServiceUrl}/api/productos", request);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<ProductoDto>()) ?? new ProductoDto();
        }

        public async Task UpdateProductoAsync(int id, ProductoUpdateDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_productosServiceUrl}/api/productos/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_productosServiceUrl}/api/productos/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<VentaDto> CreateVentaAsync(CrearVentaRequestDto request)
        {
            try
            {
                // Orquestación simple: validar y descontar stock antes de crear la venta.
                // Nota: no hay transacción distribuida; si algo falla después, no hay rollback automático.
                foreach (var d in request.Detalles)
                {
                    var producto = await GetProductoAsync(d.ProductoID);
                    if (producto == null)
                        throw new InvalidOperationException($"Producto {d.ProductoID} no existe.");

                    if (!producto.Activo)
                        throw new InvalidOperationException($"Producto {d.ProductoID} está inactivo.");

                    if (producto.Stock < d.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para producto {d.ProductoID}. Disponible: {producto.Stock}.");

                    var nuevoStock = producto.Stock - d.Cantidad;
                    await UpdateProductoAsync(d.ProductoID, new ProductoUpdateDto
                    {
                        ProductoID = producto.ProductoID,
                        Nombre = producto.Nombre ?? string.Empty,
                        Precio = producto.Precio,
                        Stock = nuevoStock,
                        Descripcion = producto.Descripcion ?? string.Empty,
                        Activo = producto.Activo
                    });
                }

                var response = await _httpClient.PostAsJsonAsync($"{_ventasServiceUrl}/api/ventas", request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<VentaDto>() ?? new VentaDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating venta: {ex.Message}");
                throw;
            }
        }

        public async Task<List<VentaDto>> GetVentasPorClienteAsync(int clienteId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ventasServiceUrl}/api/ventas/cliente/{clienteId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<VentaDto>>() ?? new List<VentaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting ventas por cliente: {ex.Message}");
                return new List<VentaDto>();
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

        public async Task<VentaDto?> GetVentaAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ventasServiceUrl}/api/ventas/{id}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<VentaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting venta: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateVentaAsync(int id, VentaUpdateRequestDto request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_ventasServiceUrl}/api/ventas/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVentaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_ventasServiceUrl}/api/ventas/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ProductoVendidoDto>> GetProductosMasVendidosAsync(int top)
        {
            var response = await _httpClient.GetAsync(
                $"{_ventasServiceUrl}/api/ventas/estadisticas/productos-mas-vendidos?top={top}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<ProductoVendidoDto>>() ?? new List<ProductoVendidoDto>();
        }
    }
}
