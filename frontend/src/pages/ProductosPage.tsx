import { useEffect, useMemo, useState } from 'react';
import { ConfirmModal } from '../components/ConfirmModal';
import type { Producto } from '../types';
import { apiService } from '../services/apiService';
import './Page.css';

type FormState = Omit<Producto, 'productoID' | 'productoUUID' | 'precio' | 'stock'> & { 
  productoID?: number; 
  productoUUID?: string; 
  precio: number | ''; 
  stock: number | ''; 
};

const emptyForm: FormState = {
  nombre: '',
  precio: '',
  stock: '',
  descripcion: '',
  activo: true,
};

export function ProductosPage() {
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [productoAEliminar, setProductoAEliminar] = useState<number | null>(null);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [query, setQuery] = useState('');

  const [form, setForm] = useState<FormState>(emptyForm);
  const isEditing = typeof form.productoID === 'number';

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    if (!q) return productos;
    return productos.filter((p) => {
      const text = `${p.productoID} ${p.nombre} ${p.descripcion}`.toLowerCase();
      return text.includes(q);
    });
  }, [productos, query]);

  async function reload() {
    try {
      setError(null);
      setLoading(true);
      const data = await apiService.getProductos();
      setProductos(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error cargando productos');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    reload();
  }, []);

  function startEdit(producto: Producto) {
    setForm({
      productoID: producto.productoID,
      nombre: producto.nombre,
      precio: producto.precio,
      stock: producto.stock,
      descripcion: producto.descripcion,
      activo: producto.activo,
      productoUUID: (producto as unknown as { productoUUID?: string }).productoUUID,
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function resetForm() {
    setForm(emptyForm);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      if (isEditing) {
        await apiService.updateProducto(form.productoID!, {
          productoID: form.productoID!,
          nombre: form.nombre,
              precio: Number(form.precio) || 0,
              stock: Number(form.stock) || 0,
          descripcion: form.descripcion,
          activo: form.activo,
        });
      } else {
        await apiService.createProducto({
          nombre: form.nombre,
              precio: Number(form.precio) || 0,
              stock: Number(form.stock) || 0,
          descripcion: form.descripcion,
          activo: form.activo,
        });
      }
      resetForm();
      await reload();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error guardando producto');
    } finally {
      setSaving(false);
    }
  }

  function pedirConfirmacionEliminar(id: number) {
    setProductoAEliminar(id);
    setConfirmOpen(true);
  }

  async function handleDeleteConfirmado() {
    if (productoAEliminar == null) return;
    setSaving(true);
    setError(null);
    try {
      await apiService.deleteProducto(productoAEliminar);
      await reload();
      if (form.productoID === productoAEliminar) resetForm();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error eliminando producto');
    } finally {
      setSaving(false);
      setConfirmOpen(false);
      setProductoAEliminar(null);
    }
  }

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Productos</h1>
          <p className="page__subtitle">Administra catálogo, stock y estado.</p>
        </div>
        <button className="btn btn-secondary" type="button" onClick={reload} disabled={loading || saving}>
          Recargar
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <div className="card__title">{isEditing ? `Editando Producto #${form.productoID}` : 'Nuevo Producto'}</div>
        <form className="form-grid" onSubmit={handleSubmit}>
          <div className="form-field">
            <label>Nombre</label>
            <input value={form.nombre} onChange={(e) => setForm({ ...form, nombre: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Precio</label>
            <input
              type="number"
              step="0.01"
              value={form.precio}
              onChange={(e) => setForm({ ...form, precio: e.target.value === '' ? '' : Number(e.target.value) })}
            />
          </div>
          <div className="form-field">
            <label>Stock</label>
            <input type="number" value={form.stock} onChange={(e) => setForm({ ...form, stock: e.target.value === '' ? '' : Number(e.target.value) })} />
          </div>
          <div className="form-field form-field--wide">
            <label>Descripción</label>
            <input value={form.descripcion} onChange={(e) => setForm({ ...form, descripcion: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Activo</label>
            <select value={form.activo ? '1' : '0'} onChange={(e) => setForm({ ...form, activo: e.target.value === '1' })}>
              <option value="1">Sí</option>
              <option value="0">No</option>
            </select>
          </div>

          <div className="form-actions">
            <button className="btn btn-success" type="submit" disabled={saving}>
              {saving ? 'Guardando...' : isEditing ? 'Actualizar' : 'Crear'}
            </button>
            <button className="btn btn-secondary" type="button" onClick={resetForm} disabled={saving}>
              Limpiar
            </button>
          </div>
        </form>
      </div>

      <div className="card">
        <div className="card__title">Listado</div>
        <div className="table-toolbar">
          <input className="search" placeholder="Buscar por nombre o descripción..." value={query} onChange={(e) => setQuery(e.target.value)} />
          <div className="table-toolbar__meta">{filtered.length} registros</div>
        </div>

        {loading ? (
          <div className="loading">Cargando...</div>
        ) : (
          <div className="table-wrap">
            <table className="table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nombre</th>
                  <th>Descripción</th>
                  <th>Precio</th>
                  <th>Stock</th>
                  <th>Activo</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map((p) => (
                  <tr key={p.productoID}>
                    <td>{p.productoID}</td>
                    <td>{p.nombre}</td>
                    <td>{p.descripcion}</td>
                    <td>${p.precio.toFixed(2)}</td>
                    <td>{p.stock}</td>
                    <td>{p.activo ? 'Sí' : 'No'}</td>
                    <td className="table-actions">
                      <button className="btn btn-primary btn-sm" type="button" onClick={() => startEdit(p)} disabled={saving}>
                        Editar
                      </button>
                      <button className="btn btn-danger btn-sm" type="button" onClick={() => pedirConfirmacionEliminar(p.productoID)} disabled={saving}>
                        Eliminar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
      <ConfirmModal
        open={confirmOpen}
        title="Eliminar producto"
        message="¿Estás seguro de que deseas eliminar este producto? Esta acción no se puede deshacer."
        confirmText="Eliminar"
        cancelText="Cancelar"
        onConfirm={handleDeleteConfirmado}
        onCancel={() => { setConfirmOpen(false); setProductoAEliminar(null); }}
      />
    </div>
  );
}
