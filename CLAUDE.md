# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Clean Architecture template — .NET 10 backend API (`src/back/`) with PostgreSQL + React 18 frontend (`src/front/`). Authentication via **Auth0 JWT** (RS256 bearer token).

## Architecture

Four layers, all in a single `ShopApi.csproj`:

| Layer | Folder | Responsibility |
|---|---|---|
| Domain | `Domain/` | Entities, domain events, exceptions — no external dependencies |
| Application | `Application/` | CQRS handlers (MediatR), DTOs, FluentValidation validators, pipeline behaviors |
| Infrastructure | `Infrastructure/` | EF Core + PostgreSQL, repositories, Autofac DI module |
| API | `API/` | ASP.NET Core controllers, global error handler |

### Key patterns

- **CQRS via MediatR** — requests in `Application/[Feature]/Queries|Commands/`, dispatched from controllers via `IMediator`.
- **Pipeline behaviors** (in order): `LoggingBehavior` → `ValidationBehavior` → `TransactionBehaviour`.
- **Repository pattern** — `IRepository<T>` / `EfRepository<T>` base; feature interfaces extend it.
- **Autofac** — DI container; `InfrastructureModule` registers repositories and services.
- **DbContext strategies** — `IModelCreatingDbContextStrategy` and `IBeforeSavingDbContextStrategy` let features plug in without touching `AppDbContext`.
- **Domain events** — dispatched inside `AppDbContext.SaveChangesAsync` via `MediatorExtension.DispatchDomainEventsAsync`.
- **Dual DB mode** — `UseInMemoryDatabase` switches between in-memory (dev) and PostgreSQL (prod). `TransactionBehaviour` skips transactions for in-memory.

### Adding a feature

Use the `/add-feature` skill. It covers all layers end-to-end: Domain → Application → Infrastructure → Migration → API → Frontend.

## EF Core Conventions

- `AppDbContext` sets `QueryTrackingBehavior.NoTracking` globally — **never add `.AsNoTracking()` in repositories**. Use `.AsTracking()` only for write operations that need change tracking.
- `HasData` seed uses **anonymous types** (not `new Entity { ... }`) to be compatible with private setters. Nullable fields can be omitted — EF inserts `null` by default.

## Error Handling

Global handler in `API/Errors/CustomErrorHandlerHelper.cs`:

| Exception | HTTP |
|---|---|
| `NotFoundException` | 404 |
| `FunctionalException` | 400 |
| Unhandled | 500 |

Throw from handlers or domain objects — never catch in controllers.

## Authentication — Auth0 JWT

All endpoints use `[Authorize]`. Backend validates via `Authority` + `Audience`.

Configuration keys (required at startup — app throws if empty):

| Key | Example |
|---|---|
| `Auth0:Domain` | `dev-xxx.eu.auth0.com` |
| `Auth0:Audience` | `https://shop-api` |

Set in `appsettings.Development.json` (git-ignored). Copy from `appsettings.Development.example.json`. For first-time Auth0 dashboard setup, use the `/auth0-setup` skill.

### IIdentityService

Inject `IIdentityService` in any handler to access the current user:

- `GetUserIdentity()` — claim `sub` or `client_id`
- `GetUsername()` — claim `name` or `Identity.Name`, defaults to `"System"`
- `GetTraceId()` — `TraceIdentifier` or `Activity.Current`
- `GetRoles()` — claims of type `role`

### Audit fields (IAuditable)

`CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` are populated automatically by `MustHaveAuditableBeforeSavingStrategy` before each save. **Never set them manually** in entities or handlers. `CreatedBy`/`UpdatedBy` are set only on `Added` and `Modified` states — never on `Deleted`.

## Configuration

| Setting | Development | Production |
|---|---|---|
| `UseInMemoryDatabase` | `true` | `false` |
| `Auth0:Domain` / `Auth0:Audience` | `appsettings.Development.json` | Environment variable |
| `ConnectionStrings:DefaultConnection` | User Secrets | `ConnectionStrings__DefaultConnection` env var |
| Swagger | Enabled (`IsDevelopment()`) | Disabled |

## Security

- Rate limiting: 100 req/min per IP (fixed window) — HTTP 429 on excess.
- Security headers on every response: `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy`.
- CORS: set `AllowedOrigins` to exact URLs in production — no wildcard `*`.
- Never commit `appsettings.Development.json` or `src/front/.env.local` — both are git-ignored.

## Frontend Architecture

Stack: Vite + React 18 + TypeScript + shadcn/ui + TanStack Query + React Hook Form + Zod + @auth0/auth0-react.

| Folder | Responsibility |
|---|---|
| `src/auth/` | `AuthGuard` (redirect if not authenticated), `AxiosInterceptor` (attaches Bearer token) |
| `src/api/` | Axios wrappers per feature |
| `src/types/` | DTO interfaces mirroring backend shapes |
| `src/components/` | shadcn/ui primitives + Layout shell |
| `src/pages/` | One component per route |

Auth flow: `Auth0Provider` → `AuthGuard` → `AxiosInterceptor` → Routes. `AxiosInterceptor` uses a `ready` state to block API calls until the Bearer interceptor is registered — prevents race conditions on F5 refresh.

Frontend env vars go in `src/front/.env.local` (copy from `.env.example`): `VITE_AUTH0_DOMAIN`, `VITE_AUTH0_CLIENT_ID`, `VITE_AUTH0_AUDIENCE`, `VITE_API_URL`.

Error responses follow the `ProblemDetails` shape. `ValidationException` includes field-level messages under `extensions.validations[]`.

## CI

- Backend: `.github/workflows/back-ci.yml` — restore → build Release → Docker build, triggers on `src/back/**` changes.
- Frontend: `.github/workflows/front-ci.yml` — `npm ci` → typecheck → build, triggers on `src/front/**` changes.
- PR: `.github/workflows/pr-ci.yml` — runs both in parallel, `all-checks` job must pass before merge.

## Migrations

Run from `src/back/`:

```bash
dotnet ef migrations add <MigrationName> --project ShopApi.csproj
dotnet ef database update --project ShopApi.csproj   # PostgreSQL only
```

In-memory mode rebuilds the schema at startup — no migration needed.

## Coding Conventions

Mandatory across all C# and TypeScript code:

- **Lambda / callback params** — explicit, self-documenting names. No `x`, `e`, `r`, `v`, `i`, `item`, `obj`. → skills `dotnet-lambda-naming`, `react-lambda-naming`.
- **Method / function params** — fully spelled out. No `ct`, `req`, `cmd`, `opts`. → skill `parameter-naming`.
- **No `any` in TypeScript** — use concrete types, generics, or `unknown`. → skill `react-no-any`.
- **Unnecessary `using` directives** — remove usings already covered by implicit usings of `Microsoft.NET.Sdk.Web`. → skill `dotnet-remove-usings`.
