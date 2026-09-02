import { useState, type FormEvent } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { ErrorBanner } from '../components/ErrorBanner';

export function LoginPage() {
  const { login, isAuthenticated, loading, error, clearError } = useAuth();
  const location = useLocation();
  const [email, setEmail] = useState('admin@example.com');
  const [password, setPassword] = useState('AdminPass1');
  const [submitting, setSubmitting] = useState(false);

  const from = (location.state as { from?: string } | null)?.from ?? '/';

  if (!loading && isAuthenticated) {
    return <Navigate to={from} replace />;
  }

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    try {
      await login(email.trim(), password);
    } catch {
      // error already surfaced via AuthContext
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="login-page">
      <div className="login-panel">
        <div className="login-panel__brand">
          <span className="brand__mark brand__mark--lg">H</span>
          <h1>Harbor Stay</h1>
          <p>Inicia sesión para gestionar reservas y tu suscripción.</p>
        </div>

        <ErrorBanner message={error} onClose={clearError} />

        <form className="form" onSubmit={(e) => void onSubmit(e)}>
          <label>
            Email
            <input
              type="email"
              autoComplete="username"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </label>

          <label>
            Contraseña
            <input
              type="password"
              autoComplete="current-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              minLength={6}
            />
          </label>

          <button className="btn btn--primary" type="submit" disabled={submitting}>
            {submitting ? 'Entrando…' : 'Entrar'}
          </button>
        </form>

        <p className="hint">
          Demo: <code>admin@example.com</code> / <code>AdminPass1</code> (seed al arrancar la API).
          Credenciales inválidas o <code>IsActive=false</code> → 401.
        </p>
      </div>
    </div>
  );
}
