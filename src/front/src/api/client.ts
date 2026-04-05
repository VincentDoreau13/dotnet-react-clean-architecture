import axios from "axios"
import type { AxiosError } from "axios"
import type { ApiProblemDetails } from "@/types/catalog"

export const apiClient = axios.create({ baseURL: "/api" })

/**
 * Narrows an unknown catch value to a typed Axios error carrying
 * the backend's ProblemDetails payload.
 */
export function isApiError(err: unknown): err is AxiosError<ApiProblemDetails> {
  return axios.isAxiosError(err)
}
