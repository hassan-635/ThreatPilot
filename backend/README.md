# ThreatPilot Backend

Welcome to the `ThreatPilot` backend service. This API is built with ASP.NET Core 8 and PostgreSQL, and it provides the core application server for ingesting security events, authenticating users, publishing alerts, and exposing real-time notifications via SignalR.

## What this backend contains

- `Program.cs` — application startup, dependency registration, JWT authentication, and database configuration
- `Data/ApplicationDbContext.cs` — Entity Framework Core context and model configuration
- `Controllers/` — REST API endpoints for alerts, authentication, and ingestion
- `Hubs/AlertHub.cs` — SignalR hub for live alert updates
- `Migrations/` — Entity Framework migrations for the PostgreSQL schema
- `.env` / `.env.example` — environment-based secret configuration

## Setup

1. Copy the example environment file:

```powershell
cd backend
copy .env.example .env
```

2. Open `backend/.env` and replace the placeholder values with your real secrets:

- `ConnectionStrings__DefaultConnection` — PostgreSQL connection string (with actual password)
- `Jwt__Key` — JWT signing secret
- `Jwt__Issuer` — JWT issuer name
- `Jwt__Audience` — JWT audience value

3. Make sure PostgreSQL is running and reachable at the configured host.

## Run the backend

From the `backend` folder, run:

```powershell
dotnet run
```

The backend will start in Development mode and listen on:

- `http://localhost:5229`

If you want HTTPS support, use the `https` profile from Visual Studio / `dotnet run` with launch settings.

## Database migrations

To apply migrations and update the database schema, run:

```powershell
dotnet ef database update
```

If you need to pass a custom connection string instead of using `.env`, use:

```powershell
dotnet ef database update --connection "Host=localhost;Database=threatpilot_db;Username=postgres;Password=YOUR_PASSWORD"
```

## Notes

- Do not commit `.env`; it contains sensitive values.
- Use `.env.example` as the template for new environments.
- If you change secrets, restart the backend so the new values are loaded.

## Helpful commands

```powershell
cd backend
copy .env.example .env
# edit .env
dotnet ef database update
dotnet run
```

Enjoy working with the backend service — it is now configured to run securely using environment-based secrets and should be easy to start locally.
