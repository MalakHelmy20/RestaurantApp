import { getOrders } from '@/api/orders'
import { getRestaurants } from '@/api/restaurants'
import { useAuth } from '@/context/AuthContext'
import { restaurantImage } from '@/lib/foodImages'
import { formatDateTime, formatMoney, statusLabel } from '@/lib/format'
import { Button } from '@/ui/Button'
import { CoverImage } from '@/ui/CoverImage'
import { EmptyState, PageError, Skeleton } from '@/ui/Feedback'
import { SearchBar } from '@/ui/SearchBar'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'
import { Store } from 'lucide-react'
import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'

export function KitchenPage() {
  const { user, hasRole } = useAuth()
  const [draft, setDraft] = useState('')
  const [query, setQuery] = useState('')
  const trimmed = query.trim().toLowerCase()

  const restaurantsQuery = useQuery({
    queryKey: ['restaurants'],
    queryFn: getRestaurants,
  })

  const ordersQuery = useQuery({
    queryKey: ['orders'],
    queryFn: async () => {
      try {
        return await getOrders()
      } catch (error) {
        if (axios.isAxiosError(error) && error.response?.status === 404) {
          return []
        }
        throw error
      }
    },
  })

  const mine = useMemo(
    () =>
      (restaurantsQuery.data ?? []).filter((restaurant) =>
        hasRole('SystemAdmin') ? true : restaurant.owner?.id === user?.id,
      ),
    [hasRole, restaurantsQuery.data, user?.id],
  )

  const visibleKitchens = useMemo(() => {
    if (!trimmed) return mine
    return mine.filter((restaurant) => {
      const blob = [
        restaurant.name,
        restaurant.address,
        restaurant.phone,
        ...restaurant.categories.map((category) => category.name),
        ...restaurant.products.map((product) => product.name),
      ]
        .join(' ')
        .toLowerCase()
      return blob.includes(trimmed)
    })
  }, [mine, trimmed])

  const openTickets = useMemo(() => {
    const tickets = (ordersQuery.data ?? []).filter(
      (order) => order.status !== 'delivered' && order.status !== 'cancelled',
    )
    if (!trimmed) return tickets
    return tickets.filter((order) => {
      const blob = [
        order.restaurantName,
        order.customerName,
        statusLabel(order.status),
        order.status,
        ...order.orderItems.map((item) => item.productName),
      ]
        .join(' ')
        .toLowerCase()
      return blob.includes(trimmed)
    })
  }, [ordersQuery.data, trimmed])

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.24em] text-muted">Kitchen</p>
          <h1 className="mt-2 font-display text-4xl">Back of house</h1>
          <p className="mt-2 max-w-xl text-muted">
            Manage your kitchens, keep the menu current, and move tickets along.
          </p>
        </div>
        <Link to="/kitchen/new">
          <Button>New kitchen</Button>
        </Link>
      </div>

      <SearchBar
        value={draft}
        onChange={setDraft}
        onSearch={() => setQuery(draft)}
        placeholder="Search kitchens, dishes, guests, or tickets"
        label="Search kitchen dashboard"
      />

      <section className="mt-12">
        <h2 className="font-display text-2xl">Your kitchens</h2>
        {restaurantsQuery.isLoading ? (
          <Skeleton className="mt-4 h-24" />
        ) : restaurantsQuery.isError ? (
          <div className="mt-4">
            <PageError
              message="Restaurants could not be loaded."
              onRetry={() => void restaurantsQuery.refetch()}
            />
          </div>
        ) : mine.length === 0 ? (
          <EmptyState
            icon={Store}
            title="No kitchen yet"
            description="Open a restaurant, then add categories and a menu."
            action={
              <Link
                to="/kitchen/new"
                className="inline-flex h-11 items-center rounded-full bg-ink px-5 text-sm text-cream"
              >
                Create a kitchen
              </Link>
            }
          />
        ) : visibleKitchens.length === 0 ? (
          <p className="mt-4 text-muted">No kitchens match that search.</p>
        ) : (
          <div className="mt-4 divide-y divide-line border-y border-line">
            {visibleKitchens.map((restaurant) => (
              <Link
                key={restaurant.id}
                to={`/kitchen/${restaurant.id}`}
                className="flex items-center gap-4 py-5 sm:justify-between"
              >
                <div className="flex min-w-0 items-center gap-4">
                  <CoverImage
                    src={restaurantImage(
                      restaurant.name,
                      restaurant.categories.map((category) => category.name),
                    )}
                    alt={restaurant.name}
                    className="h-16 w-16 shrink-0 rounded-2xl"
                  />
                  <div className="min-w-0">
                    <p className="font-display text-2xl">{restaurant.name}</p>
                    <p className="text-sm text-muted">
                      {restaurant.address} · {restaurant.products.length} dishes
                    </p>
                  </div>
                </div>
                <span className="text-sm text-muted">Manage menu →</span>
              </Link>
            ))}
          </div>
        )}
      </section>

      <section className="mt-14">
        <h2 className="font-display text-2xl">Open tickets</h2>
        {ordersQuery.isLoading ? (
          <Skeleton className="mt-4 h-24" />
        ) : openTickets.length === 0 ? (
          <p className="mt-4 text-muted">
            {trimmed ? 'No open tickets match that search.' : 'No open tickets right now.'}
          </p>
        ) : (
          <div className="mt-5 grid gap-4 sm:grid-cols-2">
            {openTickets.map((order) => (
              <Link
                key={order.id}
                to={`/orders/${order.id}`}
                className="rounded-[1.5rem] bg-ink px-5 py-5 text-cream transition hover:bg-ink-soft"
              >
                <p className="text-xs uppercase tracking-[0.18em] text-cream/55">
                  {statusLabel(order.status)}
                </p>
                <p className="mt-2 font-display text-2xl">{order.restaurantName}</p>
                <p className="mt-1 text-sm text-cream/70">
                  {order.customerName} · {formatDateTime(order.orderDate)}
                </p>
                <p className="mt-4 font-display text-xl">{formatMoney(order.totalPrice)}</p>
              </Link>
            ))}
          </div>
        )}
      </section>
    </div>
  )
}
