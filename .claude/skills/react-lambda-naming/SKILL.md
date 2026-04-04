---
name: react-lambda-naming
description: Enforce descriptive lambda/callback parameter names in React and TypeScript — no single-character or opaque names like x, e, r, v, cb, fn, item, obj
---

# Lambda Naming Convention — React / TypeScript

Lambda and callback parameters must be **self-documenting** in all TypeScript and React code. The reader must understand what the parameter represents without hovering over it.

## Rule

**Never** use single-character (`x`, `e`, `r`, `v`, `i`, `n`, `s`, `p`, `t`, `o`, `c`) or generic opaque names (`item`, `obj`, `val`, `data`, `res`, `cb`, `fn`, `el`, `elem`).  
**Always** use the camelCase name that reflects what the parameter actually is.

This applies equally to C# lambdas (see `efcore-lambda-naming`) and TypeScript/React lambdas — it is the same rule, same language.

## Examples by context

### Array methods

```ts
// BAD
const names = items.map(x => x.name)
const active = items.filter(i => i.isActive)
const total = orders.reduce((acc, o) => acc + o.amount, 0)
const ids = items.flatMap(x => x.subItems.map(s => s.id))

// GOOD
const names = items.map(catalogItem => catalogItem.name)
const active = items.filter(catalogItem => catalogItem.isActive)
const total = orders.reduce((sum, order) => sum + order.amount, 0)
const ids = items.flatMap(catalogItem => catalogItem.subItems.map(subItem => subItem.id))
```

### Promise chains

```ts
// BAD
api.get("/catalog/items").then(r => r.data)
fetchItem(id).then(res => setItem(res)).catch(e => console.error(e))

// GOOD
api.get("/catalog/items").then(response => response.data)
fetchItem(id).then(catalogItem => setItem(catalogItem)).catch(error => console.error(error))
```

### Event handlers in JSX

```tsx
// BAD
<input onChange={(e) => setValue(e.target.value)} />
<form onSubmit={(e) => { e.preventDefault(); submit() }} />
<Button onClick={(e) => e.stopPropagation()} />

// GOOD
<input onChange={(changeEvent) => setValue(changeEvent.target.value)} />
<form onSubmit={(submitEvent) => { submitEvent.preventDefault(); submit() }} />
<Button onClick={(clickEvent) => clickEvent.stopPropagation()} />
```

### TanStack Query / mutation callbacks

```ts
// BAD
useMutation({
  mutationFn: (data) => catalogApi.createItem(data),
  onSuccess: (res) => {
    queryClient.invalidateQueries({ queryKey: ["catalog-items"] })
    navigate(`/catalog/${res.id}`)
  },
  onError: (e) => setError(e.message),
})

// GOOD
useMutation({
  mutationFn: (command) => catalogApi.createItem(command),
  onSuccess: (createdItem) => {
    queryClient.invalidateQueries({ queryKey: ["catalog-items"] })
    navigate(`/catalog/${createdItem.id}`)
  },
  onError: (error) => setError(error.message),
})
```

### React Hook Form render props

```tsx
// BAD
<FormField
  control={form.control}
  name="price"
  render={({ field }) => (         // "field" is acceptable — it is the RHF convention
    <FormItem>
      <FormControl>
        <Input {...field} />
      </FormControl>
    </FormItem>
  )}
/>
```

> `field` is the established React Hook Form API name — it is acceptable. All other render-prop parameters must be explicit.

### Axios / fetch interceptors

```ts
// BAD
axios.interceptors.response.use(
  r => r,
  e => Promise.reject(e)
)

// GOOD
axios.interceptors.response.use(
  response => response,
  error => Promise.reject(error)
)
```

### `Object.entries` / `Object.keys`

```ts
// BAD
Object.entries(config).forEach(([k, v]) => console.log(k, v))

// GOOD
Object.entries(config).forEach(([key, value]) => console.log(key, value))
```

## Deriving the name

| What the parameter represents | Parameter name |
|---|---|
| `CatalogItemDto` from API | `catalogItem` |
| Array element of `Order[]` | `order` |
| DOM event (`React.ChangeEvent`) | `changeEvent`, `submitEvent`, `clickEvent` |
| Axios response object | `response` |
| Caught error / rejection | `error` |
| Accumulator in `reduce` | use what it accumulates: `sum`, `total`, `grouped` |
| MutationFn argument | use the command/payload type name: `command`, `payload` |
| `onSuccess` first argument | use the return type name: `createdItem`, `updatedOrder` |

## Exceptions

- `_` for intentionally ignored parameters (e.g., `array.map((_, index) => index)`)
- `field` in React Hook Form `render={({ field }) => ...}` — idiomatic RHF API name
- Mathematical/algorithmic conventions where the letter **is** the name (`x`, `y` in geometry)

## Why

Clean Code principle — a name should reveal intent. Single-character lambda params force reliance on IDE tooling. Code is read far more often than it is written. This rule applies identically across C# and TypeScript: the language changes, the principle does not.
