import axios from "axios"
import type { AxiosError } from "axios"
import type { CatalogItemDto, CreateCatalogItemCommand, ApiProblemDetails } from "@/types/catalog"

export const apiClient = axios.create({ baseURL: "/api" })

/**
 * Narrows an unknown catch value to a typed Axios error carrying
 * the backend's ProblemDetails payload.
 */
export function isApiError(err: unknown): err is AxiosError<ApiProblemDetails> {
  return axios.isAxiosError(err)
}

export const catalogApi = {
  getItems: (): Promise<CatalogItemDto[]> =>
    apiClient.get<CatalogItemDto[]>("/catalog/items").then((response) => response.data),

  getItemById: (id: number): Promise<CatalogItemDto> =>
    apiClient.get<CatalogItemDto>(`/catalog/items/${id}`).then((response) => response.data),

  createItem: (command: CreateCatalogItemCommand): Promise<CatalogItemDto> =>
    apiClient.post<CatalogItemDto>("/catalog/items", command).then((response) => response.data),
}
