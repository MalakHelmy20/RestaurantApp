import { createRating, getRestaurantRatings, updateRating } from '@/api/ratings'
import { getRestaurant } from '@/api/restaurants'
import { useAuth } from '@/context/AuthContext'
import { useCart } from '@/context/CartContext'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { dishImage, restaurantImage } from '@/lib/foodImages'
import { formatMoney, formatTimeSpan, isOpenNow } from '@/lib/format'
import type { Product } from '@/types'
import { Button } from '@/ui/Button'
import { CoverImage } from '@/ui/CoverImage'
import { PageError, Skeleton } from '@/ui/Feedback'
import { Stars } from '@/ui/Stars'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Clock, MapPin, Phone } from 'lucide-react'
import { useMemo, useState } from 'react'
import { Link, useParams } from 'react-router-dom'

export function RestaurantPage() {
  const { id = '' } = useParams()
  const { user, isAuthenticated, hasRole } = useAuth()
  const { addItem, items } = useCart()
  const { notify } = useToast()
  const queryClient = useQueryClient()
  const [ratingValue, setRatingValue] = useState(0)

  const restaurantQuery = useQuery({
    queryKey: ['restaurant', id],
    queryFn: () => getRestaurant(id),
    enabled: Boolean(id),
  })

  const ratingsQuery = useQuery({
    queryKey: ['ratings', id],
    queryFn: () => getRestaurantRatings(id),
    enabled: Boolean(id),
  })

  const restaurant = restaurantQuery.data
  const myRating = ratingsQuery.data?.find((rating) => rating.userId === user?.id)

  const grouped = useMemo(() => {
    if (!restaurant) return []
    const namesById = new Map(restaurant.categories.map((category) => [category.id, category.name]))
    const map = new Map<string, Product[]>()
    for (const product of restaurant.products) {
      const key = product.categoryName || namesById.get(product.categoryId) || 'Menu'
      const list = map.get(key) ?? []
      list.push(product)
      map.set(key, list)
    }
    return [...map.entries()].filter(([, products]) => products.length > 0)
  }, [restaurant])

  const ratingMutation = useMutation({
    mutationFn: async (value: number) => {
      if (myRating) {
        await updateRating(myRating.id, { ratingValue: value })
        return
      }
      await createRating({ restaurantId: id, ratingValue: value })
    },
    onSuccess: async () => {
      notify(myRating ? 'Rating updated.' : 'Thanks for the rating.', 'success')
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: ['ratings', id] }),
        queryClient.invalidateQueries({ queryKey: ['restaurant', id] }),
        queryClient.invalidateQueries({ queryKey: ['restaurants'] }),
      ])
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  if (restaurantQuery.isLoading) {
    return (
      <div className="mx-auto max-w-6xl px-5 py-12 sm:px-8">
        <Skeleton className="h-10 w-64" />
        <Skeleton className="mt-4 h-5 w-80" />
        <Skeleton className="mt-10 h-72" />
      </div>
    )
  }

  if (restaurantQuery.isError || !restaurant) {
    return (
      <div className="mx-auto max-w-6xl px-5 py-12 sm:px-8">
        <PageError
          message="This restaurant could not be found."
          onRetry={() => void restaurantQuery.refetch()}
        />
      </div>
    )
  }

  const open = isOpenNow(restaurant.openTime, restaurant.closeTime)

  const onAdd = (product: Product) => {
    if (!open) {
      notify('This kitchen is currently closed and is not accepting orders.', 'error')
      return
    }
    if (!product.isAvailable) return
    const result = addItem(product, restaurant)
    notify(
      result.replaced
        ? `Bag cleared for ${restaurant.name}. ${product.name} added.`
        : `${product.name} added to bag.`,
      'success',
    )
  }

  const cover = restaurantImage(restaurant.name, [
    restaurant.description,
    ...restaurant.categories.map((category) => category.name),
  ])

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <CoverImage
        src={cover}
        alt={restaurant.name}
        className="mb-8 h-48 w-full rounded-[1.75rem] sm:h-64"
      />
      <div className="max-w-3xl">
        <p className="text-xs uppercase tracking-[0.24em] text-muted">
          {open ? 'Open now' : 'Currently closed'}
        </p>
        <h1 className="mt-2 font-display text-4xl leading-tight sm:text-6xl">{restaurant.name}</h1>
        <p className="mt-4 max-w-2xl text-lg leading-relaxed text-ink-soft">
          {restaurant.description}
        </p>
        <div className="mt-5 flex flex-wrap items-center gap-x-5 gap-y-2 text-sm text-muted">
          <span className="inline-flex items-center gap-2">
            <MapPin size={16} /> {restaurant.address}
          </span>
          <span className="inline-flex items-center gap-2">
            <Phone size={16} /> {restaurant.phone}
          </span>
          <span className="inline-flex items-center gap-2">
            <Clock size={16} /> {formatTimeSpan(restaurant.openTime)} –{' '}
            {formatTimeSpan(restaurant.closeTime)}
          </span>
        </div>
        {!open ? (
          <p className="mt-4 text-sm text-muted">
            This kitchen is closed right now. You can browse the menu, but orders open again at{' '}
            {formatTimeSpan(restaurant.openTime)}.
          </p>
        ) : null}
        <div className="mt-4">
          <Stars value={restaurant.averageRating} count={restaurant.totalRatingsCount} />
        </div>
      </div>

      <div className="mt-12 grid gap-14 lg:grid-cols-[minmax(0,1fr)_18rem]">
        <div>
          {grouped.length === 0 ? (
            <p className="text-muted">This kitchen has not published a menu yet.</p>
          ) : (
            grouped.map(([category, products]) => (
              <section key={category} className="mb-12">
                <h2 className="font-display text-2xl">{category}</h2>
                <div className="mt-4 divide-y divide-line">
                  {products.length === 0 ? (
                    <p className="py-4 text-sm text-muted">Nothing listed in this section yet.</p>
                  ) : (
                    products.map((product) => {
                      const inBag = items.find((item) => item.productId === product.id)
                      return (
                        <div
                          key={product.id}
                          className="grid gap-4 py-5 sm:grid-cols-[4.5rem_minmax(0,1fr)_auto] sm:items-center"
                        >
                          <CoverImage
                            src={dishImage(
                              product.name,
                              product.categoryName,
                              product.id,
                              product.description ?? '',
                            )}
                            alt={product.name}
                            className="h-16 w-16 rounded-2xl"
                          />
                          <div>
                            <div className="flex items-baseline gap-3">
                              <h3 className="text-lg font-medium">{product.name}</h3>
                              <span className="menu-rule hidden min-w-8 flex-1 sm:block" />
                              <span className="font-display text-lg">
                                {formatMoney(product.price)}
                              </span>
                            </div>
                            {product.description ? (
                              <p className="mt-1 max-w-xl text-sm leading-relaxed text-muted">
                                {product.description}
                              </p>
                            ) : null}
                            {!product.isAvailable ? (
                              <p className="mt-2 text-xs uppercase tracking-[0.16em] text-muted">
                                Unavailable
                              </p>
                            ) : null}
                          </div>
                          <Button
                            size="sm"
                            variant="soft"
                            disabled={!open || !product.isAvailable}
                            onClick={() => onAdd(product)}
                          >
                            {inBag ? `Add another · ${inBag.quantity}` : 'Add'}
                          </Button>
                        </div>
                      )
                    })
                  )}
                </div>
              </section>
            ))
          )}
        </div>

        <aside className="h-fit rounded-[1.75rem] bg-cream p-6 lg:sticky lg:top-24">
          <h2 className="font-display text-2xl">Ratings</h2>
          {ratingsQuery.isLoading ? (
            <Skeleton className="mt-4 h-24" />
          ) : (
            <div className="mt-4 space-y-3">
              {(ratingsQuery.data ?? []).length === 0 ? (
                <p className="text-sm text-muted">No ratings yet.</p>
              ) : (
                (ratingsQuery.data ?? []).map((rating) => (
                  <div key={rating.id} className="flex items-center justify-between text-sm">
                    <span>{rating.userName || 'Guest'}</span>
                    <Stars value={rating.ratingValue} />
                  </div>
                ))
              )}
            </div>
          )}

          {hasRole('Customer') ? (
            <div className="mt-6 border-t border-line pt-5">
              <p className="text-sm text-muted">
                {myRating ? 'Update your rating' : 'Rate this kitchen'}
              </p>
              <div className="mt-3">
                <Stars
                  value={ratingValue || myRating?.ratingValue || 0}
                  onChange={setRatingValue}
                  size={22}
                />
              </div>
              <Button
                className="mt-4 w-full"
                size="sm"
                disabled={!ratingValue || ratingMutation.isPending}
                loading={ratingMutation.isPending}
                onClick={() => ratingMutation.mutate(ratingValue)}
              >
                {myRating ? 'Update rating' : 'Submit rating'}
              </Button>
            </div>
          ) : !isAuthenticated ? (
            <p className="mt-5 text-sm text-muted">
              <Link to="/login" className="text-terracotta hover:underline">
                Sign in
              </Link>{' '}
              as a guest to leave a rating.
            </p>
          ) : null}
        </aside>
      </div>
    </div>
  )
}
