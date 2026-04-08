export interface Cliente {
  clienteID: number;
  cedula: string;
  nombre: string;
  apellido: string;
  direccion: string;
  telefono: string;
  email: string;
}

export interface Producto {
  productoID: number;
  nombre: string;
  precio: number;
  stock: number;
  descripcion: string;
  activo: boolean;
}

export interface DetalleVenta {
  detalleID?: number;
  ventaID?: number;
  productoID: number;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface Venta {
  ventaID?: number;
  clienteID: number;
  fecha?: string;
  subtotal: number;
  iva: number;
  total: number;
  detalles: DetalleVenta[];
}

export interface DetalleVentaRequest {
  productoID: number;
  cantidad: number;
  precioUnitario: number;
}

export interface CrearVentaRequest {
  clienteID: number;
  detalles: DetalleVentaRequest[];
}
