import { login as loginRequest, register as registerRequest } from '@/api/auth'
import { storage } from '@/lib/storage'
import type { LoginRequest, RegisterRequest, User, UserRole } from '@/types'
import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

type AuthState = {
  user: User | null
  token: string | null
}

type AuthContextValue = AuthState & {
  isAuthenticated: boolean
  login: (payload: LoginRequest) => Promise<User>
  register: (payload: RegisterRequest) => Promise<User>
  logout: () => void
  setUser: (user: User) => void
  hasRole: (...roles: UserRole[]) => boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

function readAuth(): AuthState {
  return {
    token: storage.getToken(),
    user: storage.getUser<User>(),
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(readAuth)

  const persist = useCallback((token: string, user: User) => {
    storage.setToken(token)
    storage.setUser(user)
    setState({ token, user })
  }, [])

  const login = useCallback(
    async (payload: LoginRequest) => {
      const response = await loginRequest(payload)
      persist(response.token, response.user)
      return response.user
    },
    [persist],
  )

  const register = useCallback(
    async (payload: RegisterRequest) => {
      const response = await registerRequest(payload)
      persist(response.token, response.user)
      return response.user
    },
    [persist],
  )

  const logout = useCallback(() => {
    storage.clearToken()
    storage.clearUser()
    setState({ token: null, user: null })
  }, [])

  const setUser = useCallback((user: User) => {
    storage.setUser(user)
    setState((current) => ({ ...current, user }))
  }, [])

  const hasRole = useCallback(
    (...roles: UserRole[]) => {
      if (!state.user) return false
      return roles.includes(state.user.role as UserRole)
    },
    [state.user],
  )

  const value = useMemo<AuthContextValue>(
    () => ({
      ...state,
      isAuthenticated: Boolean(state.token && state.user),
      login,
      register,
      logout,
      setUser,
      hasRole,
    }),
    [state, login, register, logout, setUser, hasRole],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) throw new Error('useAuth must be used within AuthProvider')
  return context
}
