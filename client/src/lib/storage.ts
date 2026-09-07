const TOKEN_KEY = 'hearth.token'
const USER_KEY = 'hearth.user'
const CART_KEY = 'hearth.cart'

export const storage = {
  getToken() {
    return localStorage.getItem(TOKEN_KEY)
  },
  setToken(token: string) {
    localStorage.setItem(TOKEN_KEY, token)
  },
  clearToken() {
    localStorage.removeItem(TOKEN_KEY)
  },
  getUser<T>() {
    const raw = localStorage.getItem(USER_KEY)
    if (!raw) return null
    try {
      return JSON.parse(raw) as T
    } catch {
      return null
    }
  },
  setUser(user: unknown) {
    localStorage.setItem(USER_KEY, JSON.stringify(user))
  },
  clearUser() {
    localStorage.removeItem(USER_KEY)
  },
  getCart<T>(fallback: T) {
    const raw = localStorage.getItem(CART_KEY)
    if (!raw) return fallback
    try {
      return JSON.parse(raw) as T
    } catch {
      return fallback
    }
  },
  setCart(cart: unknown) {
    localStorage.setItem(CART_KEY, JSON.stringify(cart))
  },
}
