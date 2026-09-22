# CutCal

Appointment booking platform for hair and beauty salons.

## Architecture

- `backend/CutCal.Model` - DTOs, requests, responses, search objects, exceptions and constants (role, appointment-state and payment names)
- `backend/CutCal.Services` - database entities, `CutCalDbContext`, migrations and seed, business services, state machine, FluentValidation validators
- `backend/CutCal.Common.Services` - `CryptoService` (BCrypt password hashing)
- `backend/CutCal.Messaging` - RabbitMQ topology, message contract and retry policy shared by the API and the worker
- `backend/CutCal.WebAPI` - controllers, filters (exceptions, validation), JWT auth, Swagger, static images
- `backend/CutCal.Worker` - separate microservice (own container): consumes RabbitMQ and sends the e-mails
- `UI/cutcal_mobile` - Flutter mobile app (Android): customers, plus Business Mode for salon managers
- `UI/cutcal_desktop` - Flutter desktop app (Windows): administration and reports

Two services share one SQL Server database (`220240`) and talk through RabbitMQ: the API publishes a message after every appointment or payment event, the worker turns it into an e-mail.
If the worker fails to send a message it retries with exponential backoff (1 s, 2 s, 4 s, 8 s) and then moves it to the dead-letter queue `cutcal.notifications.email.dead`, where it can be inspected in the RabbitMQ management UI.

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

All configuration (JWT key, SMTP, PayPal, RabbitMQ credentials, geocoding) is read from `.env` (see `.env.example`); nothing is hardcoded in `appsettings.json`.
`docker-compose` only overrides the two values that differ inside containers (database connection string and RabbitMQ host), so the same `.env` works for Docker and for running locally. The compose file waits for SQL Server and RabbitMQ health checks before starting the API and the worker, and image tags are pinned.

## Running the backend locally without Docker

Requires SQL Server LocalDB (or change `ConnectionStrings__DefaultConnection` in `.env`) and, for e-mails, a local RabbitMQ on `localhost:5672`. Without RabbitMQ the API still works; the e-mail messages are just not delivered (the failure is logged).

```bash
cd backend
dotnet run --project CutCal.WebAPI --urls http://localhost:5194
dotnet run --project CutCal.Worker
```

## Flutter mobile app

```bash
cd UI/cutcal_mobile
flutter pub get
flutter run --dart-define=API_BASE_URL=http://10.0.2.2:5194
```

The API address is read with `String.fromEnvironment('API_BASE_URL')` (a trailing slash is optional). Defaults: `http://10.0.2.2:5194` on mobile (Android emulator) and `http://localhost:5194` on desktop.

## Flutter desktop app

```bash
cd UI/cutcal_desktop
flutter pub get
flutter run -d windows --dart-define=API_BASE_URL=http://localhost:5194
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

## Notes

- Notifications are refreshed by polling (`GET /Notifications`), not SignalR.
- Server-side validation: every request object has a FluentValidation validator that runs before the action; errors state the expected format (for example the phone number pattern) and are shown to the user.
- Recommender: see `recommender-dokumentacija.md`.
- Demo data: 15 salons (3 per category, across Sarajevo, Mostar, Banja Luka, Zagreb and Belgrade) with photos, 30 staff members (`staff1`..`staff30`), 11 customers (`customer`, `customer2`..`customer10`, plus `mobile`), about 140 historic appointments with 90 reviews (salon ratings are the real average of their reviews), and every salon is topped up to three upcoming appointments each time the API starts. All seeded accounts use the password `test`.
- Salon and staff images are served by the API from `wwwroot/images` (credits in `wwwroot/images/CREDITS.md`).
