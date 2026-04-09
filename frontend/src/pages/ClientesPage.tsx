import { useEffect, useMemo, useState } from 'react';
import { ConfirmModal } from '../components/ConfirmModal';
import type { Cliente } from '../types';
import { apiService } from '../services/apiService';
import './Page.css';

type FormState = Omit<Cliente, 'clienteID'> & { clienteID?: number };

const emptyForm: FormState = {
  cedula: '',
  nombre: '',
  apellido: '',
  direccion: '',
  telefono: '',
  email: '',
};

export function ClientesPage() {
  const [confirmOpen, setConfirmOpen] = useState(false);
  const [clienteAEliminar, setClienteAEliminar] = useState<number | null>(null);
  const [clientes, setClientes] = useState<Cliente[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [query, setQuery] = useState('');

  const [form, setForm] = useState<FormState>(emptyForm);
  const isEditing = typeof form.clienteID === 'number';

  const filtered = useMemo(() => {
    const q = query.trim().toLowerCase();
    if (!q) return clientes;
    return clientes.filter((c) => {
      const text = `${c.cedula} ${c.nombre} ${c.apellido} ${c.email} ${c.telefono} ${c.direccion}`.toLowerCase();
      return text.includes(q);
    });
  }, [clientes, query]);

  async function reload() {
    try {
      setError(null);
      setLoading(true);
      const data = await apiService.getClientes();
      setClientes(data);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error cargando clientes');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    reload();
  }, []);

  function startEdit(cliente: Cliente) {
    setForm({ ...cliente });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  function resetForm() {
    setForm(emptyForm);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    // Validación de campos obligatorios
    if (!form.cedula.trim() || !form.nombre.trim() || !form.apellido.trim() || !form.email.trim()) {
      setError('Por favor, complete todos los campos obligatorios: cédula, nombre, apellido y correo.');
      return;
    }
    setSaving(true);
    try {
      if (isEditing) {
        await apiService.updateCliente(form.clienteID!, {
          clienteID: form.clienteID!,
          cedula: form.cedula,
          nombre: form.nombre,
          apellido: form.apellido,
          direccion: form.direccion,
          telefono: form.telefono,
          email: form.email,
        });
      } else {
        await apiService.createCliente({
          cedula: form.cedula,
          nombre: form.nombre,
          apellido: form.apellido,
          direccion: form.direccion,
          telefono: form.telefono,
          email: form.email,
        });
      }
      resetForm();
      await reload();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error guardando cliente');
    } finally {
      setSaving(false);
    }
  }

  function pedirConfirmacionEliminar(id: number) {
    setClienteAEliminar(id);
    setConfirmOpen(true);
  }

  async function handleDeleteConfirmado() {
    if (clienteAEliminar == null) return;
    setSaving(true);
    setError(null);
    try {
      await apiService.deleteCliente(clienteAEliminar);
      await reload();
      if (form.clienteID === clienteAEliminar) resetForm();
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Error eliminando cliente');
    } finally {
      setSaving(false);
      setConfirmOpen(false);
      setClienteAEliminar(null);
    }
  }

  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Clientes</h1>
          <p className="page__subtitle">Crea, edita y elimina clientes.</p>
        </div>
        <button className="btn btn-secondary" type="button" onClick={reload} disabled={loading || saving}>
          Recargar
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="card">
        <div className="card__title">{isEditing ? `Editando Cliente #${form.clienteID}` : 'Nuevo Cliente'}</div>
        <form className="form-grid" onSubmit={handleSubmit}>
          <div className="form-field">
            <label>Cédula</label>
            <input value={form.cedula} onChange={(e) => setForm({ ...form, cedula: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Nombre</label>
            <input value={form.nombre} onChange={(e) => setForm({ ...form, nombre: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Apellido</label>
            <input value={form.apellido} onChange={(e) => setForm({ ...form, apellido: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Email</label>
            <input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} />
          </div>
          <div className="form-field">
            <label>Teléfono</label>
            <input value={form.telefono} onChange={(e) => setForm({ ...form, telefono: e.target.value })} />
          </div>
          <div className="form-field form-field--wide">
            <label>Dirección</label>
            <input value={form.direccion} onChange={(e) => setForm({ ...form, direccion: e.target.value })} />
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
          <input
            className="search"
            placeholder="Buscar por cédula, nombre, email..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
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
                  <th>Cédula</th>
                  <th>Nombre</th>
                  <th>Email</th>
                  <th>Teléfono</th>
                  <th>Acciones</th>
                </tr>
              </thead>
              <tbody>
                {filtered.map((c) => (
                  <tr key={c.clienteID}>
                    <td>{c.clienteID}</td>
                    <td>{c.cedula}</td>
                    <td>
                      {c.nombre} {c.apellido}
                    </td>
                    <td>{c.email}</td>
                    <td>{c.telefono}</td>
                    <td className="table-actions">
                      <button className="btn btn-primary btn-sm" type="button" onClick={() => startEdit(c)} disabled={saving}>
                        Editar
                      </button>
                      <button className="btn btn-danger btn-sm" type="button" onClick={() => pedirConfirmacionEliminar(c.clienteID)} disabled={saving}>
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
        title="Eliminar cliente"
        message="¿Estás seguro de que deseas eliminar este cliente? Esta acción no se puede deshacer."
        confirmText="Eliminar"
        cancelText="Cancelar"
        onConfirm={handleDeleteConfirmado}
        onCancel={() => { setConfirmOpen(false); setClienteAEliminar(null); }}
      />
    </div>
  );
}

