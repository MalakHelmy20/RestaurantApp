import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

type ToastTone = 'success' | 'error' | 'info'
type Toast = { id: number; message: string; tone: ToastTone }

type ToastContextValue = {
  notify: (message: string, tone?: ToastTone) => void
}

const ToastContext = createContext<ToastContextValue | null>(null)

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])

  const notify = useCallback((message: string, tone: ToastTone = 'info') => {
    const id = Date.now() + Math.random()
    setToasts((current) => [...current, { id, message, tone }])
    window.setTimeout(() => {
      setToasts((current) => current.filter((toast) => toast.id !== id))
    }, 3200)
  }, [])

  const value = useMemo(() => ({ notify }), [notify])

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className="pointer-events-none fixed bottom-5 right-5 z-[80] flex w-[min(22rem,calc(100vw-2rem))] flex-col gap-2">
        {toasts.map((toast) => (
          <div
            key={toast.id}
            className={`pointer-events-auto rounded-2xl px-4 py-3 text-sm shadow-soft animate-fade-up ${
              toast.tone === 'error'
                ? 'bg-ink text-cream'
                : toast.tone === 'success'
                  ? 'bg-sage text-cream'
                  : 'bg-ink text-cream'
            }`}
          >
            {toast.message}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast() {
  const context = useContext(ToastContext)
  if (!context) throw new Error('useToast must be used within ToastProvider')
  return context
}
