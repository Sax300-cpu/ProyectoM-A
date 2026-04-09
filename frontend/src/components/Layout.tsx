import { NavLink, Outlet } from 'react-router-dom';
import './Layout.css';

export function Layout() {
  return (
    <div className="layout">
      <header className="topbar">
        <div className="topbar__brand">
          <div className="topbar__title">Sistema de Ventas</div>
          <div className="topbar__subtitle">Clientes, productos y ventas</div>
        </div>
        <nav className="topbar__nav">
          <NavLink to="/" end className={({ isActive }) => (isActive ? 'navlink navlink--active' : 'navlink')}>
            Nueva venta
          </NavLink>
          <NavLink to="/historial" className={({ isActive }) => (isActive ? 'navlink navlink--active' : 'navlink')}>
            Historial
          </NavLink>
          <NavLink to="/clientes" className={({ isActive }) => (isActive ? 'navlink navlink--active' : 'navlink')}>
            Clientes
          </NavLink>
          <NavLink to="/productos" className={({ isActive }) => (isActive ? 'navlink navlink--active' : 'navlink')}>
            Productos
          </NavLink>
        </nav>
      </header>

      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}

