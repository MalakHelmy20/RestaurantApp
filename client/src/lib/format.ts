import type { OrderStatus, UserRole } from '@/types'

export function formatMoney(value: number) {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
  }).format(value)
}

export function formatTimeSpan(value: string | null | undefined) {
  if (!value) return '—'
  const match = value.match(/(\d{1,2}):(\d{2})/)
  if (!match) return value
  const hours = Number(match[1])
  const minutes = match[2]
  const suffix = hours >= 12 ? 'PM' : 'AM'
  const hour12 = hours % 12 || 12
  return `${hour12}:${minutes} ${suffix}`
}

export function toTimeInput(value: string | null | undefined) {
  if (!value) return ''
  const match = value.match(/(\d{1,2}):(\d{2})/)
  if (!match) return ''
  return `${String(Number(match[1])).padStart(2, '0')}:${match[2]}`
}

export function fromTimeInput(value: string) {
  if (!value) return ''
  return `${value}:00`
}

export function formatDateTime(value: string) {
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  }).format(date)
}

export function fullName(first: string, last: string) {
  return `${first} ${last}`.trim()
}

export function isOpenNow(openTime: string, closeTime: string) {
  const now = new Date()
  const current = now.getHours() * 60 + now.getMinutes()
  const openMatch = openTime.match(/(\d{1,2}):(\d{2})/)
  const closeMatch = closeTime.match(/(\d{1,2}):(\d{2})/)
  if (!openMatch || !closeMatch) return false
  const open = Number(openMatch[1]) * 60 + Number(openMatch[2])
  const close = Number(closeMatch[1]) * 60 + Number(closeMatch[2])
  if (open === close) return false
  if (open < close) return current >= open && current < close
  return current >= open || current < close
}

export function roleLabel(role: UserRole | string) {
  if (role === 'SystemAdmin') return 'Admin'
  if (role === 'RestaurantOwner') return 'Owner'
  if (role === 'Customer') return 'Guest'
  return role
}

export function statusLabel(status: OrderStatus | string) {
  switch (status) {
    case 'confirm':
      return 'Confirmed'
    case 'preparing':
      return 'Preparing'
    case 'onDelivery':
      return 'On the way'
    case 'delivered':
      return 'Delivered'
    case 'cancelled':
      return 'Cancelled'
    default:
      return status
  }
}
