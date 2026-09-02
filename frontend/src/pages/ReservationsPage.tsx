import { useEffect, useState } from 'react';
import { getReservations } from '../api/domain';
import { ApiError, type Reservation } from '../types';
import { ErrorBanner } from '../components/ErrorBanner';

export function ReservationsPage() {
  const [items, setItems] = useState<Reservation[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setLoading(true);
    setError(null);
    try {
      const data = await getReservations();
      setItems(data);
    } catch (err) {
      if (err instanceof ApiError) {
        if (err.status === 403) {
          setError('403 — Solo Admin puede listar reservas.');
        } else if (err.status === 401) {
          setError(`401 — ${err.message}`);
        } else {
          setError(`${err.status} — ${err.message}`);
        }
      } else {
        setError('No se pudieron cargar las reservas.');
      }
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  return (
    <section className="stack">
      <header className="section-head">
        <div>
          <h1>Reservas</h1>
          <p>
            Query <code>GET /reservations</code> (solo Admin) — lectura relacionada con habitación.
          </p>
        </div>
        <button type="button" className="btn btn--ghost" onClick={() => void load()}>
          Recargar
        </button>
      </header>

      <ErrorBanner message={error} onClose={() => setError(null)} />

      {loading ? <p className="muted">Cargando…</p> : null}

      {!loading && items.length === 0 && !error ? (
        <p className="muted">Aún no hay reservas.</p>
      ) : null}

      <div className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>Huésped</th>
              <th>Habitación</th>
              <th>Check-in</th>
              <th>Check-out</th>
              <th>Huéspedes</th>
              <th>Total</th>
            </tr>
          </thead>
          <tbody>
            {items.map((r) => (
              <tr key={r.id}>
                <td>{r.guestName}</td>
                <td>
                  {r.roomNumber} · {r.roomType}
                </td>
                <td>{new Date(r.checkInDate).toLocaleDateString()}</td>
                <td>{new Date(r.checkOutDate).toLocaleDateString()}</td>
                <td>{r.guests}</td>
                <td>${r.totalPrice}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  );
}
