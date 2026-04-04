# dotnet-react-clean-architecture

A production-ready starter template combining a **.NET 10 Clean Architecture API** with a **React 18 frontend** — batteries included.

[![Back-end CI](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/back-ci.yml/badge.svg)](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/back-ci.yml)
[![Front-end CI](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/front-ci.yml/badge.svg)](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/front-ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## Stack

| Layer | Technology |
|---|---|
| **API** | .NET 10 · ASP.NET Core · MediatR (CQRS) · FluentValidation · Autofac |
| **Database** | PostgreSQL 17 · EF Core (code-first migrations) |
| **Frontend** | React 18 · TypeScript · Vite · shadcn/ui · TanStack Query · React Hook Form · Zod |
| **Infra** | Docker · Docker Compose · nginx |
| **CI** | GitHub Actions |

---

## Architecture

The backend follows **Clean Architecture** with four layers in a single project:

```
Domain         — Entities, domain events, value objects. Zero external dependencies.
Application    — CQRS handlers (MediatR), DTOs, validators (FluentValidation).
Infrastructure — EF Core + PostgreSQL, repositories, Autofac DI module.
API            — ASP.NET Core controllers, global error handler.
```

Key patterns: CQRS via MediatR · Repository pattern · Pipeline behaviors (logging → validation → transaction) · Domain events · Strategy pattern on DbContext.

The frontend is a lightweight React SPA:

```
src/api/        — Axios wrappers, type guards (isApiError)
src/types/      — DTO interfaces mirroring backend response shapes
src/components/ — shadcn/ui primitives + Layout shell
src/pages/      — CatalogPage · CatalogItemPage · CreateItemPage
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run with Docker (recommended)

```bash
# Production — postgres + API + React served by nginx on :80
docker-compose up --build

# Development — hot reload for both API (dotnet watch) and frontend (Vite)
docker-compose up
```

| Service | URL |
|---|---|
| Frontend | http://localhost |
| API (prod) | http://localhost:8080 |
| API (dev) | http://localhost:5080 |
| Swagger UI | http://localhost:8080 or http://localhost:5080 |
| PostgreSQL | localhost:5432 |

### Run locally

**Backend:**

```bash
cd src/back
dotnet restore
dotnet watch run   # hot reload on http://localhost:5080, in-memory DB
```

**Frontend:**

```bash
cd src/front
npm install
npm run dev        # http://localhost:3000, proxies /api → :5080
```

**VS Code:** use the **"Full Stack"** compound launch configuration to start both with a single `F5`.

---

## Project Structure

```
/
├── docker-compose.yml          # Production stack (postgres + API + front)
├── docker-compose.override.yml # Dev overrides (hot reload)
├── src/
│   ├── back/                   # .NET API
│   │   ├── API/                # Controllers, error handler
│   │   ├── Application/        # CQRS handlers, DTOs, validators
│   │   ├── Domain/             # Entities, domain events
│   │   ├── Infrastructure/     # EF Core, repositories, Autofac module
│   │   ├── Dockerfile
│   │   └── Dockerfile.dev
│   └── front/                  # React SPA
│       ├── src/
│       │   ├── api/
│       │   ├── components/
│       │   ├── pages/
│       │   └── types/
│       ├── Dockerfile
│       ├── Dockerfile.dev
│       └── nginx.conf
└── .github/
    └── workflows/
        ├── back-ci.yml         # Backend CI (build + Docker)
        ├── front-ci.yml        # Frontend CI (typecheck + build + Docker)
        └── pr-ci.yml           # PR quality gate (both stacks, required check)
```

---

## Database Migrations

```bash
cd src/back

# Add a migration
dotnet ef migrations add <MigrationName>

# Apply to the database
dotnet ef database update
```

> In `Development` mode (`UseInMemoryDatabase=true`) migrations are not applied — the in-memory DB is seeded on first run automatically.

---

## Adding a Feature

1. Add entity / value object in `Domain/`
2. Add query or command + handler + validator in `Application/<Feature>/`
3. Add EF configuration in `Infrastructure/Data/Configurations/`
4. Add repository interface + implementation in `Infrastructure/Repositories/`
5. Register in `Infrastructure/AutofacModules/InfrastructureModule.cs`
6. Add controller action in `API/Controllers/`

---

## CI / CD

| Workflow | Trigger | Jobs |
|---|---|---|
| `back-ci.yml` | Push / PR on `src/back/**` | Restore → Build → Docker build |
| `front-ci.yml` | Push / PR on `src/front/**` | Install → Typecheck → Build → Docker build |
| `pr-ci.yml` | Every PR to `master` / `develop` | Back + Front in parallel → `all-checks` gate |

Set **`all-checks`** (from `pr-ci.yml`) as the required status check in your branch protection rules.

---

## License

[MIT](LICENSE) — free to use as a base for any project, commercial or otherwise.
