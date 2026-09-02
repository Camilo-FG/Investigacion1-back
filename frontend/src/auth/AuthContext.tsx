import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import * as authApi from '../api/auth';
import { ApiError } from '../types';
import type { User } from '../types';
import { tokenStorage } from './tokenStorage';

interface AuthContextValue {
  user: User | null;
  loading: boolean;
  error: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshProfile: () => Promise<void>;
  clearError: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refreshProfile = useCallback(async () => {
    const access = tokenStorage.getAccessToken();
    if (!access) {
      setUser(null);
      return;
    }

    try {
      const me = await authApi.getMe();
      setUser(me);
      setError(null);
    } catch (err) {
      tokenStorage.clear();
      setUser(null);
      if (err instanceof ApiError) {
        setError(err.message);
      }
    }
  }, []);

  useEffect(() => {
    void (async () => {
      setLoading(true);
      await refreshProfile();
      setLoading(false);
    })();
  }, [refreshProfile]);

  const login = useCallback(async (email: string, password: string) => {
    setError(null);
    try {
      const tokens = await authApi.login(email, password);
      tokenStorage.setTokens(tokens.accessToken, tokens.refreshToken);
      const me = await authApi.getMe();
      setUser(me);
    } catch (err) {
      tokenStorage.clear();
      setUser(null);
      if (err instanceof ApiError) {
        if (err.status === 401) {
          setError('Credenciales inválidas o cuenta inactiva.');
        } else if (err.status === 403) {
          setError(err.message || 'Acceso denegado (403).');
        } else {
          setError(err.message);
        }
      } else {
        setError('No se pudo conectar con el servidor.');
      }
      throw err;
    }
  }, []);

  const logout = useCallback(async () => {
    try {
      if (tokenStorage.getAccessToken()) {
        await authApi.logout();
      }
    } catch {
      // still clear local session
    } finally {
      tokenStorage.clear();
      setUser(null);
      setError(null);
    }
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      loading,
      error,
      isAuthenticated: user !== null,
      isAdmin: user?.role === 'Admin',
      login,
      logout,
      refreshProfile,
      clearError: () => setError(null),
    }),
    [user, loading, error, login, logout, refreshProfile],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return ctx;
}
