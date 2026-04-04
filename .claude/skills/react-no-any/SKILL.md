---
name: react-no-any
description: Ban any in React/TypeScript — no explicit any types, no as any casts, no implicit any. Use unknown, proper generics, or type narrowing instead.
---

# No `any` in TypeScript / React

`any` disables TypeScript's type checker entirely for that value. It is contagious — one `any` silently erases type safety for all downstream code that touches it. It is forbidden in this codebase.

## Rule

**Never** use `any` — not in type annotations, not in casts, not implicitly.  
**Always** use the narrowest correct type: a concrete type, `unknown` with a type guard, or a well-constrained generic.

## Forbidden patterns

```ts
// Explicit annotation
const data: any = response.data
function process(payload: any): any { ... }
const items: any[] = []

// Casts
const error = err as any
const value = (someObj as any).field

// Generic wildcard escape
const ref = useRef<any>(null)
const [state, setState] = useState<any>(null)

// Object type holes
const config: { [key: string]: any } = {}
const options: Record<string, any> = {}
```

## Correct replacements

### Unknown API responses → `unknown` + type guard

```ts
// BAD
const data: any = await response.json()
console.log(data.items.length)  // no safety, crashes at runtime if shape differs

// GOOD
const data: unknown = await response.json()
if (isCatalogItemList(data)) {
  console.log(data.items.length)  // narrowed, type-safe
}
```

### Axios errors → `isAxiosError` type guard

```ts
// BAD
const apiError = error as any
const message = apiError.response.data.detail

// GOOD — use the isApiError guard exported from @/api/catalog
import { isApiError } from "@/api/catalog"

const apiProblem = isApiError(error) ? error.response?.data : undefined
const message = apiProblem?.detail
```

### Event handlers → use the correct React event type

```ts
// BAD
const handleChange = (event: any) => setValue(event.target.value)
const handleSubmit = (event: any) => { event.preventDefault(); submit() }

// GOOD
const handleChange = (changeEvent: React.ChangeEvent<HTMLInputElement>) =>
  setValue(changeEvent.target.value)
const handleSubmit = (submitEvent: React.FormEvent<HTMLFormElement>) => {
  submitEvent.preventDefault()
  submit()
}
```

### Generic components → constrain the generic

```ts
// BAD
function DataTable({ rows }: { rows: any[] }) { ... }
const useLocalStorage = <T = any>(key: string, initial: T) => { ... }

// GOOD
function DataTable<TRow extends { id: number }>({ rows }: { rows: TRow[] }) { ... }
const useLocalStorage = <T>(key: string, initial: T) => { ... }
```

### `useRef` / `useState` → provide the concrete type

```ts
// BAD
const inputRef = useRef<any>(null)
const [selectedItem, setSelectedItem] = useState<any>(null)

// GOOD
const inputRef = useRef<HTMLInputElement>(null)
const [selectedItem, setSelectedItem] = useState<CatalogItemDto | null>(null)
```

### Dynamic record shapes → `unknown` values

```ts
// BAD
const cache: Record<string, any> = {}

// GOOD
const cache: Record<string, unknown> = {}
// or, if the shape is known:
const cache: Record<string, CatalogItemDto> = {}
```

### `catch` clauses → `unknown` (TypeScript 4+ default in strict mode)

```ts
// BAD
} catch (e: any) {
  console.error(e.message)
}

// GOOD
} catch (error: unknown) {
  const message = error instanceof Error ? error.message : String(error)
  console.error(message)
}
```

## tsconfig enforcement

The project uses `"strict": true` in `tsconfig.app.json`. Add these two flags to make `any` a compiler error:

```json
{
  "compilerOptions": {
    "noImplicitAny": true,       // already implied by strict — makes implicit any a hard error
    "noExplicitAny": false       // TypeScript doesn't have this flag natively; use ESLint instead
  }
}
```

For ESLint enforcement add the `@typescript-eslint/no-explicit-any` rule (error level) when a linting setup is introduced.

## Exceptions

- **Type declaration files (`.d.ts`)** for untyped third-party libraries where no `@types/` package exists — document why with a comment.
- **Temporary migration scaffolding** — allowed only if the file has a `// TODO: remove any — tracking issue #N` comment. Must be resolved before merge.

## Why

`any` is an opt-out from TypeScript. Every `any` is a future runtime bug waiting to happen. `unknown` forces you to prove what the value is before you use it — the type checker then protects all downstream code automatically.
