import { VentasForm } from '../components/VentasForm';
import './Page.css';

export function VentasPage() {
  return (
    <div className="page">
      <div className="page__header">
        <div>
          <h1 className="page__title">Venta</h1>
        </div>
      </div>
      <VentasForm />
    </div>
  );
}

