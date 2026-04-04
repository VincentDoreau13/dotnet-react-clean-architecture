import axios from "axios"
import type { CatalogItemDto, CreateCatalogItemCommand } from "@/types/catalog"

const api = axios.create({ baseURL: "/api" })

export const catalogApi = {
  getItems: (): Promise<CatalogItemDto[]> =>
    api.get<CatalogItemDto[]>("/catalog/items").then((r) => r.data),

  getItemById: (id: number): Promise<CatalogItemDto> =>
    api.get<CatalogItemDto>(`/catalog/items/${id}`).then((r) => r.data),

  createItem: (command: CreateCatalogItemCommand): Promise<CatalogItemDto> =>
    api.post<CatalogItemDto>("/catalog/items", command).then((r) => r.data),
}
