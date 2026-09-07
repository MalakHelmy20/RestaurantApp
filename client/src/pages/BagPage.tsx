import { createOrder } from '@/api/orders'
import { getRestaurant } from '@/api/restaurants'
import { useAuth } from '@/context/AuthContext'
import { useCart } from '@/context/CartContext'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { dishImage } from '@/lib/foodImages'
import { formatMoney, formatTimeSpan, isOpenNow } from '@/lib/format'
import { Button } from '@/ui/Button'
import { CoverImage } from '@/ui/CoverImage'
import { EmptyState } from '@/ui/Feedback'
import { useMutation, useQuery } from '@tanstack/react-query'
import { ShoppingBag } from 'lucide-react'
import { Link, useNavigate } from 'react-router-dom'

export function BagPage() {
  const { items, restaurantName, restaurantId, total, setQuantity, removeItem, clear } = useCart()
  const { isAuthenticated, hasRole } = useAuth()
  const { notify } = useToast()
  const navigate = useNavigate()

  const restaurantQuery = useQuery({
    queryKey: ['restaurant', restaurantId],
    queryFn: () => getRestaurant(restaurantId!),
    enabled: Boolean(restaurantId),
  })
  const restaurantOpen = restaurantQuery.data
    ? isOpenNow(restaurantQuery.data.openTime, restaurantQuery.data.closeTime)
    : true

  const checkout = useMutation({
    mutationFn: () => {
      if (!restaurantId) throw new Error('Your bag is empty.')
      return createOrder({
        restaurantId,
        orderItems: items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
      })
    },
    onSuccess: (order) => {
      clear()
      notify('Order placed.', 'success')
      navigate(`/orders/${order.id}`)
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  if (items.length === 0) {
    return (
      <div className="mx-auto max-w-6xl px-5 py-12 sm:px-8">
        <EmptyState
          icon={ShoppingBag}
          title="Your bag is empty"
          description="Add a dish from a kitchen, then come back here to place the order."
          action={
            <Link
              to="/discover"
              className="inline-flex h-11 items-center rounded-full bg-ink px-5 text-sm text-cream"
            >
              Browse kitchens
            </Link>
          }
        />
      </div>
    )
  }

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <p className="text-xs uppercase tracking-[0.24em] text-muted">Bag</p>
      <h1 className="mt-2 font-display text-4xl">{restaurantName}</h1>
      <p className="mt-2 text-muted">Orders are placed with one kitchen at a time.</p>

      <div className="mt-8 divide-y divide-line border-y border-line">
        {items.map((item) => (
          <div
            key={item.productId}
            className="flex flex-col gap-3 py-5 sm:flex-row sm:items-center sm:justify-between"
          >
            <div className="flex items-center gap-4">
              <CoverImage
                src={dishImage(item.name, '', item.productId)}
                alt={item.name}
                className="h-16 w-16 shrink-0 rounded-2xl"
              />
              <div>
                <p className="font-medium">{item.name}</p>
                <p className="text-sm text-muted">{formatMoney(item.price)} each</p>
              </div>
            </div>
            <div className="flex items-center gap-3">
              <div className="flex items-center rounded-full border border-line">
                <button
                  type="button"
                  className="h-10 w-10"
                  onClick={() => setQuantity(item.productId, item.quantity - 1)}
                  aria-label="Decrease quantity"
                >
                  −
                </button>
                <span className="w-8 text-center text-sm">{item.quantity}</span>
                <button
                  type="button"
                  className="h-10 w-10"
                  onClick={() => setQuantity(item.productId, item.quantity + 1)}
                  aria-label="Increase quantity"
                >
                  +
                </button>
              </div>
              <span className="w-20 text-right font-display">
                {formatMoney(item.price * item.quantity)}
              </span>
              <button
                type="button"
                className="text-sm text-muted hover:text-ink"
                onClick={() => removeItem(item.productId)}
              >
                Remove
              </button>
            </div>
          </div>
        ))}
      </div>

      <div className="mt-8 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <p className="font-display text-3xl">{formatMoney(total)}</p>
        {!isAuthenticated ? (
          <Button onClick={() => navigate('/login', { state: { from: '/bag' } })}>
            Sign in to order
          </Button>
        ) : !hasRole('Customer') ? (
          <p className="text-sm text-muted">Only guest accounts can place orders.</p>
        ) : !restaurantOpen ? (
          <div className="sm:text-right">
            <p className="text-sm text-muted">
              {restaurantQuery.data?.name ?? restaurantName} is closed. Orders open at{' '}
              {formatTimeSpan(restaurantQuery.data?.openTime)}.
            </p>
            <Button className="mt-3" disabled>
              Kitchen closed
            </Button>
          </div>
        ) : (
          <Button onClick={() => checkout.mutate()} loading={checkout.isPending}>
            Place order
          </Button>
        )}
      </div>
    </div>
  )
}
