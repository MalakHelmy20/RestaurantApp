import { useAuth } from '@/context/AuthContext'
import type { UserRole } from '@/types'
import { Navigate, useLocation } from 'react-router-dom'
import type { ReactNode } from 'react'

type ProtectedRouteProps = {
  children: ReactNode
  roles?: UserRole[]
}

export function ProtectedRoute({ children, roles }: ProtectedRouteProps) {
  const { isAuthenticated, user, hasRole } = useAuth()
  const location = useLocation()

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />
  }

  if (roles && user && !hasRole(...roles)) {
    return <Navigate to="/" replace />
  }

  return children
}
