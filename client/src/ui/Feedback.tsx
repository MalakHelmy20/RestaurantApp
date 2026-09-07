import { cn } from '@/lib/cn'
import type { LucideIcon } from 'lucide-react'
import type { ReactNode } from 'react'

type EmptyStateProps = {
  icon: LucideIcon
  title: string
  description: string
  action?: ReactNode
  className?: string
}

export function EmptyState({ icon: Icon, title, description, action, className }: EmptyStateProps) {
  return (
    <div className={cn('flex flex-col items-start py-16', className)}>
      <div className="mb-5 flex h-12 w-12 items-center justify-center rounded-2xl bg-paper-2 text-ink-soft">
        <Icon size={22} />
      </div>
      <h3 className="font-display text-2xl font-semibold">{title}</h3>
      <p className="mt-2 max-w-md text-muted">{description}</p>
      {action ? <div className="mt-6">{action}</div> : null}
    </div>
  )
}

export function Skeleton({ className }: { className?: string }) {
  return <div className={cn('skeleton rounded-2xl', className)} />
}

export function PageError({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return (
    <div className="rounded-3xl border border-line bg-cream px-6 py-8">
      <p className="font-display text-xl">We couldn’t load this right now.</p>
      <p className="mt-2 text-muted">{message}</p>
      {onRetry ? (
        <button
          type="button"
          onClick={onRetry}
          className="mt-4 text-sm font-medium text-terracotta hover:underline"
        >
          Try again
        </button>
      ) : null}
    </div>
  )
}
