# Harbor Stay — Frontend (Persona 4)

React + Vite + TypeScript. Consume la API de Investigación 1.

## Requisitos

- API corriendo en `http://localhost:5018` (ver README raíz)
- Node 20+

## Arranque

```bash
cp .env.example .env
npm install
npm run dev
```

Abre `http://localhost:5173`.

## Qué incluye

- Login + almacenamiento de tokens
- Refresh automático en 401
- Perfil (`GET /users/me`) con UI según rol
- Habitaciones / reservas (dominio Hotel)
- Banner de errores 401 / 403 / credenciales inválidas
