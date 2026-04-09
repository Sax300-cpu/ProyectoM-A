import { useEffect, useMemo, useRef, useState } from 'react';
import type { Cliente } from '../types';
import './ClienteSearchSelect.css';

type Props = {
  id?: string;
  clientes: Cliente[];
  value: number | '';
  onChange: (clienteID: number | '') => void;
  disabled?: boolean;
};

const MAX_LIST = 60;

export function ClienteSearchSelect({ id, clientes, value, onChange, disabled }: Props) {
  const rootRef = useRef<HTMLDivElement>(null);
  const [query, setQuery] = useState('');
  const [open, setOpen] = useState(false);

  const selected = useMemo(
    () => (value === '' ? undefined : clientes.find((c) => c.clienteID === value)),
    [clientes, value],
  );

  const filtered = useMemo(() => {
    const t = query.trim().toLowerCase();
    const list = !t
      ? clientes
      : clientes.filter((c) => {
          const hay = `${c.nombre} ${c.apellido} ${c.cedula} ${c.email} ${c.telefono}`.toLowerCase();
          return hay.includes(t);
        });
    return list.slice(0, MAX_LIST);
  }, [clientes, query]);

  useEffect(() => {
    const onDoc = (e: MouseEvent) => {
      if (!rootRef.current?.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener('mousedown', onDoc);
    return () => document.removeEventListener('mousedown', onDoc);
  }, []);

  const pick = (c: Cliente) => {
    onChange(c.clienteID);
    setQuery('');
    setOpen(false);
  };

  const clear = () => {
    onChange('');
    setQuery('');
    setOpen(false);
  };

  return (
    <div className="cliente-search" ref={rootRef}>
      <div className="cliente-search__field">
        <input
          id={id}
          type="search"
          autoComplete="off"
          placeholder={
            selected
              ? 'Buscar otro cliente por nombre, cédula o correo…'
              : 'Escribe para buscar por nombre, cédula o correo…'
          }
          value={query}
          disabled={disabled}
          onChange={(e) => {
            setQuery(e.target.value);
            setOpen(true);
            if (value !== '') onChange('');
          }}
          onFocus={() => setOpen(true)}
          aria-expanded={open}
          aria-controls={id ? `${id}-listbox` : undefined}
          role="combobox"
        />
        {selected ? (
          <button type="button" className="cliente-search__clear" onClick={clear} disabled={disabled}>
            Quitar
          </button>
        ) : null}
      </div>

      {selected ? (
        <div className="cliente-search__chip" aria-live="polite">
          <span className="cliente-search__chip-name">
            {selected.nombre} {selected.apellido}
          </span>
          <span className="cliente-search__chip-meta">Cédula {selected.cedula}</span>
        </div>
      ) : null}

      {open && !disabled && filtered.length > 0 ? (
        <ul id={id ? `${id}-listbox` : undefined} className="cliente-search__list" role="listbox">
          {filtered.map((c) => (
            <li key={c.clienteID}>
              <button type="button" className="cliente-search__option" onClick={() => pick(c)}>
                <span className="cliente-search__option-name">
                  {c.nombre} {c.apellido}
                </span>
                <span className="cliente-search__option-sub">{c.cedula}</span>
              </button>
            </li>
          ))}
        </ul>
      ) : null}

      {open && !disabled && query.trim() && filtered.length === 0 ? (
        <div className="cliente-search__empty">No hay coincidencias.</div>
      ) : null}
    </div>
  );
}
