import { apiRequest } from './client';
import type { Room, Reservation } from '../types';

export function getRooms(): Promise<Room[]> {
  return apiRequest<Room[]>('/rooms');
}

export function createRoom(payload: {
  number: string;
  type: string;
  floor: number;
  capacity: number;
  basePricePerNight: number;
}): Promise<Room> {
  return apiRequest<Room>('/rooms', {
    method: 'POST',
    body: payload,
  });
}

export function getReservations(): Promise<Reservation[]> {
  return apiRequest<Reservation[]>('/reservations');
}

export function createReservation(payload: {
  roomId: string;
  guestName: string;
  checkInDate: string;
  checkOutDate: string;
  guests: number;
}): Promise<Reservation> {
  return apiRequest<Reservation>('/reservations', {
    method: 'POST',
    body: payload,
  });
}
