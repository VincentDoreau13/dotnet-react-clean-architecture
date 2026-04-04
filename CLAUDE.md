# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Clean Architecture template — .NET 10 backend API (`src/back/`) with PostgreSQL + React 18 frontend (`src/front/`) with shadcn/ui.

## Commands

### Frontend (`src/front/`)

```bash
npm install        # install deps (first time)
npm run dev        # dev server on http://localhost:3000 (proxies /api → :5080)
npm run build      # production build
```

The Vite dev server proxies all `/api` requests to the .NET API at `http://localhost:5080`.

### Backend — all commands run from `src/back/`:

```bash
# Restore & build
dotnet restore
dotnet build

# Run with hot reload (uses in-memory DB by default in Development)
dotnet watch run

# Docker — production (PostgreSQL)
docker-compose up --build

# Docker — development with hot reload
docker-compose -f docker-compose.yml -f docker-compose.override.yml up

# Migrations
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

The VSCode launch profile "Back — ShopApi (Development)" runs with `UseInMemoryDatabase=true` on `https://localhost:7080` / `http://localhost:5080`. Swagger UI is served at `/`.

## Architecture

Four layers, all in a single `ShopApi.csproj` (no separate class library projects):

| Layer | Folder | Responsibility |
|---|---|---|
| Domain | `Domain/` | Entities, domain events, enumerations, exceptions — no external dependencies |
| Application | `Application/` | CQRS handlers (MediatR), DTOs, FluentValidation validators, pipeline behaviors |
| Infrastructure | `Infrastructure/` | EF Core + PostgreSQL, repositories, Autofac DI module |
| API | `API/` | ASP.NET Core controllers, global error handler |

### Key patterns

- **CQRS via MediatR** — requests in `Application/[Feature]/Queries|Commands/`, dispatched from controllers via `IMediator`.
- **MediatR pipeline behaviors** (registered in order): `LoggingBehavior` → `ValidationBehavior` → `TransactionBehaviour`.
- **Repository pattern** — `IRepository<T>` / `EfRepository<T>` base; feature-specific interfaces (e.g. `ICatalogRepository`) extend it.
- **Autofac** is the DI container; `InfrastructureModule` (`Infrastructure/AutofacModules/`) registers repositories and services.
- **Strategy pattern on DbContext** — `IModelCreatingDbContextStrategy` and `IBeforeSavingDbContextStrategy` let features plug into EF model building and save logic without modifying `AppDbContext`.
- **Domain events** — dispatched inside `AppDbContext.SaveChangesAsync` via `MediatorExtension.DispatchDomainEventsAsync`.
- **Dual DB mode** — `UseInMemoryDatabase` appsetting switches between in-memory (dev/test) and PostgreSQL (production). `TransactionBehaviour` skips transactions for in-memory.

### Adding a new feature

1. Add entity/value object in `Domain/`.
2. Add query/command + handler + validator in `Application/<Feature>/`.
3. Add EF configuration in `Infrastructure/Data/Configurations/`.
4. Add repository interface + implementation in `Infrastructure/Repositories/`.
5. Register new types in `Infrastructure/AutofacModules/InfrastructureModule.cs`.
6. Add controller action in `API/Controllers/`.

## EF Core Conventions

`AppDbContext` sets `ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking` globally — **ne jamais ajouter `.AsNoTracking()` dans les repositories**, c'est redondant. Utiliser `.AsTracking()` explicitement uniquement pour les opérations d'écriture qui nécessitent le tracking.

`HasData` pour le seed utilise des **types anonymes** (pas `new CatalogItem { ... }`) afin d'être compatible avec les setters privés des entités.

## Error Handling

Global error handler is in `API/Errors/CustomErrorHandlerHelper.cs`, registered via `ApplicationBuilderExtensions`. It maps:
- `NotFoundException` → 404
- `FunctionalException` → 400 (with validation-style error payload)
- Unhandled → 500

## Configuration

| Setting | Development | Production |
|---|---|---|
| `UseInMemoryDatabase` | `true` | `false` |
| Database | In-memory | PostgreSQL (`Host=postgres;Database=shop_api`) |
| Swagger | Enabled | Enabled (Docker env) |

## CI

`.github/workflows/back-ci.yml` triggers on push/PR to `master`/`develop` when `src/back/**` changes. Steps: restore → build Release → Docker build (no push).

No frontend CI workflow exists yet. When adding one, the minimum steps should be:

```bash
cd src/front
npm ci
npm run typecheck
npm run build
```

## Frontend Architecture

Stack: Vite + React 18 + TypeScript + shadcn/ui (Tailwind CSS + Radix UI) + TanStack Query + React Hook Form + Zod.

| Layer | Folder | Responsibility |
|---|---|---|
| API client | `src/api/` | Axios wrappers for each backend endpoint |
| Types | `src/types/` | DTO types mirroring the backend response shapes |
| UI primitives | `src/components/ui/` | shadcn/ui components (button, card, table, form, …) |
| Layout | `src/components/Layout.tsx` | Shell with top nav |
| Pages | `src/pages/` | CatalogPage, CatalogItemPage, CreateItemPage |

The Vite dev server proxies `/api` → `$VITE_API_URL` (default `http://localhost:5080`). Copy `.env.example` to `.env.local` to override.

Error responses from the API follow the ASP.NET Core `ProblemDetails` shape. `ValidationException` errors include field-level messages under `extensions.validations[]`.
