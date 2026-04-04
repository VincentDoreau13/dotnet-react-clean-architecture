---
name: parameter-naming
description: Enforce descriptive parameter names in C# method signatures — no abbreviations like ct, req, res, cmd, msg, cfg, opts, ex, e
---

# Parameter Naming Convention

Method and constructor parameters must be **self-documenting**. The reader must understand what the parameter is without hovering over it or consulting the signature.

## Rule

**Never** abbreviate parameter names. Always use the full, explicit name.

## Common violations

| Forbidden | Correct |
|---|---|
| `ct` | `cancellationToken` |
| `req` | `request` |
| `res` | `response` |
| `cmd` | `command` |
| `msg` | `message` |
| `cfg` | `configuration` |
| `opts` / `opt` | `options` |
| `ex` / `e` | `exception` |
| `ctx` | `context` |
| `repo` | `repository` |
| `svc` | `service` |
| `val` / `v` | `value` |
| `obj` / `o` | use the actual type name |
| `str` / `s` | use the actual meaning (e.g. `name`, `path`) |
| `id` alone when ambiguous | `catalogItemId`, `orderId`, etc. |

## Examples

```csharp
// BAD
public async Task<CatalogItemDto> Handle(GetCatalogItemByIdQuery query, CancellationToken ct)
public Task<bool> ExistAsync(int id, CancellationToken ct = default)
public void Configure(IApplicationBuilder req, IWebHostEnvironment env)

// GOOD
public async Task<CatalogItemDto> Handle(GetCatalogItemByIdQuery query, CancellationToken cancellationToken)
public Task<bool> ExistAsync(int id, CancellationToken cancellationToken = default)
public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
```

## Applies to

- Method parameters
- Constructor parameters
- Lambda parameters (see `efcore-lambda-naming` skill)
- Local variable names derived from parameters

## Exceptions

- `app` for `IApplicationBuilder` / `WebApplication` — idiomatic ASP.NET Core convention
- `i`, `j` as loop indices in plain `for` loops
- EF Core migrations (auto-generated, never edit)

## Why

Clean Code principle — parameter names are the contract of a method. Abbreviations save a few keystrokes but cost every reader who encounters the code cold.
