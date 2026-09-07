import { Link } from 'react-router-dom'

export function Footer() {
  return (
    <footer className="mt-auto border-t border-line">
      <div className="mx-auto flex max-w-6xl flex-col gap-4 px-5 py-10 sm:flex-row sm:items-end sm:justify-between sm:px-8">
        <div>
          <p className="font-display text-xl">Hearth</p>
          <p className="mt-1 max-w-sm text-sm text-muted">
            A quieter way to find a table and bring a kitchen home.
          </p>
        </div>
        <div className="flex gap-6 text-sm text-muted">
          <Link to="/discover" className="hover:text-ink">
            Discover
          </Link>
          <Link to="/login" className="hover:text-ink">
            Sign in
          </Link>
        </div>
      </div>
    </footer>
  )
}
