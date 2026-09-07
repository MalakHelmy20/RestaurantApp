import { getRestaurants } from '@/api/restaurants'
import { RestaurantRow } from '@/components/RestaurantRow'
import { PageError, Skeleton } from '@/ui/Feedback'
import { useQuery } from '@tanstack/react-query'
import { ArrowRight } from 'lucide-react'
import { Link } from 'react-router-dom'

export function HomePage() {
  const restaurantsQuery = useQuery({
    queryKey: ['restaurants'],
    queryFn: getRestaurants,
  })

  const featured = (restaurantsQuery.data ?? [])
    .slice()
    .sort((a, b) => b.averageRating - a.averageRating)
    .slice(0, 4)

  return (
    <div>
      <section className="mx-auto grid max-w-6xl gap-12 px-5 pb-8 pt-12 sm:px-8 lg:grid-cols-[1.15fr_0.85fr] lg:items-end lg:pt-20">
        <div className="animate-fade-up">
          <p className="text-xs uppercase tracking-[0.28em] text-muted">Tables, tonight</p>
          <h1 className="mt-4 max-w-xl font-display text-5xl leading-[1.05] tracking-tight sm:text-6xl lg:text-7xl">
            Dinner, without the noise.
          </h1>
          <p className="mt-6 max-w-md text-lg leading-relaxed text-ink-soft">
            Browse kitchens nearby, read a real menu, and send an order in a few quiet taps.
          </p>
          <div className="mt-8 flex flex-wrap gap-3">
            <Link
              to="/discover"
              className="inline-flex h-12 items-center rounded-full bg-terracotta px-6 text-cream transition hover:bg-terracotta-dark"
            >
              Browse kitchens
            </Link>
            <Link
              to="/register"
              className="inline-flex h-12 items-center rounded-full border border-line px-6 text-ink transition hover:border-ink/30"
            >
              Create an account
            </Link>
          </div>
        </div>
        <div className="hidden rounded-[2rem] bg-ink px-8 py-10 text-cream lg:block">
          <p className="text-xs uppercase tracking-[0.24em] text-cream/50">How it works</p>
          <ol className="mt-8 space-y-8">
            {[
              ['Choose a kitchen', 'Read hours, ratings, and a proper menu.'],
              ['Build a bag', 'Add dishes from one restaurant at a time.'],
              ['Follow the order', 'Confirmed, preparing, on the way, delivered.'],
            ].map(([title, copy], index) => (
              <li key={title} className="flex gap-4">
                <span className="font-display text-2xl text-terracotta">{index + 1}</span>
                <div>
                  <p className="font-medium">{title}</p>
                  <p className="mt-1 text-sm text-cream/65">{copy}</p>
                </div>
              </li>
            ))}
          </ol>
        </div>
      </section>

      <section className="mx-auto max-w-6xl px-5 pb-24 sm:px-8">
        <div className="flex items-end justify-between gap-4">
          <div>
            <p className="text-xs uppercase tracking-[0.24em] text-muted">On the table</p>
            <h2 className="mt-2 font-display text-3xl sm:text-4xl">Kitchens worth opening</h2>
          </div>
          <Link to="/discover" className="inline-flex items-center gap-1 text-sm text-muted hover:text-ink">
            All kitchens <ArrowRight size={16} />
          </Link>
        </div>

        <div className="mt-4">
          {restaurantsQuery.isLoading ? (
            <div className="space-y-4 pt-6">
              <Skeleton className="h-24" />
              <Skeleton className="h-24" />
              <Skeleton className="h-24" />
            </div>
          ) : restaurantsQuery.isError ? (
            <div className="pt-8">
              <PageError
                message="The kitchen list could not be reached. Make sure the API is running on port 5140."
                onRetry={() => void restaurantsQuery.refetch()}
              />
            </div>
          ) : featured.length === 0 ? (
            <p className="pt-10 text-muted">No restaurants yet. Check back after the first kitchen opens.</p>
          ) : (
            featured.map((restaurant) => (
              <RestaurantRow key={restaurant.id} restaurant={restaurant} featured />
            ))
          )}
        </div>
      </section>
    </div>
  )
}
