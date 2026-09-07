import { api } from '@/api/client'
import type { CreateUserRequest, UpdateUserRequest, User } from '@/types'

export async function getUsers(role?: string) {
  const { data } = await api.get<User[]>('/User', { params: role ? { role } : undefined })
  return data
}

export async function getUser(id: string) {
  const { data } = await api.get<User>(`/User/${id}`)
  return data
}

export async function createUser(payload: CreateUserRequest) {
  const { data } = await api.post<User>('/User', payload)
  return data
}

export async function updateUser(id: string, payload: UpdateUserRequest) {
  await api.put(`/User/${id}`, payload)
}

export async function deleteUser(id: string) {
  await api.delete(`/User/${id}`)
}
