import { Button } from '@/ui/Button'
import { Input } from '@/ui/Field'
import { Search } from 'lucide-react'
import type { FormEvent } from 'react'

type SearchBarProps = {
  value: string
  onChange: (value: string) => void
  onSearch: () => void
  placeholder: string
  label: string
}

export function SearchBar({ value, onChange, onSearch, placeholder, label }: SearchBarProps) {
  const submit = (event: FormEvent) => {
    event.preventDefault()
    onSearch()
  }

  return (
    <form className="mt-8 flex max-w-2xl items-center gap-2" onSubmit={submit}>
      <div className="relative min-w-0 flex-1">
        <Search size={18} className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-muted" />
        <Input
          value={value}
          onChange={(event) => onChange(event.target.value)}
          placeholder={placeholder}
          className="h-11 pl-11"
          aria-label={label}
        />
      </div>
      <Button type="submit" variant="secondary" className="shrink-0">
        Search
      </Button>
    </form>
  )
}
