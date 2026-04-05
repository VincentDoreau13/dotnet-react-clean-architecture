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
| `src/types/` | DTO interfaces mirroring backend response shapes |
| `src/components/` | shadcn/ui primitives + Layout shell |
| `src/pages/` | One page component per route |

Stack: Vite · TypeScript · shadcn/ui · TanStack Query · React Hook Form · Zod.


## Adding a Feature

Features are implemented and reviewed entirely via Claude Code slash commands.

### 1. Implement

```
/add-feature
```

Scaffolds the full feature end-to-end: Domain → Application → Infrastructure → API → Frontend.

### 2. Review

```
/everything-claude-code:code-review
```

Reviews the current branch for correctness, type safety, security, and pattern compliance. Pass a PR number to post the review directly to GitHub.

### Convention skills (enforced automatically)

| Skill | Rule |
|---|---|
| `dotnet-lambda-naming` | Explicit lambda parameter names in C# — no `x`, `e`, `r`, `i` |
| `react-lambda-naming` | Explicit callback parameter names in TypeScript/React |
| `parameter-naming` | Fully spelled-out method/function parameters — no `ct`, `req`, `cmd` |
| `react-no-any` | No `any` in TypeScript — concrete types, generics, or `unknown` |


## Running locally

```bash
docker-compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost |
| API + Swagger | http://localhost:8080 |
| PostgreSQL | localhost:5432 |

For hot reload (API + frontend):

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```


## Contributing

1. Fork the repository and create a branch from `master`.
2. Use `/add-feature` to implement your change and `/code-review` to validate it.
3. Open a pull request against `master` — the `pr-ci.yml` workflow runs backend and frontend checks in parallel. The **`all-checks`** status must pass before merge.


## License

[MIT](LICENSE) — free to use as a base for any project, commercial or otherwise.
