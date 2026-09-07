import { getOrders } from '@/api/orders'
import { useAuth } from '@/context/AuthContext'
import { formatDateTime, formatMoney, statusLabel } from '@/lib/format'
import { EmptyState, PageError, Skeleton } from '@/ui/Feedback'
import { SearchBar } from '@/ui/SearchBar'
import { useQuery } from '@tanstack/react-query'
import axios from 'axios'
import { ClipboardList } from 'lucide-react'
import { useMemo, useState } from 'react'
import { Link } from 'react-router-dom'

export function OrdersPage() {
  const { hasRole } = useAuth()
  const canSearch = hasRole('SystemAdmin', 'RestaurantOwner')
  const [draft, setDraft] = useState('')
  const [query, setQuery] = useState('')
  const trimmed = query.trim().toLowerCase()

  const ordersQuery = useQuery({
    queryKey: ['orders'],
    queryFn: async () => {
      try {
        return await getOrders()
      } catch (error) {
        if (axios.isAxiosError(error) && error.response?.status === 404 && hasRole('RestaurantOwner')) {
          return []
        }
        throw error
      }
    },
  })

  const orders = useMemo(() => {
    const list = ordersQuery.data ?? []
    if (!canSearch || !trimmed) return list
    return list.filter((order) => {
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
  }, [canSearch, ordersQuery.data, trimmed])

  const title = hasRole('Customer')
    ? 'Your orders'
    : hasRole('RestaurantOwner')
      ? 'Incoming tickets'
      : 'All orders'

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <p className="text-xs uppercase tracking-[0.24em] text-muted">Orders</p>
      <h1 className="mt-2 font-display text-4xl">{title}</h1>

      {canSearch ? (
        <SearchBar
          value={draft}
          onChange={setDraft}
          onSearch={() => setQuery(draft)}
          placeholder="Search by guest, kitchen, dish, or status"
          label="Search orders"
        />
      ) : null}

      <div className={canSearch ? 'mt-6' : 'mt-8'}>
        {ordersQuery.isLoading ? (
          <div className="space-y-3">
            <Skeleton className="h-20" />
            <Skeleton className="h-20" />
          </div>
        ) : ordersQuery.isError ? (
          <PageError message="Orders could not be loaded." onRetry={() => void ordersQuery.refetch()} />
        ) : !ordersQuery.data || ordersQuery.data.length === 0 ? (
          <EmptyState
            icon={ClipboardList}
            title="No orders yet"
            description={
              hasRole('Customer')
                ? 'When you place an order, it will appear here.'
                : 'Tickets show up here as soon as a guest orders.'
            }
          />
        ) : orders.length === 0 ? (
          <EmptyState
            icon={ClipboardList}
            title="No orders match"
            description="Try another guest, kitchen, dish, or status."
          />
        ) : (
          <div className="divide-y divide-line border-y border-line">
            {orders.map((order) => (
              <Link
                key={order.id}
                to={`/orders/${order.id}`}
                className="grid gap-2 py-5 transition hover:bg-cream/60 sm:grid-cols-[minmax(0,1fr)_auto] sm:items-center"
              >
                <div>
                  <p className="font-medium">{order.restaurantName}</p>
                  <p className="text-sm text-muted">
                    {order.customerName} · {formatDateTime(order.orderDate)} ·{' '}
                    {order.orderItems.length} item{order.orderItems.length === 1 ? '' : 's'}
                  </p>
                </div>
                <div className="sm:text-right">
                  <p className="font-display text-xl">{formatMoney(order.totalPrice)}</p>
                  <p className="text-sm text-muted">{statusLabel(order.status)}</p>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
