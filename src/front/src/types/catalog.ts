export interface CatalogItemDto {
  id: number
  name: string
  description: string
  price: number
  availableStock: number
  createdAt: string
  updatedAt: string
}

export interface CreateCatalogItemCommand {
  name: string
  description: string
  price: number
  availableStock: number
}

export interface UpdateCatalogItemStockCommand {
  availableStock: number
}

/**
 * ASP.NET Core ProblemDetails shape returned by CustomErrorHandlerHelper.
 * Extensions are serialized as top-level properties in the JSON response,
 * not nested under an `extensions` object.
 */
export interface ApiProblemDetails {
  title?: string
  detail?: string
  status?: number
  validations?: string[]
  code?: string
  traceId?: string
}
