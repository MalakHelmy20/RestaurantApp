import { AuthProvider } from '@/context/AuthContext'
import { CartProvider } from '@/context/CartContext'
import { ToastProvider } from '@/context/ToastContext'
import { AppShell } from '@/layout/AppShell'
import { ProtectedRoute } from '@/layout/ProtectedRoute'
import { AccountPage } from '@/pages/AccountPage'
import { BagPage } from '@/pages/BagPage'
import { DiscoverPage } from '@/pages/DiscoverPage'
import { HomePage } from '@/pages/HomePage'
import { KitchenPage } from '@/pages/KitchenPage'
import { KitchenRestaurantPage } from '@/pages/KitchenRestaurantPage'
import { LoginPage } from '@/pages/LoginPage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { OrderDetailPage } from '@/pages/OrderDetailPage'
import { OrdersPage } from '@/pages/OrdersPage'
import { RegisterPage } from '@/pages/RegisterPage'
import { RestaurantPage } from '@/pages/RestaurantPage'
import { UsersPage } from '@/pages/UsersPage'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Route, Routes } from 'react-router-dom'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
})

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <CartProvider>
          <ToastProvider>
            <BrowserRouter>
              <Routes>
                <Route element={<AppShell />}>
                  <Route path="/" element={<HomePage />} />
                  <Route path="/discover" element={<DiscoverPage />} />
                  <Route path="/restaurants/:id" element={<RestaurantPage />} />
                  <Route path="/login" element={<LoginPage />} />
                  <Route path="/register" element={<RegisterPage />} />
                  <Route path="/bag" element={<BagPage />} />
                  <Route
                    path="/orders"
                    element={
                      <ProtectedRoute>
                        <OrdersPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/orders/:id"
                    element={
                      <ProtectedRoute>
                        <OrderDetailPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/account"
                    element={
                      <ProtectedRoute>
                        <AccountPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/kitchen"
                    element={
                      <ProtectedRoute roles={['RestaurantOwner', 'SystemAdmin']}>
                        <KitchenPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/kitchen/new"
                    element={
                      <ProtectedRoute roles={['RestaurantOwner', 'SystemAdmin']}>
                        <KitchenRestaurantPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/kitchen/:id"
                    element={
                      <ProtectedRoute roles={['RestaurantOwner', 'SystemAdmin']}>
                        <KitchenRestaurantPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route
                    path="/people"
                    element={
                      <ProtectedRoute roles={['SystemAdmin']}>
                        <UsersPage />
                      </ProtectedRoute>
                    }
                  />
                  <Route path="*" element={<NotFoundPage />} />
                </Route>
              </Routes>
            </BrowserRouter>
          </ToastProvider>
        </CartProvider>
      </AuthProvider>
    </QueryClientProvider>
  )
}
