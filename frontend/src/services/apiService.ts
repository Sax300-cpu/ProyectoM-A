import type {
  Cliente,
  ClienteCreateRequest,
  ClienteUpdateRequest,
  Producto,
  ProductoCreateRequest,
  ProductoUpdateRequest,
  Venta,
  CrearVentaRequest,
  ProductoVendido,
} from '../types';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5003/api';

export const apiService = {
  // Clientes
  async getClientes(): Promise<Cliente[]> {
    const response = await fetch(`${API_URL}/gateway/clientes`);
    if (!response.ok) throw new Error('Failed to fetch clientes');
    return response.json();
  },

  async createCliente(cliente: ClienteCreateRequest): Promise<Cliente> {
    const response = await fetch(`${API_URL}/gateway/clientes`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(cliente),
    });
    if (!response.ok) {
      let errorMsg = 'No se pudo crear el Cliente, verifique los datos cédula, correo o teléfono.';
      try {
        const data = await response.json();
        if (data && typeof data.message === 'string') errorMsg = data.message;
        else if (data && data.errors && Array.isArray(data.errors) && data.errors.length > 0) errorMsg = data.errors[0];
      } catch {}
      throw new Error(errorMsg);
    }
    return response.json();
  },

  async updateCliente(id: number, cliente: ClienteUpdateRequest): Promise<void> {
    const response = await fetch(`${API_URL}/gateway/clientes/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(cliente),
    });
    if (!response.ok) throw new Error('Failed to update cliente');
  },

  async deleteCliente(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/gateway/clientes/${id}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to delete cliente');
  },

  // Productos
  async getProductos(): Promise<Producto[]> {
    const response = await fetch(`${API_URL}/gateway/productos`);
    if (!response.ok) throw new Error('Failed to fetch productos');
    return response.json();
  },

  async createProducto(producto: ProductoCreateRequest): Promise<Producto> {
    const response = await fetch(`${API_URL}/gateway/productos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(producto),
    });
    if (!response.ok) {
      let errorMsg = 'No se pudo crear el producto, verifique el nombre.';
      try {
        const data = await response.json();
        if (data && typeof data.message === 'string') errorMsg = data.message;
        else if (data && data.errors && Array.isArray(data.errors) && data.errors.length > 0) errorMsg = data.errors[0];
      } catch {}
      throw new Error(errorMsg);
    }
    return response.json();
  },

  async updateProducto(id: number, producto: ProductoUpdateRequest): Promise<void> {
    const response = await fetch(`${API_URL}/gateway/productos/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(producto),
    });
    if (!response.ok) throw new Error('Failed to update producto');
  },

  async deleteProducto(id: number): Promise<void> {
    const response = await fetch(`${API_URL}/gateway/productos/${id}`, { method: 'DELETE' });
    if (!response.ok) throw new Error('Failed to delete producto');
  },

  // Ventas
  async getVentas(): Promise<Venta[]> {
    const response = await fetch(`${API_URL}/gateway/ventas`);
    if (!response.ok) throw new Error('Failed to fetch ventas');
    return response.json();
  },

  async getVentasByCliente(clienteID: number): Promise<Venta[]> {
    const response = await fetch(`${API_URL}/gateway/ventas/cliente/${clienteID}`);
    if (!response.ok) throw new Error('Failed to fetch historial de ventas');
    return response.json();
  },

  async getProductosMasVendidos(top = 10): Promise<ProductoVendido[]> {
    const response = await fetch(
      `${API_URL}/gateway/ventas/estadisticas/productos-mas-vendidos?top=${top}`,
    );
    if (!response.ok) throw new Error('Failed to fetch ranking de productos');
    return response.json();
  },

  async createVenta(venta: CrearVentaRequest): Promise<Venta> {
    const response = await fetch(`${API_URL}/gateway/ventas`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(venta),
    });
    if (!response.ok) throw new Error('Failed to create venta');
    return response.json();
  },
};
