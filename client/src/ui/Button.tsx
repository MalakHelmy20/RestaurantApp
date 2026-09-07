import { cn } from '@/lib/cn'
import type { ButtonHTMLAttributes, ReactNode } from 'react'

type Variant = 'primary' | 'secondary' | 'ghost' | 'danger' | 'soft'
type Size = 'sm' | 'md' | 'lg'

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: Variant
  size?: Size
  loading?: boolean
  icon?: ReactNode
}

const variants: Record<Variant, string> = {
  primary:
    'bg-terracotta text-cream hover:bg-terracotta-dark disabled:bg-terracotta/50',
  secondary: 'bg-ink text-cream hover:bg-ink-soft disabled:bg-ink/50',
  ghost: 'bg-transparent text-ink hover:bg-paper-2 disabled:text-muted',
  danger: 'bg-danger text-cream hover:bg-danger/90 disabled:bg-danger/50',
  soft: 'bg-cream text-ink border border-line hover:border-ink/20 disabled:opacity-60',
}

const sizes: Record<Size, string> = {
  sm: 'h-9 px-3 text-sm',
  md: 'h-11 px-4 text-sm',
  lg: 'h-12 px-5 text-base',
}

export function Button({
  className,
  variant = 'primary',
  size = 'md',
  loading = false,
  icon,
  children,
  disabled,
  type = 'button',
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={cn(
        'inline-flex items-center justify-center gap-2 rounded-full font-medium transition duration-150 disabled:cursor-not-allowed',
        variants[variant],
        sizes[size],
        className,
      )}
      disabled={disabled || loading}
      {...props}
    >
      {loading ? (
        <span className="h-4 w-4 animate-spin rounded-full border-2 border-current border-t-transparent" />
      ) : (
        icon
      )}
      {children}
    </button>
  )
}
