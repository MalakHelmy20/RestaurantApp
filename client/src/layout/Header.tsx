import { useAuth } from '@/context/AuthContext'
import { useCart } from '@/context/CartContext'
import { cn } from '@/lib/cn'
import { roleLabel } from '@/lib/format'
import { Menu, ShoppingBag, X } from 'lucide-react'
import { useState } from 'react'
import { Link, NavLink, useNavigate } from 'react-router-dom'

const navLink = ({ isActive }: { isActive: boolean }) =>
  cn(
    'rounded-full px-3 py-2 text-sm transition',
    isActive ? 'text-ink' : 'text-muted hover:text-ink',
  )

export function Header() {
  const { user, isAuthenticated, logout, hasRole } = useAuth()
  const { count } = useCart()
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()

  const close = () => setOpen(false)

  const signOut = () => {
    logout()
    close()
    navigate('/')
  }

  return (
    <header className="sticky top-0 z-40 border-b border-line/80 bg-paper/85 backdrop-blur-md">
      <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-5 sm:h-[4.5rem] sm:px-8">
        <Link to="/" className="flex items-baseline gap-2" onClick={close}>
          <span className="font-display text-2xl font-semibold tracking-tight">Hearth</span>
          <span className="hidden text-xs uppercase tracking-[0.22em] text-muted sm:inline">
            kitchens
          </span>
        </Link>

        <nav className="hidden items-center gap-1 md:flex">
          <NavLink to="/discover" className={navLink}>
            Discover
          </NavLink>
          {isAuthenticated ? (
            <NavLink to="/orders" className={navLink}>
              Orders
            </NavLink>
          ) : null}
          {hasRole('RestaurantOwner', 'SystemAdmin') ? (
            <NavLink to="/kitchen" className={navLink}>
              Kitchen
            </NavLink>
          ) : null}
          {hasRole('SystemAdmin') ? (
            <NavLink to="/people" className={navLink}>
              People
            </NavLink>
          ) : null}
        </nav>

        <div className="flex items-center gap-2">
          <Link
            to="/bag"
            className="relative rounded-full p-2 text-ink transition hover:bg-paper-2"
            aria-label="Bag"
          >
            <ShoppingBag size={20} />
            {count > 0 ? (
              <span className="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-terracotta px-1 text-[11px] font-medium text-cream">
                {count}
              </span>
            ) : null}
          </Link>

          <div className="hidden items-center gap-3 md:flex">
            {isAuthenticated && user ? (
              <>
                <Link to="/account" className="text-sm text-muted hover:text-ink">
                  {user.firstName}
                  <span className="ml-2 text-xs uppercase tracking-wider">
                    {roleLabel(user.role)}
                  </span>
                </Link>
                <button
                  type="button"
                  onClick={signOut}
                  className="text-sm text-muted hover:text-ink"
                >
                  Sign out
                </button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-sm text-muted hover:text-ink">
                  Sign in
                </Link>
                <Link
                  to="/register"
                  className="whitespace-nowrap rounded-full bg-[#E56A2A] px-4 py-2 text-sm font-medium text-white shadow-sm transition hover:bg-[#cf5c22]"
                >
                  Create account
                </Link>
              </>
            )}
          </div>

          <button
            type="button"
            className="rounded-full p-2 md:hidden"
            onClick={() => setOpen((value) => !value)}
            aria-label="Menu"
          >
            {open ? <X size={20} /> : <Menu size={20} />}
          </button>
        </div>
      </div>

      {open ? (
        <div className="border-t border-line bg-paper px-5 py-4 md:hidden">
          <div className="flex flex-col gap-2 text-base">
            <Link to="/discover" onClick={close} className="py-2">
              Discover
            </Link>
            {isAuthenticated ? (
              <Link to="/orders" onClick={close} className="py-2">
                Orders
              </Link>
            ) : null}
            {hasRole('RestaurantOwner', 'SystemAdmin') ? (
              <Link to="/kitchen" onClick={close} className="py-2">
                Kitchen
              </Link>
            ) : null}
            {hasRole('SystemAdmin') ? (
              <Link to="/people" onClick={close} className="py-2">
                People
              </Link>
            ) : null}
            <Link to="/bag" onClick={close} className="py-2">
              Bag
            </Link>
            {isAuthenticated && user ? (
              <>
                <Link to="/account" onClick={close} className="py-2">
                  Account
                </Link>
                <button type="button" onClick={signOut} className="py-2 text-left">
                  Sign out
                </button>
              </>
            ) : (
              <>
                <Link to="/login" onClick={close} className="py-2">
                  Sign in
                </Link>
                <Link
                  to="/register"
                  onClick={close}
                  className="mt-1 inline-flex w-fit rounded-full bg-[#E56A2A] px-4 py-2 text-sm font-medium text-white"
                >
                  Create an account
                </Link>
              </>
            )}
          </div>
        </div>
      ) : null}
    </header>
  )
}
