---
name: add-feature
description: Add a full-stack feature to the project following Clean Architecture — Domain → Application → Infrastructure → API → Frontend
---

# Add Feature

Implement a complete feature end-to-end across all layers of the Clean Architecture stack. Read the existing code before writing anything new; always mirror the patterns already in place.

## Checklist

Work through the layers in order. Do not skip layers.

### 1. Domain (`src/back/Domain/`)

- [ ] Create the entity or value object under `Domain/<FeatureName>/`
- [ ] Use private setters and a factory method / constructor that enforces invariants
- [ ] Raise domain events if relevant (`IDomainEvent` via `AddDomainEvent`)
- [ ] No external dependencies — only primitive types and other domain objects

```csharp
// Domain/<FeatureName>/<EntityName>.cs
public class Order : BaseEntity
{
    public string Reference { get; private set; }

    private Order() { }  // EF constructor

    public static Order Create(string reference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);
        return new Order { Reference = reference };
    }
}
```

### 2. Application (`src/back/Application/<FeatureName>/`)

For each use case, create a subfolder `Queries/<UseCaseName>/` or `Commands/<UseCaseName>/`.

Each subfolder contains:
- `<UseCaseName>.cs` — the MediatR `IRequest<TResponse>` record
- `<UseCaseName>Handler.cs` — the `IRequestHandler<TRequest, TResponse>` implementation
- `<UseCaseName>Validator.cs` — the `AbstractValidator<TRequest>` (FluentValidation)

```csharp
// Application/<Feature>/Commands/CreateOrder/CreateOrderCommand.cs
public record CreateOrderCommand(string Reference) : IRequest<int>;

// Application/<Feature>/Commands/CreateOrder/CreateOrderCommandHandler.cs
public class CreateOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, int>
{
    public async Task<int> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var order = Order.Create(command.Reference);
        await orderRepository.AddAsync(order, cancellationToken);
        return order.Id;
    }
}

// Application/<Feature>/Commands/CreateOrder/CreateOrderCommandValidator.cs
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.Reference).NotEmpty().MaximumLength(100);
    }
}
```

### 3. Infrastructure (`src/back/Infrastructure/`)

#### EF Core configuration (`Infrastructure/Data/Configurations/`)

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderBuilder)
    {
        orderBuilder.ToTable("Orders");
        orderBuilder.Property(order => order.Reference)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

- Configurations are auto-discovered by `AppDbContext.OnModelCreating` via `ApplyConfigurationsFromAssembly` — no manual registration needed
- Seed uses **anonymous types** (not `new Order { ... }`) to be compatible with private setters:

```csharp
orderBuilder.HasData(new { Id = 1, Reference = "ORD-001" });
```

#### Repository (`Infrastructure/Repositories/`)

- Define the interface in `Application/<Feature>/` (or a shared `Application/Interfaces/` if reused)
- Implement in `Infrastructure/Repositories/`
- Do **not** add `.AsNoTracking()` — it is already set globally on the DbContext
- Use `.AsTracking()` only for write operations that require change tracking

```csharp
// Application/<Feature>/IOrderRepository.cs
public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByReferenceAsync(string reference, CancellationToken cancellationToken);
}

// Infrastructure/Repositories/OrderRepository.cs
public class OrderRepository(AppDbContext dbContext)
    : EfRepository<Order>(dbContext), IOrderRepository
{
    public async Task<Order?> GetByReferenceAsync(
        string reference,
        CancellationToken cancellationToken)
        => await DbSet.FirstOrDefaultAsync(
            order => order.Reference == reference,
            cancellationToken);
}
```

### 4. EF Core Migration

**Do not skip this step.** Every schema change requires a migration.

```bash
cd src/back
dotnet ef migrations add Add<FeatureName> --project ShopApi.csproj
dotnet ef database update --project ShopApi.csproj   # only when running PostgreSQL
```

- [ ] Generate the migration after creating/modifying entities and configurations
- [ ] Verify the generated migration file makes sense (check for unwanted changes)
- [ ] In development with `UseInMemoryDatabase=true`, `database update` is not needed — the schema is rebuilt at startup

### 5. API (`src/back/API/Controllers/`)

- One controller per aggregate/feature, inheriting `ControllerBase`
- Inject `IMediator` and dispatch commands/queries
- Return `IActionResult` with appropriate HTTP status codes
- Errors are handled globally — do not catch exceptions in controllers

```csharp
[ApiController]
[Route("api/orders")]
public class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderId = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = orderId }, null);
    }
}
```

### 6. Frontend (`src/front/`)

#### API client (`src/api/`)

```typescript
// src/api/orders.ts
import axios from "axios";
import type { Order } from "../types/order";

export async function fetchOrders(): Promise<Order[]> {
    const response = await axios.get<Order[]>("/api/orders");
    return response.data;
}
```

#### Types (`src/types/`)

```typescript
// src/types/order.ts
export interface Order {
    id: number;
    reference: string;
}
```

#### Page (`src/pages/`)

Use TanStack Query for data fetching and React Hook Form + Zod for forms:

```typescript
// src/pages/OrdersPage.tsx
import { useQuery } from "@tanstack/react-query";
import { fetchOrders } from "../api/orders";

export default function OrdersPage() {
    const { data: orders = [], isLoading } = useQuery({
        queryKey: ["orders"],
        queryFn: fetchOrders,
    });

    if (isLoading) return <p>Loading…</p>;

    return (
        <ul>
            {orders.map((order) => (
                <li key={order.id}>{order.reference}</li>
            ))}
        </ul>
    );
}
```

## Conventions to enforce throughout

- Lambda/callback params: explicit names — see skills `dotnet-lambda-naming` and `react-lambda-naming`
- Method/function params: fully spelled out — see skill `parameter-naming`
- TypeScript: no `any` — see skill `react-no-any`
- No `.AsNoTracking()` in repositories (already set globally)
- Seed data uses anonymous types

## Error mapping (already configured — do not change)

| Exception | HTTP status |
|---|---|
| `NotFoundException` | 404 |
| `FunctionalException` | 400 |
| Unhandled | 500 |

Throw these exceptions from handlers or domain objects; the global handler in `API/Errors/CustomErrorHandlerHelper.cs` takes care of the rest.
