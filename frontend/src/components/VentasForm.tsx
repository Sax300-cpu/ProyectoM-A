import { useState, useEffect } from 'react';
import type { Cliente, Producto, DetalleVentaRequest } from '../types';
import { apiService } from '../services/apiService';
import './VentasForm.css';

export const VentasForm = () => {
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [clienteSeleccionado, setClienteSeleccionado] = useState<number | ''>('');
  const [detalles, setDetalles] = useState<DetalleVentaRequest[]>([]);
  const [productoSeleccionado, setProductoSeleccionado] = useState<number | ''>('');
  const [cantidadSeleccionada, setCantidadSeleccionada] = useState<number>(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  // Cargar clientes y productos al montar el componente
  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [clientesData, productosData] = await Promise.all([
          apiService.getClientes(),
          apiService.getProductos(),
        ]);
        setClientes(clientesData);
        setProductos(productosData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error loading data');
      } finally {
        setLoading(false);
      }
    };
    loadData();
  }, []);

  const handleAgregarProducto = () => {
    if (!productoSeleccionado || !cantidadSeleccionada) {
      setError('Selecciona producto y cantidad');
      return;
    }

    const producto = productos.find(p => p.productoID === productoSeleccionado);
    if (!producto) return;

    if (producto.stock < cantidadSeleccionada) {
      setError(`Stock insuficiente. Disponible: ${producto.stock}`);
      return;
    }

    const detalleExistente = detalles.find(d => d.productoID === productoSeleccionado);
    if (detalleExistente) {
      setDetalles(detalles.map(d =>
        d.productoID === productoSeleccionado
          ? { ...d, cantidad: d.cantidad + cantidadSeleccionada }
          : d
      ));
    } else {
      setDetalles([
        ...detalles,
        {
          productoID: productoSeleccionado as number,
          cantidad: cantidadSeleccionada,
          precioUnitario: producto.precio,
        },
      ]);
    }

    setProductoSeleccionado('');
    setCantidadSeleccionada(1);
    setError(null);
  };

  const handleRemoverProducto = (productoID: number) => {
    setDetalles(detalles.filter(d => d.productoID !== productoID));
  };

  const calcularTotales = () => {
    const subtotal = detalles.reduce(
      (sum, d) => sum + d.precioUnitario * d.cantidad,
      0
    );
    const iva = subtotal * 0.19;
    const total = subtotal + iva;
    return { subtotal, iva, total };
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);

    if (!clienteSeleccionado || detalles.length === 0) {
      setError('Selecciona un cliente y al menos un producto');
      return;
    }

    try {
      setLoading(true);
      await apiService.createVenta({
        clienteID: clienteSeleccionado as number,
        detalles,
      });

      setSuccess(true);
      setClienteSeleccionado('');
      setDetalles([]);
      setProductoSeleccionado('');
      setCantidadSeleccionada(1);

      setTimeout(() => setSuccess(false), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error creating venta');
    } finally {
      setLoading(false);
    }
  };

  const { subtotal, iva, total } = calcularTotales();
  const clienteInfo = clientes.find(c => c.clienteID === clienteSeleccionado);

  if (loading && clientes.length === 0) {
    return <div className="loading">Cargando datos...</div>;
  }

  return (
    <div className="ventas-form-container">
      <h1>Formulario de Ventas</h1>

      {error && <div className="alert alert-error">{error}</div>}
      {success && <div className="alert alert-success">¡Venta creada exitosamente!</div>}

      <div className="form-section">
        <h2>1. Seleccionar Cliente</h2>
        <div className="form-group">
          <label htmlFor="cliente">Cliente:</label>
          <select
            id="cliente"
            value={clienteSeleccionado}
            onChange={(e) => setClienteSeleccionado(e.target.value ? Number(e.target.value) : '')}
          >
            <option value="">-- Selecciona un cliente --</option>
            {clientes.map(cliente => (
              <option key={cliente.clienteID} value={cliente.clienteID}>
                {cliente.nombre} {cliente.apellido} ({cliente.cedula})
              </option>
            ))}
          </select>
        </div>

        {clienteInfo && (
          <div className="client-info">
            <p><strong>Email:</strong> {clienteInfo.email}</p>
            <p><strong>Teléfono:</strong> {clienteInfo.telefono}</p>
            <p><strong>Dirección:</strong> {clienteInfo.direccion}</p>
          </div>
        )}
      </div>

      <div className="form-section">
        <h2>2. Agregar Productos</h2>
        <div className="product-form">
          <div className="form-group">
            <label htmlFor="producto">Producto:</label>
            <select
              id="producto"
              value={productoSeleccionado}
              onChange={(e) => setProductoSeleccionado(e.target.value ? Number(e.target.value) : '')}
            >
              <option value="">-- Selecciona un producto --</option>
              {productos.filter(p => p.activo).map(producto => (
                <option key={producto.productoID} value={producto.productoID}>
                  {producto.nombre} - ${producto.precio.toFixed(2)} (Stock: {producto.stock})
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="cantidad">Cantidad:</label>
            <input
              id="cantidad"
              type="number"
              min="1"
              value={cantidadSeleccionada}
              onChange={(e) => setCantidadSeleccionada(parseInt(e.target.value) || 1)}
            />
          </div>

          <button
            type="button"
            onClick={handleAgregarProducto}
            className="btn btn-primary"
          >
            Agregar
          </button>
        </div>
      </div>

      {detalles.length > 0 && (
        <div className="form-section">
          <h2>3. Detalle de Venta</h2>
          <div className="detalles-table">
            <table>
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Cantidad</th>
                  <th>Precio Unitario</th>
                  <th>Subtotal</th>
                  <th>Acción</th>
                </tr>
              </thead>
              <tbody>
                {detalles.map(detalle => {
                  const producto = productos.find(p => p.productoID === detalle.productoID);
                  const subtotal = detalle.cantidad * detalle.precioUnitario;
                  return (
                    <tr key={detalle.productoID}>
                      <td>{producto?.nombre}</td>
                      <td>{detalle.cantidad}</td>
                      <td>${detalle.precioUnitario.toFixed(2)}</td>
                      <td>${subtotal.toFixed(2)}</td>
                      <td>
                        <button
                          type="button"
                          onClick={() => handleRemoverProducto(detalle.productoID)}
                          className="btn btn-danger btn-sm"
                        >
                          Quitar
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="totales">
            <div className="total-row">
              <span>Subtotal:</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            <div className="total-row">
              <span>IVA (19%):</span>
              <span>${iva.toFixed(2)}</span>
            </div>
            <div className="total-row total-final">
              <span>Total:</span>
              <span>${total.toFixed(2)}</span>
            </div>
          </div>
        </div>
      )}

      <div className="form-actions">
        <button
          onClick={handleSubmit}
          disabled={!clienteSeleccionado || detalles.length === 0 || loading}
          className="btn btn-success"
        >
          {loading ? 'Procesando...' : 'Crear Venta'}
        </button>
        <button
          type="button"
          onClick={() => {
            setClienteSeleccionado('');
            setDetalles([]);
            setError(null);
          }}
          className="btn btn-secondary"
        >
          Limpiar Formulario
        </button>
      </div>
    </div>
  );
};
