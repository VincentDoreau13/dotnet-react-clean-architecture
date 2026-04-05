import type { OrderDto, CreateOrderCommand } from "@/types/order"
import { apiClient } from "@/api/client"

export const ordersApi = {
  getOrders: (): Promise<OrderDto[]> =>
    apiClient.get<OrderDto[]>("/orders").then((response) => response.data),

  getOrderById: (id: number): Promise<OrderDto> =>
    apiClient.get<OrderDto>(`/orders/${id}`).then((response) => response.data),

  createOrder: (command: CreateOrderCommand): Promise<OrderDto> =>
    apiClient.post<OrderDto>("/orders", command).then((response) => response.data),
}
