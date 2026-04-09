import { Navigate, Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ClientesPage } from './pages/ClientesPage';
import { ProductosPage } from './pages/ProductosPage';
import { VentasPage } from './pages/VentasPage';
import { HistorialVentasPage } from './pages/HistorialVentasPage';
import './App.css';

function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<VentasPage />} />
        <Route path="/historial" element={<HistorialVentasPage />} />
        <Route path="/clientes" element={<ClientesPage />} />
        <Route path="/productos" element={<ProductosPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Route>
    </Routes>
  )
}

export default App
