---
name: dotnet-remove-usings
description: Remove unnecessary using directives from C# files using dotnet format IDE0005
---

# Remove Unnecessary Using Directives (.NET)

Supprime les `using` inutiles dans les fichiers C# via l'analyseur `IDE0005` de `dotnet format`.

## Rule

Tout `using` qui n'est pas consommé dans le fichier (y compris les namespaces déjà couverts par les **implicit usings** du SDK Web) doit être retiré. Un `using` inutile est du bruit — il augmente la charge cognitive sans apporter d'information.

Les **implicit usings** automatiquement disponibles dans `Microsoft.NET.Sdk.Web` incluent notamment :
`System`, `System.Linq`, `System.Collections.Generic`, `System.Threading.Tasks`,
`Microsoft.AspNetCore.Http`, `Microsoft.AspNetCore.Routing`, `Microsoft.Extensions.DependencyInjection`, etc.

## How to apply

### Sur le projet entier
```bash
cd src/back
dotnet format analyzers ShopApi.csproj --diagnostics IDE0005
```

> **Note** : `dotnet format` n'applique IDE0005 que si un `.editorconfig` active explicitement la règle. Sans `.editorconfig`, identifier et supprimer manuellement les usings listés dans les implicit usings du SDK (voir ci-dessous).

### Implicit usings automatiques dans `Microsoft.NET.Sdk.Web`
Ces namespaces sont injectés sans `using` explicite :
- `System`, `System.Linq`, `System.Collections.Generic`, `System.IO`
- `System.Threading`, `System.Threading.Tasks`, `System.Net.Http`
- `Microsoft.AspNetCore.Builder`, `Microsoft.AspNetCore.Http`
- `Microsoft.AspNetCore.Routing`, `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Hosting`
- `Microsoft.Extensions.Logging`

Tout `using` de cette liste dans un fichier `.cs` est inutile.

## Examples

```csharp
// BAD — Microsoft.AspNetCore.Http est un implicit using dans SDK.Web
using System.Diagnostics;
using Microsoft.AspNetCore.Http;   // ← superflu
using ShopApi.Application.Common.Interfaces;

// GOOD
using System.Diagnostics;
using ShopApi.Application.Common.Interfaces;
```

## Exceptions

- **`Migrations/`** — fichiers auto-générés par EF Core, ne jamais toucher.
- **`global using`** explicites dans `GlobalUsings.cs` — ne pas supprimer sans vérifier.
- Un `using` peut être nécessaire pour les **alias** (`using Alias = Some.Long.Namespace`) même si le type n'est pas utilisé directement.

## Why

Les implicit usings de .NET 6+ signifient que beaucoup de `using` hérités d'anciennes habitudes sont devenus redondants. Les laisser crée du bruit et induit en erreur sur les dépendances réelles d'un fichier.
