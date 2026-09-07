import { restaurantImage } from '@/lib/foodImages'
import { formatTimeSpan, isOpenNow } from '@/lib/format'
import type { Restaurant, RestaurantSummary } from '@/types'
import { CoverImage } from '@/ui/CoverImage'
import { Stars } from '@/ui/Stars'
import { Link } from 'react-router-dom'

type Props = {
  restaurant: Restaurant | RestaurantSummary
  featured?: boolean
}

function isFullRestaurant(value: Restaurant | RestaurantSummary): value is Restaurant {
  return 'description' in value
}

export function RestaurantRow({ restaurant, featured }: Props) {
  const full = isFullRestaurant(restaurant)
  const open = full ? isOpenNow(restaurant.openTime, restaurant.closeTime) : false
  const hints = full
    ? [
        restaurant.description,
        ...restaurant.categories.map((category) => category.name),
      ]
    : []
  const image = restaurantImage(restaurant.name, hints)

  return (
    <Link
      to={`/restaurants/${restaurant.id}`}
      className="group grid gap-5 border-b border-line py-7 transition sm:grid-cols-[7.5rem_minmax(0,1fr)_auto] sm:items-center"
    >
      <CoverImage
        src={image}
        alt={restaurant.name}
        className="h-28 w-full rounded-2xl sm:h-[6.5rem] sm:w-[6.5rem]"
      />
      <div className="min-w-0">
        <div className="flex flex-wrap items-baseline gap-x-3 gap-y-1">
          <h3
            className={`font-display tracking-tight group-hover:text-terracotta ${
              featured ? 'text-3xl sm:text-4xl' : 'text-2xl'
            }`}
          >
            {restaurant.name}
          </h3>
          {full ? (
            <span className="text-xs uppercase tracking-[0.18em] text-muted">
              {open ? 'Open now' : 'Closed'}
            </span>
          ) : null}
        </div>
        <p className="mt-1 truncate text-muted">{restaurant.address}</p>
        {full && restaurant.description ? (
          <p className="mt-2 max-w-2xl text-sm leading-relaxed text-ink-soft line-clamp-2">
            {restaurant.description}
          </p>
        ) : null}
        {full && restaurant.categories.length > 0 ? (
          <p className="mt-3 text-xs uppercase tracking-[0.16em] text-muted">
            {restaurant.categories
              .map((category) => category.name)
              .slice(0, 4)
              .join(' · ')}
          </p>
        ) : null}
      </div>
      <div className="sm:text-right">
        <Stars value={restaurant.averageRating} count={full ? restaurant.totalRatingsCount : 0} />
        {full ? (
          <p className="mt-2 text-sm text-muted">
            {formatTimeSpan(restaurant.openTime)} – {formatTimeSpan(restaurant.closeTime)}
          </p>
        ) : null}
      </div>
    </Link>
  )
}
