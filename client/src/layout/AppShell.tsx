import { Footer } from '@/layout/Footer'
import { Header } from '@/layout/Header'
import { Outlet } from 'react-router-dom'

export function AppShell() {
  return (
    <div className="flex min-h-screen flex-col">
      <Header />
      <main className="flex-1">
        <Outlet />
      </main>
      <Footer />
    </div>
  )
}
