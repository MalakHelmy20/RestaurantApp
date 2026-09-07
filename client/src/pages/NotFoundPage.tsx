import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <div className="mx-auto max-w-3xl px-5 py-24 sm:px-8">
      <p className="text-xs uppercase tracking-[0.24em] text-muted">404</p>
      <h1 className="mt-3 font-display text-5xl">This table is empty.</h1>
      <p className="mt-4 text-muted">The page you wanted is not on the menu.</p>
      <Link
        to="/"
        className="mt-8 inline-flex h-11 items-center rounded-full bg-ink px-5 text-sm text-cream"
      >
        Back to Hearth
      </Link>
    </div>
  )
}
