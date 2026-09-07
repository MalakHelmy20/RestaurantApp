import { api } from '@/api/client'
import type { CreateOrderRequest, Order, UpdateOrderRequest } from '@/types'

export async function getOrders() {
  const { data } = await api.get<Order[]>('/Order')
  return data
}

export async function getOrder(id: string) {
  const { data } = await api.get<Order>(`/Order/${id}`)
  return data
}

export async function createOrder(payload: CreateOrderRequest) {
  const { data } = await api.post<Order>('/Order', payload)
  return data
}

export async function updateOrder(id: string, payload: UpdateOrderRequest) {
  const { data } = await api.put<{ message: string; updated: boolean }>(`/Order/${id}`, payload)
  return data
}

export async function deleteOrder(id: string) {
  const { data } = await api.delete<boolean>(`/Order/${id}`)
  return data
}
