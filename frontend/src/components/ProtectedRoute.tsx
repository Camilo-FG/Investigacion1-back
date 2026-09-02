import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';

export function ProtectedRoute() {
  const { isAuthenticated, loading } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <div className="page-center">
        <p className="muted">Cargando sesión…</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }

  return <Outlet />;
}

export function AdminRoute() {
  const { isAdmin, loading } = useAuth();

  if (loading) {
    return (
      <div className="page-center">
        <p className="muted">Cargando sesión…</p>
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <div className="panel panel--warn">
        <h2>Acceso restringido</h2>
        <p>Esta sección requiere rol <strong>Admin</strong>. Tu rol actual no tiene permiso (403).</p>
      </div>
    );
  }

  return <Outlet />;
}
