export interface CatalogItemDto {
  id: number
  name: string
  description: string
  price: number
  availableStock: number
  createdAt: string
}

export interface CreateCatalogItemCommand {
  name: string
  description: string
  price: number
  availableStock: number
}
