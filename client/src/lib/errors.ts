import axios from 'axios'

type ValidationProblem = {
  title?: string
  message?: string
  errors?: Record<string, string[]>
}

function tidyMessage(raw: string) {
  const firstLine = raw.replace(/<[^>]+>/g, ' ').split('\n')[0]?.trim() ?? ''
  const withoutType = firstLine.includes(': ')
    ? firstLine.slice(firstLine.indexOf(': ') + 2)
    : firstLine
  const cleaned = withoutType.split(' at ')[0].replace(/\s+/g, ' ').trim()
  if (!cleaned) return ''
  if (/Exception| at .*Controller|stack/i.test(raw) && cleaned.length > 80) return ''
  return cleaned.length > 180 ? `${cleaned.slice(0, 177)}…` : cleaned
}

export function getErrorMessage(error: unknown, fallback = 'Something went wrong.') {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ValidationProblem | string | undefined
    if (typeof data === 'string' && data.trim()) {
      return tidyMessage(data) || fallback
    }
    if (data && typeof data === 'object') {
      if (data.errors) {
        const first = Object.values(data.errors).flat()[0]
        if (first) return tidyMessage(first)
      }
      if (data.message) return tidyMessage(data.message)
      if (data.title) return tidyMessage(data.title)
    }
    if (error.response?.status === 401) return 'Invalid email or password.'
    if (error.response?.status === 403) return 'You do not have permission to do that.'
  }
  if (error instanceof Error && error.message) return tidyMessage(error.message) || fallback
  return fallback
}
