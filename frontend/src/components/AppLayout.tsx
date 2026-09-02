import { NavLink, Outlet } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

export function AppLayout() {
  const { user, isAdmin, logout } = useAuth();

  return (
    <div className="app-shell">
      <header className="topbar">
        <div className="brand">
          <span className="brand__mark">H</span>
          <div>
            <p className="brand__name">Harbor Stay</p>
            <p className="brand__tag">Investigación 1 · Hotel API</p>
          </div>
        </div>

        <nav className="nav">
          <NavLink to="/" end>
            Mi perfil
          </NavLink>
          <NavLink to="/rooms">Habitaciones</NavLink>
          {isAdmin ? <NavLink to="/reservations">Reservas</NavLink> : null}
          {isAdmin ? <NavLink to="/admin/rooms">Admin cuartos</NavLink> : null}
        </nav>

        <div className="session">
          <div className="session__meta">
            <span>{user?.email}</span>
            <span className="session__role">{user?.role}</span>
          </div>
          <button type="button" className="btn btn--ghost" onClick={() => void logout()}>
            Cerrar sesión
          </button>
        </div>
      </header>

      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
