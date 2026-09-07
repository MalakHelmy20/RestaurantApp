import { api } from '@/api/client'
import type {
  CreateRestaurantRequest,
  Restaurant,
  RestaurantFilter,
  RestaurantSummary,
  UpdateRestaurantRequest,
} from '@/types'

export async function getRestaurants() {
  const { data } = await api.get<Restaurant[]>('/Restaurant')
  return data
}

export async function getRestaurant(id: string) {
  const { data } = await api.get<Restaurant>(`/Restaurant/${id}`)
  return data
}

export async function filterRestaurants(filter: RestaurantFilter) {
  const { data } = await api.get<RestaurantSummary[]>('/Restaurant/filter', {
    params: filter,
  })
  return data
}

export async function createRestaurant(payload: CreateRestaurantRequest) {
  const { data } = await api.post<Restaurant>('/Restaurant', payload)
  return data
}

export async function updateRestaurant(id: string, payload: UpdateRestaurantRequest) {
  const { data } = await api.put<boolean>(`/Restaurant/${id}`, payload)
  return data
}

export async function deleteRestaurant(id: string) {
  const { data } = await api.delete<boolean>(`/Restaurant/${id}`)
  return data
}
