import { useAuth } from '@/context/AuthContext'
import { getErrorMessage } from '@/lib/errors'
import { Button } from '@/ui/Button'
import { Field, Input } from '@/ui/Field'
import { zodResolver } from '@hookform/resolvers/zod'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { z } from 'zod'

const schema = z.object({
  email: z.string().min(1, 'Email address is required.').email('Invalid email format.'),
  password: z.string().min(1, 'Password is required.'),
})

type FormValues = z.infer<typeof schema>

export function LoginPage() {
  const { login, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()
  const [serverError, setServerError] = useState('')
  const from = (location.state as { from?: string } | null)?.from ?? '/'

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { email: '', password: '' },
  })

  if (isAuthenticated) return <Navigate to={from} replace />

  const onSubmit = form.handleSubmit(async (values) => {
    setServerError('')
    try {
      await login(values)
      navigate(from, { replace: true })
    } catch (error) {
      setServerError(getErrorMessage(error, 'Invalid email or password.'))
    }
  })

  return (
    <div className="mx-auto grid min-h-[70vh] max-w-6xl items-center gap-12 px-5 py-12 sm:px-8 lg:grid-cols-2">
      <div>
        <p className="text-xs uppercase tracking-[0.24em] text-muted">Welcome back</p>
        <h1 className="mt-3 font-display text-4xl sm:text-5xl">Sign in to Hearth</h1>
        <p className="mt-4 max-w-md text-muted">
          Pick up your bag, follow an order, or step into the kitchen.
        </p>
      </div>
      <form onSubmit={onSubmit} className="max-w-md space-y-4">
        {serverError ? (
          <p className="rounded-2xl bg-ink px-4 py-3 text-sm leading-relaxed text-cream">
            {serverError}
          </p>
        ) : null}
        <Field label="Email" error={form.formState.errors.email?.message}>
          <Input type="email" autoComplete="email" {...form.register('email')} />
        </Field>
        <Field label="Password" error={form.formState.errors.password?.message}>
          <Input type="password" autoComplete="current-password" {...form.register('password')} />
        </Field>
        <Button type="submit" className="w-full" loading={form.formState.isSubmitting}>
          Sign in
        </Button>
        <p className="text-sm text-muted">
          New here?{' '}
            <Link to="/register" className="font-medium text-terracotta hover:underline">
              Create an account
            </Link>
        </p>
      </form>
    </div>
  )
}
