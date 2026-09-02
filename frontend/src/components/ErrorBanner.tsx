import type { ReactNode } from 'react';

interface ErrorBannerProps {
  message: string | null;
  onClose?: () => void;
}

export function ErrorBanner({ message, onClose }: ErrorBannerProps) {
  if (!message) {
    return null;
  }

  return (
    <div className="error-banner" role="alert">
      <span>{message}</span>
      {onClose ? (
        <button type="button" className="error-banner__close" onClick={onClose} aria-label="Cerrar">
          ×
        </button>
      ) : null}
    </div>
  );
}

interface StatusBadgeProps {
  children: ReactNode;
  tone?: 'ok' | 'warn' | 'muted';
}

export function StatusBadge({ children, tone = 'muted' }: StatusBadgeProps) {
  return <span className={`status-badge status-badge--${tone}`}>{children}</span>;
}
