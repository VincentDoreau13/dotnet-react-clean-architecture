import type { CatalogItemDto, CreateCatalogItemCommand } from "@/types/catalog"
import { apiClient } from "@/api/client"

export { isApiError } from "@/api/client"

export const catalogApi = {
  getItems: (): Promise<CatalogItemDto[]> =>
    apiClient.get<CatalogItemDto[]>("/catalog/items").then((response) => response.data),

  getItemById: (id: number): Promise<CatalogItemDto> =>
    apiClient.get<CatalogItemDto>(`/catalog/items/${id}`).then((response) => response.data),

  createItem: (command: CreateCatalogItemCommand): Promise<CatalogItemDto> =>
    apiClient.post<CatalogItemDto>("/catalog/items", command).then((response) => response.data),
}
