import { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Cliente, Producto, ProductoVendido, Venta } from '../types';
import { apiService } from '../services/apiService';
import { ClienteSearchSelect } from '../components/ClienteSearchSelect';
import './Page.css';

export function HistorialVentasPage() {
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [clienteId, setClienteId] = useState<number | ''>('');
  const [ventas, setVentas] = useState<Venta[]>([]);
  const [ranking, setRanking] = useState<ProductoVendido[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadingVentas, setLoadingVentas] = useState(false);
  const [loadingRanking, setLoadingRanking] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const nombreProducto = (id: number) =>
    productos.find((p) => p.productoID === id)?.nombre ?? `Producto #${id}`;

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        setLoading(true);
        setError(null);
        const [c, p, r] = await Promise.all([
          apiService.getClientes(),
          apiService.getProductos(),
          apiService.getProductosMasVendidos(10),
        ]);
        if (!cancelled) {
          setClientes(c);
          setProductos(p);
          setRanking(r);
        }
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Error cargando datos');
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  useEffect(() => {
    if (clienteId === '') {
      setVentas([]);
      return;
    }
    let cancelled = false;
    (async () => {
      try {
        setLoadingVentas(true);
        setError(null);
        const v = await apiService.getVentasByCliente(clienteId as number);
        if (!cancelled) setVentas(v);
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Error cargando ventas');
      } finally {
        if (!cancelled) setLoadingVentas(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [clienteId]);

  const lineasCliente = useMemo(() => {
    return ventas.flatMap((v) =>
      (v.detalles ?? []).map((d) => ({
        ventaID: v.ventaID,
        fecha: v.fecha,
        productoID: d.productoID,
        cantidad: d.cantidad,
        precioUnitario: d.precioUnitario,
        subtotal: d.subtotal,
      })),
    );
  }, [ventas]);

  async function refreshRanking() {
    try {
      setLoadingRanking(true);
      const r = await apiService.getProductosMasVendidos(10);
      setRanking(r);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error actualizando ranking');
    } finally {
      setLoadingRanking(false);
    }
  }

  if (loading) {
    return (
      <div className="page">
        <p className="loading">Cargando…</p>
      </div>
    );
  }

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Historial y detalle de ventas</h1>
          <p className="page__subtitle">
            Consulta compras por cliente y el ranking de productos más vendidos (según líneas guardadas en
            ventas).{' '}
            <Link to="/" className="historial-page__link">
              Ir a nueva venta
            </Link>
          </p>
        </div>
        <button type="button" className="btn btn-secondary" onClick={refreshRanking} disabled={loadingRanking}>
          {loadingRanking ? 'Actualizando…' : 'Actualizar ranking'}
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <div className="card__title">Productos más vendidos (unidades)</div>
        {ranking.length === 0 ? (
          <p className="historial-page__empty">Aún no hay líneas de venta registradas.</p>
        ) : (
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>#</th>
                  <th>Producto</th>
                  <th>Unidades vendidas</th>
                </tr>
              </thead>
              <tbody>
                {ranking.map((row, i) => (
                  <tr key={row.productoID}>
                    <td>{i + 1}</td>
                    <td>{nombreProducto(row.productoID)}</td>
                    <td>{row.unidadesVendidas}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <div className="card">
        <div className="card__title">Historial por cliente</div>
        <div className="form-field form-field--wide">
          <label htmlFor="historial-cliente">Buscar y seleccionar cliente</label>
          <ClienteSearchSelect id="historial-cliente" clientes={clientes} value={clienteId} onChange={setClienteId} />
        </div>

        {clienteId === '' ? (
          <p className="historial-page__empty">Elige un cliente para ver sus ventas y el detalle por línea.</p>
        ) : loadingVentas ? (
          <p className="loading">Cargando ventas…</p>
        ) : ventas.length === 0 ? (
          <p className="historial-page__empty">Este cliente no tiene ventas registradas.</p>
        ) : (
          <>
            <div className="table-wrap historial-page__mt">
              <table className="table">
                <thead>
                  <tr>
                    <th>Venta</th>
                    <th>Fecha</th>
                    <th>Total</th>
                    <th>Líneas</th>
                  </tr>
                </thead>
                <tbody>
                  {ventas.map((v) => (
                    <tr key={v.ventaID}>
                      <td>{v.ventaID}</td>
                      <td>{v.fecha ? new Date(v.fecha).toLocaleString('es') : '—'}</td>
                      <td>${v.total.toFixed(2)}</td>
                      <td>{v.detalles?.length ?? 0}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="card__title historial-page__mt">Detalle por producto (todas las ventas del cliente)</div>
            <div className="table-wrap">
              <table className="table">
                <thead>
                  <tr>
                    <th>Venta</th>
                    <th>Fecha</th>
                    <th>Producto</th>
                    <th>Cant.</th>
                    <th>P. unit.</th>
                    <th>Subtotal línea</th>
                  </tr>
                </thead>
                <tbody>
                  {lineasCliente.map((line, idx) => (
                    <tr key={`${line.ventaID}-${line.productoID}-${idx}`}>
                      <td>{line.ventaID}</td>
                      <td>{line.fecha ? new Date(line.fecha).toLocaleString('es') : '—'}</td>
                      <td>{nombreProducto(line.productoID)}</td>
                      <td>{line.cantidad}</td>
                      <td>${line.precioUnitario.toFixed(2)}</td>
                      <td>${line.subtotal.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
