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

/**
 * ASP.NET Core ProblemDetails shape returned by CustomErrorHandlerHelper.
 * ValidationException errors include field messages under extensions.validations[].
 */
export interface ApiProblemDetails {
  title?: string
  detail?: string
  status?: number
  extensions?: { validations?: string[] }
}
