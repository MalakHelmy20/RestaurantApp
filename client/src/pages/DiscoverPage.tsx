import { getCategories } from '@/api/categories'
import { filterRestaurants, getRestaurants } from '@/api/restaurants'
import { RestaurantRow } from '@/components/RestaurantRow'
import { cn } from '@/lib/cn'
import { EmptyState, PageError, Skeleton } from '@/ui/Feedback'
import { Input } from '@/ui/Field'
import { useQuery } from '@tanstack/react-query'
import { Search, UtensilsCrossed } from 'lucide-react'
import { useMemo, useState } from 'react'

export function DiscoverPage() {
  const [search, setSearch] = useState('')
  const [categoryId, setCategoryId] = useState('')
  const trimmed = search.trim()
  const usingFilter = Boolean(trimmed || categoryId)

  const allQuery = useQuery({
    queryKey: ['restaurants'],
    queryFn: getRestaurants,
    enabled: !usingFilter,
  })

  const filteredQuery = useQuery({
    queryKey: ['restaurants-filter', trimmed, categoryId],
    queryFn: () =>
      filterRestaurants({
        searchTerm: trimmed || undefined,
        categoryId: categoryId || undefined,
        pageNumber: 1,
        pageSize: 50,
      }),
    enabled: usingFilter,
  })

  const categoriesQuery = useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
  })

  const categoryChips = useMemo(() => {
    const seen = new Map<string, { id: string; name: string }>()
    for (const category of categoriesQuery.data ?? []) {
      const key = category.name.trim().toLowerCase()
      if (!seen.has(key)) seen.set(key, { id: category.id, name: category.name })
    }
    return [...seen.values()]
  }, [categoriesQuery.data])

  const list = usingFilter ? filteredQuery.data : allQuery.data
  const isLoading = usingFilter ? filteredQuery.isLoading : allQuery.isLoading
  const isError = usingFilter ? filteredQuery.isError : allQuery.isError
  const refetch = usingFilter ? filteredQuery.refetch : allQuery.refetch

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <p className="text-xs uppercase tracking-[0.24em] text-muted">Discover</p>
      <h1 className="mt-2 font-display text-4xl sm:text-5xl">Find a kitchen</h1>
      <p className="mt-3 max-w-xl text-muted">
        Search by name, or skim by the kind of cooking you want tonight.
      </p>

      <div className="relative mt-8 max-w-xl">
        <Search size={18} className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-muted" />
        <Input
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Search restaurants"
          className="pl-11"
          aria-label="Search restaurants"
        />
      </div>

      {categoryChips.length > 0 ? (
        <div className="mt-5 flex flex-wrap gap-2">
          <button
            type="button"
            onClick={() => setCategoryId('')}
            className={cn(
              'rounded-full px-3 py-1.5 text-sm transition',
              categoryId === '' ? 'bg-ink text-cream' : 'bg-cream text-ink-soft hover:bg-paper-2',
            )}
          >
            All
          </button>
          {categoryChips.map((category) => (
            <button
              key={category.id}
              type="button"
              onClick={() => setCategoryId(category.id === categoryId ? '' : category.id)}
              className={cn(
                'rounded-full px-3 py-1.5 text-sm transition',
                categoryId === category.id
                  ? 'bg-ink text-cream'
                  : 'bg-cream text-ink-soft hover:bg-paper-2',
              )}
            >
              {category.name}
            </button>
          ))}
        </div>
      ) : null}

      <div className="mt-6">
        {isLoading ? (
          <div className="space-y-4">
            <Skeleton className="h-24" />
            <Skeleton className="h-24" />
            <Skeleton className="h-24" />
          </div>
        ) : isError ? (
          <PageError message="Restaurants could not be loaded." onRetry={() => void refetch()} />
        ) : !list || list.length === 0 ? (
          <EmptyState
            icon={UtensilsCrossed}
            title="No kitchens match"
            description="Try another name, or clear the category filter."
          />
        ) : (
          list.map((restaurant) => (
            <RestaurantRow key={restaurant.id} restaurant={restaurant} />
          ))
        )}
      </div>
    </div>
  )
}
