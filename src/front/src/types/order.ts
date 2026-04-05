export interface OrderItemDto {
  id: number
  catalogItemId: number
  catalogItemName: string
  quantity: number
  unitPrice: number
}

export interface OrderDto {
  id: number
  items: OrderItemDto[]
  totalPrice: number
  createdAt: string
  updatedAt: string
}

export interface CreateOrderItemCommand {
  catalogItemId: number
  quantity: number
}

export interface CreateOrderCommand {
  items: CreateOrderItemCommand[]
}
