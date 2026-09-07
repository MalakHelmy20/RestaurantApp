import { cn } from '@/lib/cn'
import { Star } from 'lucide-react'

type StarsProps = {
  value: number
  count?: number
  size?: number
  onChange?: (value: number) => void
}

export function Stars({ value, count = 0, size = 16, onChange }: StarsProps) {
  return (
    <div className="inline-flex items-center gap-2">
      <div className="flex items-center gap-0.5">
        {Array.from({ length: 5 }, (_, index) => {
          const filled = index < Math.round(value)
          const starValue = index + 1
          const className = cn(
            filled ? 'fill-gold text-gold' : 'text-line',
            onChange && 'cursor-pointer transition hover:scale-110',
          )
          if (onChange) {
            return (
              <button
                key={starValue}
                type="button"
                onClick={() => onChange(starValue)}
                aria-label={`${starValue} star${starValue === 1 ? '' : 's'}`}
              >
                <Star size={size} className={className} />
              </button>
            )
          }
          return <Star key={starValue} size={size} className={className} />
        })}
      </div>
      {count > 0 ? (
        <span className="text-sm text-muted">
          {value.toFixed(1)} · {count}
        </span>
      ) : value > 0 ? (
        <span className="text-sm text-muted">{value.toFixed(1)}</span>
      ) : null}
    </div>
  )
}
