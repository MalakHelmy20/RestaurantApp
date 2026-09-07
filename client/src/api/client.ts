import axios, { type AxiosError } from 'axios'
import { storage } from '@/lib/storage'

export const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

api.interceptors.request.use((config) => {
  const token = storage.getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      const url = error.config?.url ?? ''
      const isAuthCall = url.includes('/User/login') || url.includes('/User/register')
      if (!isAuthCall && storage.getToken()) {
        storage.clearToken()
        storage.clearUser()
        if (!window.location.pathname.startsWith('/login')) {
          window.location.assign('/login')
        }
      }
    }
    return Promise.reject(error)
  },
)
