import { api } from '@/api/client'
import type { CreateProductRequest, Product, ProductFilter, UpdateProductRequest } from '@/types'

export async function getProducts() {
  const { data } = await api.get<Product[]>('/Product')
  return data
}

export async function getProduct(id: string) {
  const { data } = await api.get<Product>(`/Product/${id}`)
  return data
}

export async function filterProducts(filter: ProductFilter) {
  const { data } = await api.get<Product[]>('/Product/filter', { params: filter })
  return data
}

export async function createProduct(payload: CreateProductRequest) {
  const { data } = await api.post<Product>('/Product', payload)
  return data
}

export async function updateProduct(id: string, payload: UpdateProductRequest) {
  await api.put(`/Product/${id}`, payload)
}

export async function deleteProduct(id: string) {
  await api.delete(`/Product/${id}`)
}
