import { cn } from '@/lib/cn'
import { useEffect, useState } from 'react'

type CoverImageProps = {
  src: string
  alt: string
  className?: string
}

export function CoverImage({ src, alt, className }: CoverImageProps) {
  const [failed, setFailed] = useState(false)

  useEffect(() => {
    setFailed(false)
  }, [src])

  if (failed) {
    return <div className={cn('bg-paper-2', className)} aria-hidden />
  }

  return (
    <img
      src={src}
      alt={alt}
      loading="lazy"
      referrerPolicy="no-referrer"
      onError={() => setFailed(true)}
      className={cn('bg-paper-2 object-cover', className)}
    />
  )
}
