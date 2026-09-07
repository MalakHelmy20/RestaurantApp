import { cn } from '@/lib/cn'
import { X } from 'lucide-react'
import { useEffect, type ReactNode } from 'react'

type ModalProps = {
  open: boolean
  title: string
  description?: string
  onClose: () => void
  children: ReactNode
  wide?: boolean
}

export function Modal({ open, title, description, onClose, children, wide }: ModalProps) {
  useEffect(() => {
    if (!open) return
    const onKey = (event: KeyboardEvent) => {
      if (event.key === 'Escape') onClose()
    }
    document.addEventListener('keydown', onKey)
    document.body.style.overflow = 'hidden'
    return () => {
      document.removeEventListener('keydown', onKey)
      document.body.style.overflow = ''
    }
  }, [open, onClose])

  if (!open) return null

  return (
    <div className="fixed inset-0 z-[70] flex items-end justify-center p-0 sm:items-center sm:p-6">
      <button
        type="button"
        className="absolute inset-0 bg-ink/40 backdrop-blur-[2px]"
        aria-label="Close dialog"
        onClick={onClose}
      />
      <div
        className={cn(
          'relative max-h-[92vh] w-full overflow-y-auto rounded-t-3xl bg-cream p-6 shadow-soft animate-fade-up sm:rounded-3xl',
          wide ? 'sm:max-w-2xl' : 'sm:max-w-lg',
        )}
      >
        <div className="mb-5 flex items-start justify-between gap-4">
          <div>
            <h2 className="font-display text-2xl font-semibold tracking-tight">{title}</h2>
            {description ? <p className="mt-1 text-sm text-muted">{description}</p> : null}
          </div>
          <button
            type="button"
            onClick={onClose}
            className="rounded-full p-2 text-muted transition hover:bg-paper-2 hover:text-ink"
            aria-label="Close"
          >
            <X size={18} />
          </button>
        </div>
        {children}
      </div>
    </div>
  )
}
