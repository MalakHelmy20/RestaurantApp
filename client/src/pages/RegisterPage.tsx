import { useAuth } from '@/context/AuthContext'
import { getErrorMessage } from '@/lib/errors'
import { Button } from '@/ui/Button'
import { Field, Input } from '@/ui/Field'
import { zodResolver } from '@hookform/resolvers/zod'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { z } from 'zod'

const schema = z
  .object({
    firstName: z.string().min(2, 'First name must be between 2 and 50 characters.').max(50),
    lastName: z.string().min(2, 'Last name must be between 2 and 50 characters.').max(50),
    email: z.string().min(1, 'Email is required.').email('Email is not valid.'),
    phone: z.string().min(1, 'Phone is required.'),
    address: z.string().max(200, 'Address cannot exceed 200 characters.').optional(),
    password: z
      .string()
      .min(6, 'Password must be at least 6 characters long.')
      .regex(
        /^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=[\]{};':"\\|,.<>/?]).*$/,
        'Password must contain at least one uppercase letter and one special character.',
      ),
    confirmPassword: z.string().min(1, 'Confirm password is required.'),
  })
  .refine((values) => values.password === values.confirmPassword, {
    message: 'Passwords do not match.',
    path: ['confirmPassword'],
  })

type FormValues = z.infer<typeof schema>

export function RegisterPage() {
  const { register, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const [serverError, setServerError] = useState('')

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      address: '',
      password: '',
      confirmPassword: '',
    },
  })

  if (isAuthenticated) return <Navigate to="/" replace />

  const onSubmit = form.handleSubmit(async (values) => {
    setServerError('')
    try {
      await register({
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phone: values.phone,
        password: values.password,
        confirmPassword: values.confirmPassword,
        address: values.address || null,
      })
      navigate('/', { replace: true })
    } catch (error) {
      setServerError(getErrorMessage(error, 'Registration failed.'))
    }
  })

  return (
    <div className="mx-auto max-w-6xl px-5 py-12 sm:px-8">
      <div className="max-w-2xl">
        <p className="text-xs uppercase tracking-[0.24em] text-muted">Join Hearth</p>
        <h1 className="mt-3 font-display text-4xl sm:text-5xl">Create your place at the table</h1>
        <p className="mt-4 text-muted">
          New accounts join as guests. The first person to register becomes the system admin.
        </p>
      </div>
      <form onSubmit={onSubmit} className="mt-10 grid max-w-2xl gap-4 sm:grid-cols-2">
        {serverError ? (
          <p className="rounded-2xl bg-ink px-4 py-3 text-sm text-cream sm:col-span-2">
            {serverError}
          </p>
        ) : null}
        <Field label="First name" error={form.formState.errors.firstName?.message}>
          <Input autoComplete="given-name" {...form.register('firstName')} />
        </Field>
        <Field label="Last name" error={form.formState.errors.lastName?.message}>
          <Input autoComplete="family-name" {...form.register('lastName')} />
        </Field>
        <Field label="Email" className="sm:col-span-2" error={form.formState.errors.email?.message}>
          <Input type="email" autoComplete="email" {...form.register('email')} />
        </Field>
        <Field label="Phone" error={form.formState.errors.phone?.message}>
          <Input type="tel" autoComplete="tel" {...form.register('phone')} />
        </Field>
        <Field label="Address" hint="Optional" error={form.formState.errors.address?.message}>
          <Input autoComplete="street-address" {...form.register('address')} />
        </Field>
        <Field
          label="Password"
          hint="At least 6 characters, one uppercase letter, one special character."
          error={form.formState.errors.password?.message}
        >
          <Input type="password" autoComplete="new-password" {...form.register('password')} />
        </Field>
        <Field label="Confirm password" error={form.formState.errors.confirmPassword?.message}>
          <Input
            type="password"
            autoComplete="new-password"
            {...form.register('confirmPassword')}
          />
        </Field>
        <div className="sm:col-span-2">
          <Button type="submit" loading={form.formState.isSubmitting}>
            Create account
          </Button>
          <p className="mt-4 text-sm text-muted">
            Already have an account?{' '}
            <Link to="/login" className="text-terracotta hover:underline">
              Sign in
            </Link>
          </p>
        </div>
      </form>
    </div>
  )
}
