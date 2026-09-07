import { api } from '@/api/client'
import type { Category, CreateCategoryRequest, UpdateCategoryRequest } from '@/types'

export async function getCategories() {
  const { data } = await api.get<Category[]>('/Category')
  return data
}

export async function getCategory(id: string) {
  const { data } = await api.get<Category>(`/Category/${id}`)
  return data
}

export async function createCategory(payload: CreateCategoryRequest) {
  const { data } = await api.post<Category>('/Category', payload)
  return data
}

export async function updateCategory(id: string, payload: UpdateCategoryRequest) {
  await api.put(`/Category/${id}`, payload)
}

export async function deleteCategory(id: string) {
  await api.delete(`/Category/${id}`)
}
