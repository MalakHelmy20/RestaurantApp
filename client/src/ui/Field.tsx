import { cn } from '@/lib/cn'
import type {
  InputHTMLAttributes,
  ReactNode,
  SelectHTMLAttributes,
  TextareaHTMLAttributes,
} from 'react'

type FieldProps = {
  label: string
  error?: string
  hint?: string
  className?: string
  children: ReactNode
}

export function Field({ label, error, hint, className, children }: FieldProps) {
  return (
    <label className={cn('block', className)}>
      <span className="mb-1.5 block text-sm font-medium text-ink-soft">{label}</span>
      {children}
      {error ? (
        <span className="mt-1.5 block text-sm text-danger">{error}</span>
      ) : hint ? (
        <span className="mt-1.5 block text-sm text-muted">{hint}</span>
      ) : null}
    </label>
  )
}

const controlClass =
  'w-full rounded-2xl border border-line bg-cream px-4 py-3 text-ink outline-none transition placeholder:text-muted/70 focus:border-terracotta'

export function Input({ className, ...props }: InputHTMLAttributes<HTMLInputElement>) {
  return <input className={cn(controlClass, className)} {...props} />
}

export function Textarea({
  className,
  ...props
}: TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea className={cn(controlClass, 'min-h-28 resize-y', className)} {...props} />
}

export function Select({ className, ...props }: SelectHTMLAttributes<HTMLSelectElement>) {
  return <select className={cn(controlClass, className)} {...props} />
}
