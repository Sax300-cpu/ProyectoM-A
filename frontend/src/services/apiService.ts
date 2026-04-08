import type { Cliente, Producto, Venta, CrearVentaRequest } from '../types';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5003/api';

export const apiService = {
  // Clientes
  async getClientes(): Promise<Cliente[]> {
    const response = await fetch(`${API_URL}/gateway/clientes`);
    if (!response.ok) throw new Error('Failed to fetch clientes');
    return response.json();
  },

  // Productos
  async getProductos(): Promise<Producto[]> {
    const response = await fetch(`${API_URL}/gateway/productos`);
    if (!response.ok) throw new Error('Failed to fetch productos');
    return response.json();
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
