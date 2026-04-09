import { useState, useEffect, useMemo } from 'react';
import { Link } from 'react-router-dom';
import type { Cliente, Producto, DetalleVentaRequest } from '../types';
import { apiService } from '../services/apiService';
import { ClienteSearchSelect } from './ClienteSearchSelect';
import '../pages/Page.css';
import './VentasForm.css';

function formatFechaProducto(iso?: string | null): string {
  if (!iso) return '—';
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '—';
  return d.toLocaleString('es', { dateStyle: 'medium', timeStyle: 'short' });
}

export const VentasForm = () => {
    const [busquedaProducto, setBusquedaProducto] = useState('');
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [clienteSeleccionado, setClienteSeleccionado] = useState<number | ''>('');
  const [detalles, setDetalles] = useState<DetalleVentaRequest[]>([]);
  const [productoSeleccionado, setProductoSeleccionado] = useState<number | ''>('');
  const [cantidadSeleccionada, setCantidadSeleccionada] = useState<number>(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const productoPreview = useMemo(() => {
    if (productoSeleccionado === '') return null;
    return productos.find((p) => p.productoID === productoSeleccionado) ?? null;
  }, [productoSeleccionado, productos]);

  const productosActivosOrdenados = useMemo(
    () => {
      let filtrados = productos.filter((p) => p.activo);
      if (busquedaProducto.trim() !== '') {
        filtrados = filtrados.filter((p) =>
          p.nombre.toLowerCase().includes(busquedaProducto.trim().toLowerCase())
        );
      }
      return filtrados.sort((a, b) =>
        a.nombre.localeCompare(b.nombre, 'es', { sensitivity: 'base' })
      );
    },
    [productos, busquedaProducto],
  );

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

    const producto = productos.find((p) => p.productoID === productoSeleccionado);
    if (!producto) return;

    if (producto.stock < cantidadSeleccionada) {
      setError(`Stock insuficiente. Disponible: ${producto.stock}`);
      return;
    }

    const detalleExistente = detalles.find((d) => d.productoID === productoSeleccionado);
    if (detalleExistente) {
      setDetalles(
        detalles.map((d) =>
          d.productoID === productoSeleccionado
            ? { ...d, cantidad: d.cantidad + cantidadSeleccionada }
            : d,
        ),
      );
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
    setDetalles(detalles.filter((d) => d.productoID !== productoID));
  };

  const calcularTotales = () => {
    const subtotal = detalles.reduce((sum, d) => sum + d.precioUnitario * d.cantidad, 0);
    const iva = subtotal * 0.12;
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
        subtotal,
        iva,
        total,
      });

      const productosActualizados = await apiService.getProductos();
      setProductos(productosActualizados);

      setSuccess(true);
      setClienteSeleccionado('');
      setDetalles([]);
      setProductoSeleccionado('');
      setCantidadSeleccionada(1);

      // Recargar productos para mostrar stock actualizado
      try {
        const productosData = await apiService.getProductos();
        setProductos(productosData);
      } catch (err) {
        console.error('Error recargando productos:', err);
      }

      setTimeout(() => setSuccess(false), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error creating venta');
    } finally {
      setLoading(false);
    }
  };

  const { subtotal, iva, total } = calcularTotales();
  const clienteInfo = clientes.find((c) => c.clienteID === clienteSeleccionado);

  const puedeAgregarLinea =
    productoPreview &&
    productoPreview.stock > 0 &&
    cantidadSeleccionada >= 1 &&
    cantidadSeleccionada <= productoPreview.stock;

  if (loading && clientes.length === 0) {
    return <div className="loading">Cargando datos...</div>;
  }

  return (
    <div className="ventas-form">
      {error && <div className="alert alert-error">{error}</div>}
      {success && (
        <div className="alert alert-success ventas-form__flash">Venta registrada correctamente.</div>
      )}

      <p className="ventas-form__nav-hint">
        ¿Historial o ranking de ventas?{' '}
        <Link to="/historial" className="ventas-form__nav-link">
          Abrir historial y reportes
        </Link>
      </p>

      <div className="card">
        <div className="card__title">1 · Cliente</div>
        <div className="form-field form-field--wide">
          <label htmlFor="ventas-cliente">Cliente</label>
          <ClienteSearchSelect
            id="ventas-cliente"
            clientes={clientes}
            value={clienteSeleccionado}
            onChange={setClienteSeleccionado}
            disabled={loading}
          />
        </div>

        {clienteInfo && (
          <div className="ventas-detail ventas-detail--cliente">
            <div className="ventas-detail__label">Datos del cliente</div>
            <div className="ventas-detail__grid">
              <div>
                <span className="ventas-detail__k">Correo</span>
                <span className="ventas-detail__v">{clienteInfo.email || '—'}</span>
              </div>
              <div>
                <span className="ventas-detail__k">Teléfono</span>
                <span className="ventas-detail__v">{clienteInfo.telefono || '—'}</span>
              </div>
              <div className="ventas-detail__full">
                <span className="ventas-detail__k">Dirección</span>
                <span className="ventas-detail__v">{clienteInfo.direccion || '—'}</span>
              </div>
            </div>
          </div>
        )}
      </div>

        <div className="card">
          <div className="card__title">2 · Agregar productos a la venta</div>
          <p className="ventas-form__hint">Elige solo el nombre en la lista; el detalle aparece debajo.</p>

          <div className="ventas-product-picker">
            <div className="form-field ventas-product-picker__search">
              <label htmlFor="busqueda-producto">Buscar producto</label>
              <input
                id="busqueda-producto"
                type="text"
                placeholder="Buscar por nombre..."
                value={busquedaProducto}
                onChange={(e) => setBusquedaProducto(e.target.value)}
              />
            </div>
            <div className="form-field ventas-product-picker__select">
              <label htmlFor="producto">Producto</label>
              <select
                id="producto"
                value={productoSeleccionado}
                onChange={(e) => setProductoSeleccionado(e.target.value ? Number(e.target.value) : '')}
              >
                <option value="">— Selecciona un producto —</option>
                {productosActivosOrdenados.map((p) => (
                  <option key={p.productoID} value={p.productoID}>
                    {p.nombre}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-field ventas-product-picker__qty">
              <label htmlFor="cantidad">Cantidad</label>
              <input
                id="cantidad"
                type="number"
                min={1}
                value={cantidadSeleccionada}
                onChange={(e) => setCantidadSeleccionada(parseInt(e.target.value, 10) || 1)}
              />
            </div>
            <div className="ventas-product-picker__btn">
              <button
                type="button"
                className="btn btn-success"
                onClick={handleAgregarProducto}
                disabled={!puedeAgregarLinea}
              >
                Aceptar
              </button>
            </div>
          </div>

        {productoPreview ? (
          <div className="ventas-product-card">
            <div className="ventas-product-card__top">
              <h3 className="ventas-product-card__name">{productoPreview.nombre}</h3>
              <span
                className={
                  productoPreview.stock === 0
                    ? 'ventas-stock ventas-stock--out'
                    : productoPreview.stock <= 5
                      ? 'ventas-stock ventas-stock--low'
                      : 'ventas-stock ventas-stock--ok'
                }
              >
                Stock: {productoPreview.stock}
              </span>
            </div>
            <p className="ventas-product-card__desc">
              {productoPreview.descripcion?.trim() ? productoPreview.descripcion : 'Sin descripción.'}
            </p>
            <dl className="ventas-product-card__meta">
              <div>
                <dt>Precio</dt>
                <dd>${productoPreview.precio.toFixed(2)}</dd>
              </div>
              <div>
                <dt>Fecha de creación</dt>
                <dd>{formatFechaProducto(productoPreview.fechaCreacion)}</dd>
              </div>
            </dl>
            {productoPreview.stock === 0 ? (
              <p className="ventas-product-card__warn">Este producto no tiene unidades disponibles.</p>
            ) : null}
          </div>
        ) : (
          <div className="ventas-product-placeholder">Selecciona un producto para ver su ficha.</div>
        )}
      </div>

      {detalles.length > 0 ? (
        <div className="card">
          <div className="card__title">3 · Resumen de la venta</div>
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Cant.</th>
                  <th>P. unit.</th>
                  <th>Subtotal</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {detalles.map((detalle) => {
                  const producto = productos.find((p) => p.productoID === detalle.productoID);
                  const lineSub = detalle.cantidad * detalle.precioUnitario;
                  return (
                    <tr key={detalle.productoID}>
                      <td>{producto?.nombre}</td>
                      <td>{detalle.cantidad}</td>
                      <td>${detalle.precioUnitario.toFixed(2)}</td>
                      <td>${lineSub.toFixed(2)}</td>
                      <td className="table-actions">
                        <button
                          type="button"
                          className="btn btn-danger btn-sm"
                          onClick={() => handleRemoverProducto(detalle.productoID)}
                        >
                          Cancelar
                        </button>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          <div className="ventas-totals">
            <div className="ventas-totals__row">
              <span>Subtotal</span>
              <span>${subtotal.toFixed(2)}</span>
            </div>
            <div className="ventas-totals__row">
              <span>IVA (12%)</span>
              <span>${iva.toFixed(2)}</span>
            </div>
            <div className="ventas-totals__row ventas-totals__row--total">
              <span>Total</span>
              <span>${total.toFixed(2)}</span>
            </div>
          </div>
        </div>
      ) : null}

      <div className="ventas-form__footer-actions">
        <button
          type="button"
          className="btn btn-success"
          onClick={handleSubmit}
          disabled={!clienteSeleccionado || detalles.length === 0 || loading}
        >
          {loading ? 'Procesando…' : 'Registrar venta'}
        </button>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={() => {
            setClienteSeleccionado('');
            setDetalles([]);
            setProductoSeleccionado('');
            setCantidadSeleccionada(1);
            setError(null);
          }}
        >
          Limpiar formulario
        </button>
      </div>
    </div>
  );
};
