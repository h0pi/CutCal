# CutCal

Appointment booking platform for hair and beauty salons.

## Architecture

- `backend/CutCal.Model` — DTOs, requests, responses, search objects, exceptions
- `backend/CutCal.Services` — database entities, `CutCalDbContext`, business services, state machine, validators
- `backend/CutCal.Common.Services` — `CryptoService` (password hashing)
- `backend/CutCal.WebAPI` — controllers, filters, JWT auth, Swagger
- `backend/CutCal.Worker` — separate microservice, consumes RabbitMQ and sends emails
- `UI/cutcal_mobile` — Flutter mobile app (Android)
- `UI/cutcal_desktop` — Flutter desktop app (Windows)

Two microservices only: `CutCal.WebAPI` and `CutCal.Worker`, sharing a single SQL Server database (`220240`).

## Running the backend

Prerequisites: Docker Desktop.

```bash
docker-compose up --build
```

This starts:

| Service   | URL / Port                          |
|-----------|--------------------------------------|
| API       | http://localhost:5194                |
| Swagger   | http://localhost:5194/swagger        |
| RabbitMQ  | http://localhost:15672 (guest/guest) |
| SQL Server| localhost:1433                       |

Database migrations are applied automatically on API startup, and seed data (salons, users, services, appointments, reviews) is inserted the first time the database is created.

Secrets are read from `.env` (see `.env.example`). Nothing is hardcoded in `appsettings.json`.

## Running the backend locally without Docker

```bash
cd backend
dotnet restore
dotnet run --project CutCal.WebAPI
```

Point `ConnectionStrings__DefaultConnection` / `RabbitMQ__Host` in `.env` at `localhost` instead of `db` / `rabbitmq` when running outside Docker.

## Flutter mobile app

```bash
cd UI/cutcal_mobile
flutter pub get
flutter run --dart-define=baseUrl=http://10.0.2.2:5194/
```

## Flutter desktop app

```bash
cd UI/cutcal_desktop
flutter pub get
flutter run -d windows --dart-define=baseUrl=http://localhost:5194/
```

## Test credentials

Every seeded account uses the password `test`.

| Kontekst | Korisničko ime | Lozinka |
|---|---|---|
| Desktop verzija (Admin) | `desktop` | `test` |
| Mobilna verzija (Customer) | `mobile` | `test` |
| Više korisničkih uloga: Admin | `admin` | `test` |
| Više korisničkih uloga: SalonManager (Business Mode u mobilnoj) | `manager` | `test` |
| Više korisničkih uloga: Customer | `customer` | `test` |
| Više korisničkih uloga: Staff | `staff1` | `test` |

Logging out invalidates the access token on the server (it is blacklisted until it would have expired) and deletes the refresh token, so a token copied before logout stops working.

## Notes / known deviations from the original spec

- Real-time notifications use polling (`GET /Notifications` every 30s from the client) as the primary implementation. A `// TODO: SignalR` marker is left on `NotificationsController` for a future push-based upgrade.
- Demo data: 15 salons (3 per category, across Sarajevo, Mostar, Banja Luka, Zagreb and Belgrade) with photos, 30 staff members (`staff1`..`staff30`), 10 customers (`customer`, `customer2`..`customer10`, plus `mobile`), about 140 historic appointments with 90 reviews (salon ratings are the real average of their reviews), and every salon is topped up to three upcoming appointments each time the API starts. All seeded accounts use the password `test`.
- `Users/{id}/ChangePassword` lives under the Admin-only `UsersController` per spec. A logged-in Customer changing their own password would need a "my profile" style endpoint, which is not in the original controller list — flagged here as a gap for a future iteration.
- Google Maps address→coordinate geocoding and the SignalR hub are left as explicit `// TODO`s in the Flutter apps, as called out in the spec.
