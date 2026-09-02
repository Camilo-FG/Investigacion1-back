import { useAuth } from '../auth/AuthContext';
import { StatusBadge } from '../components/ErrorBanner';

export function ProfilePage() {
  const { user, isAdmin } = useAuth();

  if (!user) {
    return null;
  }

  const expired = new Date(user.subscriptionExpirationDate) < new Date();

  return (
    <section className="stack">
      <header className="section-head">
        <h1>Mi perfil</h1>
        <p>Respuesta de <code>GET /users/me</code> — vista protegida con el accessToken.</p>
      </header>

      <div className="profile-grid">
        <article className="panel">
          <h2>Cuenta</h2>
          <dl className="meta-list">
            <div>
              <dt>Email</dt>
              <dd>{user.email}</dd>
            </div>
            <div>
              <dt>Rol</dt>
              <dd>
                <StatusBadge tone={isAdmin ? 'ok' : 'muted'}>{user.role}</StatusBadge>
              </dd>
            </div>
            <div>
              <dt>Estado</dt>
              <dd>
                <StatusBadge tone={user.isActive ? 'ok' : 'warn'}>
                  {user.isActive ? 'Activo' : 'Inactivo'}
                </StatusBadge>
              </dd>
            </div>
            <div>
              <dt>Expiración suscripción</dt>
              <dd>
                {new Date(user.subscriptionExpirationDate).toLocaleString()}{' '}
                {expired ? <StatusBadge tone="warn">Vencida → 403</StatusBadge> : null}
              </dd>
            </div>
            <div>
              <dt>Id</dt>
              <dd>
                <code>{user.id}</code>
              </dd>
            </div>
          </dl>
        </article>

        <article className="panel">
          <h2>Permisos según rol</h2>
          <ul className="perm-list">
            <li className="perm-list__item perm-list__item--ok">Ver habitaciones y crear reservas</li>
            <li className={`perm-list__item ${isAdmin ? 'perm-list__item--ok' : 'perm-list__item--blocked'}`}>
              Listar reservas (<code>GET /reservations</code>) — solo Admin
            </li>
            <li className={`perm-list__item ${isAdmin ? 'perm-list__item--ok' : 'perm-list__item--blocked'}`}>
              Crear habitaciones (<code>POST /rooms</code>) — solo Admin
            </li>
            <li className={`perm-list__item ${isAdmin ? 'perm-list__item--ok' : 'perm-list__item--blocked'}`}>
              Gestionar usuarios / status / expiración — solo Admin
            </li>
          </ul>
          {!isAdmin ? (
            <p className="muted">
              Las rutas de administración están ocultas en el menú. Si las fuerzas por URL, verás el bloqueo 403.
            </p>
          ) : (
            <p className="muted">Sesión Admin: menú completo habilitado.</p>
          )}
        </article>
      </div>
    </section>
  );
}
