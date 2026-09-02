import { apiRequest } from './client';
import type { TokenResponse, User } from '../types';

export function login(email: string, password: string): Promise<TokenResponse> {
  return apiRequest<TokenResponse>('/login', {
    method: 'POST',
    auth: false,
    body: { email, password },
  });
}

export function logout(): Promise<void> {
  return apiRequest<void>('/logout', { method: 'POST' });
}

export function getMe(): Promise<User> {
  return apiRequest<User>('/users/me');
}
