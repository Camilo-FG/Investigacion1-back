import { useState, type FormEvent } from 'react';
import { createRoom } from '../api/domain';
import { ApiError, type Room } from '../types';
import { ErrorBanner } from '../components/ErrorBanner';

const ROOM_TYPES = ['Single', 'Double', 'Suite'] as const;

export function AdminRoomsPage() {
  const [error, setError] = useState<string | null>(null);
  const [created, setCreated] = useState<Room | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const [number, setNumber] = useState('');
  const [type, setType] = useState<string>('Double');
  const [floor, setFloor] = useState(1);
  const [capacity, setCapacity] = useState(2);
  const [basePricePerNight, setBasePricePerNight] = useState(120);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    setCreated(null);
    try {
      const room = await createRoom({
        number: number.trim(),
        type,
        floor,
        capacity,
        basePricePerNight,
      });
      setCreated(room);
      setNumber('');
    } catch (err) {
      if (err instanceof ApiError) {
        if (err.status === 403) {
          setError('403 — Solo Admin puede crear habitaciones.');
        } else if (err.status === 401) {
          setError(`401 — ${err.message}`);
        } else {
          setError(`${err.status} — ${err.message}`);
        }
      } else {
        setError('No se pudo crear la habitación.');
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="stack">
      <header className="section-head">
        <h1>Admin · Habitaciones</h1>
        <p>
          Command <code>POST /rooms</code> — solo rol <strong>Admin</strong>.
        </p>
      </header>

      <ErrorBanner message={error} onClose={() => setError(null)} />
      {created ? (
        <div className="success-banner">
          Creada {created.number} ({created.type}) — ${created.basePricePerNight}/noche
        </div>
      ) : null}

      <article className="panel panel--narrow">
        <form className="form" onSubmit={(e) => void onSubmit(e)}>
          <label>
            Número
            <input value={number} onChange={(e) => setNumber(e.target.value)} required placeholder="101" />
          </label>
          <label>
            Tipo
            <select value={type} onChange={(e) => setType(e.target.value)}>
              {ROOM_TYPES.map((t) => (
                <option key={t} value={t}>
                  {t}
                </option>
              ))}
            </select>
          </label>
          <div className="form-row">
            <label>
              Piso
              <input
                type="number"
                value={floor}
                onChange={(e) => setFloor(Number(e.target.value))}
                required
              />
            </label>
            <label>
              Capacidad
              <input
                type="number"
                min={1}
                value={capacity}
                onChange={(e) => setCapacity(Number(e.target.value))}
                required
              />
            </label>
          </div>
          <label>
            Precio base / noche
            <input
              type="number"
              min={1}
              step="0.01"
              value={basePricePerNight}
              onChange={(e) => setBasePricePerNight(Number(e.target.value))}
              required
            />
          </label>
          <button className="btn btn--primary" type="submit" disabled={submitting}>
            {submitting ? 'Creando…' : 'Crear habitación'}
          </button>
        </form>
      </article>
    </section>
  );
}
