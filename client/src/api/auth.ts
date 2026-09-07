import { api } from '@/api/client'
import type { LoginRequest, LoginResponse, RegisterRequest } from '@/types'

export async function login(payload: LoginRequest) {
  const { data } = await api.post<LoginResponse>('/User/login', payload)
  return data
}

export async function register(payload: RegisterRequest) {
  const { data } = await api.post<LoginResponse>('/User/register', payload)
  return data
}
