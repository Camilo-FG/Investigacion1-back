export type Role = 'Admin' | 'Subscription_L1';

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
}

export interface User {
  id: string;
  email: string;
  role: Role;
  isActive: boolean;
  subscriptionExpirationDate: string;
}

export interface Room {
  id: string;
  number: string;
  type: string;
  floor: number;
  capacity: number;
  basePricePerNight: number;
}

export interface Reservation {
  id: string;
  roomId: string;
  roomNumber: string;
  roomType: string;
  roomFloor: number;
  roomCapacity: number;
  basePricePerNight: number;
  guestName: string;
  checkInDate: string;
  checkOutDate: string;
  guests: number;
  totalPrice: number;
}

export interface ApiErrorBody {
  error?: string;
}

export class ApiError extends Error {
  status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
  }
}
