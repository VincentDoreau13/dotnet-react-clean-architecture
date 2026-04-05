# dotnet-react-clean-architecture

A production-ready starter template combining a **.NET 10 Clean Architecture API** with a **React 18 frontend**, built with [Claude Code](https://claude.ai/code).

[![Back-end CI](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/back-ci.yml/badge.svg)](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/back-ci.yml)
[![Front-end CI](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/front-ci.yml/badge.svg)](https://github.com/VincentDoreau13/dotnet-react-clean-architecture/actions/workflows/front-ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)


## Purpose

This template demonstrates how to build and evolve a full-stack application using **Claude Code as the primary development tool**. The codebase serves as a reference for Clean Architecture on .NET with a React frontend — every feature is implemented and reviewed through slash commands, not by hand.


## Architecture

### Backend — Clean Architecture (.NET 10)

Four layers in a single project (`src/back/`):

| Layer | Folder | Responsibility |
|---|---|---|
| Domain | `Domain/` | Entities, domain events — no external dependencies |
| Application | `Application/` | CQRS handlers (MediatR), DTOs, FluentValidation validators |
| Infrastructure | `Infrastructure/` | EF Core + PostgreSQL, repositories, Autofac DI |
| API | `API/` | ASP.NET Core controllers, global error handler |

Key patterns: CQRS via MediatR · Repository pattern · Pipeline behaviors (logging → validation → transaction) · Domain events dispatched inside `SaveChangesAsync`.

### Frontend — React 18 SPA (`src/front/`)

| Folder | Responsibility |
|---|---|
| `src/api/` | Axios wrappers per feature, shared `apiClient` |
| `src/auth/` | Auth0 guard (`AuthGuard`) and Axios token interceptor (`AxiosInterceptor`) |
| `src/types/` | DTO interfaces mirroring backend response shapes |
| `src/components/` | shadcn/ui primitives + Layout shell |
| `src/pages/` | One page component per route |

Stack: Vite · TypeScript · shadcn/ui · TanStack Query · React Hook Form · Zod · Auth0 React SDK.


## Security

### Authentication — Auth0 (RS256 JWT)

All API endpoints require a valid JWT issued by Auth0. The backend validates the token via `Authority` + `Audience`; the frontend acquires it with `getAccessTokenSilently` and attaches it as a `Bearer` header via an Axios interceptor.

The `IIdentityService` interface exposes the current user's identity, name, trace ID, and roles from the JWT claims — without any coupling to HTTP or Auth0 specifics.

### Audit fields

Every entity that implements `IAuditable` automatically gets `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` populated before each save via an EF Core strategy. No controller or handler needs to set these manually.

### Rate limiting

A fixed-window rate limiter (100 requests / minute per IP) is applied to all API endpoints. Exceeding the limit returns HTTP 429.

### Security headers

Every response includes `X-Content-Type-Options`, `X-Frame-Options`, and `Referrer-Policy` headers.


## Environment setup

### Auth0 (first time)

Run `/auth0-setup` in Claude Code — it walks you through creating the SPA application and the custom API in the Auth0 dashboard, configuring callback URLs, and avoiding common pitfalls (wrong audience, unauthorized client).

### Backend

Copy `src/back/appsettings.Development.example.json` to `src/back/appsettings.Development.json` and fill in your Auth0 credentials (Domain + Audience). This file is git-ignored — never commit real credentials.

### Frontend

Copy `src/front/.env.example` to `src/front/.env.local` and fill in `VITE_AUTH0_DOMAIN`, `VITE_AUTH0_CLIENT_ID`, `VITE_AUTH0_AUDIENCE`, and `VITE_API_URL`. This file is git-ignored.


## Adding a Feature

Features are implemented and reviewed entirely via Claude Code slash commands.

### 1. Implement

`/add-feature` scaffolds the full feature end-to-end: Domain → Application → Infrastructure → API → Frontend.

### 2. Review

`/everything-claude-code:code-review` reviews the current branch for correctness, type safety, security, and pattern compliance. Pass a PR number to post the review directly to GitHub.

### Convention skills (enforced automatically)

| Skill | Rule |
|---|---|
| `dotnet-lambda-naming` | Explicit lambda parameter names in C# — no `x`, `e`, `r`, `i` |
| `react-lambda-naming` | Explicit callback parameter names in TypeScript/React |
| `parameter-naming` | Fully spelled-out method/function parameters — no `ct`, `req`, `cmd` |
| `react-no-any` | No `any` in TypeScript — concrete types, generics, or `unknown` |
| `dotnet-remove-usings` | Remove implicit-using redundancies from C# files |


## Running locally

Use `docker-compose up --build` for a full stack (frontend on port 80, API + Swagger on port 8080, PostgreSQL on 5432). Add the override file for hot reload in development. See `CLAUDE.md` for the full command reference.


## Contributing

1. Fork the repository and create a branch from `master`.
2. Use `/add-feature` to implement your change and `/code-review` to validate it.
3. Open a pull request against `master` — CI runs backend and frontend checks automatically.


## License

[MIT](LICENSE) — free to use as a base for any project, commercial or otherwise.
