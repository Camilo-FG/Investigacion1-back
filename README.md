# Investigación 1 — Backend (Autenticación + RBAC)

Backend en **.NET 10 (minimal APIs)** con arquitectura **Vertical Slice** y separación **Command/Query (CQRS)**. Persistencia con **EF Core + PostgreSQL**.

## Estructura

```
Features/
├── Auth/                  # Persona 1 — autenticación
│   ├── Login, Register, AdminRegister, Refresh, Logout
└── Users/                 # Persona 2 — RBAC y reglas de negocio
    ├── GetMe                    (Query)   GET /users/me
    ├── GetUsers                 (Query)   GET /users
    ├── GetUserById              (Query)   GET /users/{id}
    ├── UpdateUserStatus         (Command) PATCH /users/{id}/status
    └── UpdateSubscriptionExpiration (Command) PATCH /users/{id}/subscription-expiration
```

## Autenticación (Persona 1)

| Método | Ruta              | Acceso          | Descipción                          |
| ------ | ----------------- | --------------- | ----------------------------------- |
| POST   | `/register`       | Público         | Registra un `Subscription_L1`.      |
| POST   | `/login`          | Público         | Emite accessToken + refreshToken.   |
| POST   | `/admin/register` | 1er Admin o Auth | Crea un `Admin`.                    |
| POST   | `/refresh`        | Refresh token   | Rota refresh y emite nuevo access.  |
| POST   | `/logout`         | Autenticado     | Revoca las sesiones refresh.        |

Roles válidos: `Admin` y `Subscription_L1`. No existe endpoint para cambiar el rol de un usuario.

## Endpoints de usuarios — RBAC (Persona 2)

| Método | Ruta                                 | Acceso                        | Regla de negocio aplicada                                         |
| ------ | ------------------------------------ | ----------------------------- | ----------------------------------------------------------------- |
| GET    | `/users/me`                          | Cualquier usuario autenticado | Devuelve los datos del usuario del token.                         |
| GET    | `/users`                             | Solo `Admin`                  | Lista todos los usuarios.                                         |
| GET    | `/users/{id}`                        | Solo `Admin`                  | Consulta un usuario por id; `404 User not found` si no existe.    |
| PATCH  | `/users/{id}/status`                 | Solo `Admin`                  | Activa/desactiva un usuario. Véanse reglas especiales abajo.      |
| PATCH  | `/users/{id}/subscription-expiration`| Solo `Admin`                  | Extiende/modifica la expiración de una suscripción `Subscription_L1`. |

## Reglas de negocio

- Endpoint protegido sin token válido → **401 Unauthorized**.
- Token válido pero rol insuficiente → **403 Forbidden**.
- `Subscription_L1` con `SubscriptionExpirationDate` vencida → **403** en cualquier endpoint autenticado (middleware global, no endpoint por endpoint).
- Un Admin **no puede desactivarse a sí mismo** (`PATCH /users/{id}/status` con su propio id → **403**).
- Un Admin **no puede desactivar al último Admin activo** del sistema (**403**).
- La ruta de activar/desactivar es **independiente** de la ruta de expiración de suscripción.
- No existe endpoint para cambiar el rol de un usuario.

## Formato de error

Todos los errores usan un `ErrorResponse` consistente: `{ "error": "<mensaje>" }`.

## Configuración

Copiar `appsettings.Example.json` a `appsettings.json` (o usar variables de entorno) y definir las claves `ConnectionStrings:DefaultConnection` y `Jwt:Secret` (mínimo 32 caracteres).

```bash
dotnet run
```