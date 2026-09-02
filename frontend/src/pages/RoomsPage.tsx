import { useEffect, useState, type FormEvent } from 'react';
import { createReservation, getRooms } from '../api/domain';
import { ApiError, type Room } from '../types';
import { ErrorBanner } from '../components/ErrorBanner';

export function RoomsPage() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const [roomId, setRoomId] = useState('');
  const [guestName, setGuestName] = useState('');
  const [checkInDate, setCheckInDate] = useState('');
  const [checkOutDate, setCheckOutDate] = useState('');
  const [guests, setGuests] = useState(1);
  const [submitting, setSubmitting] = useState(false);

  async function loadRooms() {
    setLoading(true);
    setError(null);
    try {
      const data = await getRooms();
      setRooms(data);
      if (!roomId && data.length > 0) {
        setRoomId(data[0].id);
      }
    } catch (err) {
      if (err instanceof ApiError) {
        setError(formatApiError(err));
      } else {
        setError('No se pudieron cargar las habitaciones.');
      }
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadRooms();
  }, []);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    setSuccess(null);
    try {
      const reservation = await createReservation({
        roomId,
        guestName: guestName.trim(),
        checkInDate: new Date(checkInDate).toISOString(),
        checkOutDate: new Date(checkOutDate).toISOString(),
        guests,
      });
      setSuccess(
        `Reserva creada: ${reservation.guestName} · cuarto ${reservation.roomNumber} · total $${reservation.totalPrice}`,
      );
      setGuestName('');
    } catch (err) {
      if (err instanceof ApiError) {
        setError(formatApiError(err));
      } else {
        setError('No se pudo crear la reserva.');
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="stack">
      <header className="section-head">
        <h1>Habitaciones</h1>
        <p>
          Consulta <code>GET /rooms</code> y crea una reserva con <code>POST /reservations</code>.
        </p>
      </header>

      <ErrorBanner message={error} onClose={() => setError(null)} />
      {success ? <div className="success-banner">{success}</div> : null}

      <div className="split">
        <article className="panel">
          <div className="panel__head">
            <h2>Disponibles</h2>
            <button type="button" className="btn btn--ghost" onClick={() => void loadRooms()}>
              Recargar
            </button>
          </div>

          {loading ? <p className="muted">Cargando…</p> : null}
          {!loading && rooms.length === 0 ? (
            <p className="muted">No hay habitaciones. Un Admin puede crearlas en “Admin cuartos”.</p>
          ) : null}

          <ul className="room-list">
            {rooms.map((room) => (
              <li key={room.id}>
                <button
                  type="button"
                  className={`room-card ${roomId === room.id ? 'room-card--active' : ''}`}
                  onClick={() => setRoomId(room.id)}
                >
                  <strong>
                    {room.number} · {room.type}
                  </strong>
                  <span>
                    Piso {room.floor} · Cap. {room.capacity} · ${room.basePricePerNight}/noche
                  </span>
                </button>
              </li>
            ))}
          </ul>
        </article>

        <article className="panel">
          <h2>Nueva reserva</h2>
          <form className="form" onSubmit={(e) => void onSubmit(e)}>
            <label>
              Habitación
              <select value={roomId} onChange={(e) => setRoomId(e.target.value)} required>
                <option value="" disabled>
                  Selecciona…
                </option>
                {rooms.map((room) => (
                  <option key={room.id} value={room.id}>
                    {room.number} ({room.type})
                  </option>
                ))}
              </select>
            </label>
            <label>
              Huésped
              <input value={guestName} onChange={(e) => setGuestName(e.target.value)} required />
            </label>
            <div className="form-row">
              <label>
                Check-in
                <input
                  type="date"
                  value={checkInDate}
                  onChange={(e) => setCheckInDate(e.target.value)}
                  required
                />
              </label>
              <label>
                Check-out
                <input
                  type="date"
                  value={checkOutDate}
                  onChange={(e) => setCheckOutDate(e.target.value)}
                  required
                />
              </label>
            </div>
            <label>
              Huéspedes
              <input
                type="number"
                min={1}
                value={guests}
                onChange={(e) => setGuests(Number(e.target.value))}
                required
              />
            </label>
            <button className="btn btn--primary" type="submit" disabled={submitting || !roomId}>
              {submitting ? 'Reservando…' : 'Crear reserva'}
            </button>
          </form>
        </article>
      </div>
    </section>
  );
}

function formatApiError(err: ApiError): string {
  if (err.status === 401) {
    return `401 — ${err.message}`;
  }
  if (err.status === 403) {
    return `403 — ${err.message}`;
  }
  return `${err.status} — ${err.message}`;
}
