import { api } from '@/api/client'
import type { CreateRatingRequest, Rating, UpdateRatingRequest } from '@/types'

export async function getRatings() {
  const { data } = await api.get<Rating[]>('/Rating')
  return data
}

export async function getRestaurantRatings(restaurantId: string) {
  const { data } = await api.get<Rating[]>(`/Rating/restaurant/${restaurantId}`)
  return data
}

export async function createRating(payload: CreateRatingRequest) {
  const { data } = await api.post<Rating>('/Rating', payload)
  return data
}

export async function updateRating(id: string, payload: UpdateRatingRequest) {
  const { data } = await api.put<boolean>(`/Rating/${id}`, payload)
  return data
}

export async function deleteRating(id: string) {
  const { data } = await api.delete<boolean>(`/Rating/${id}`)
  return data
}
