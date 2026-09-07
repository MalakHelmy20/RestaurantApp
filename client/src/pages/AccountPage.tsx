import { updateUser } from '@/api/users'
import { useAuth } from '@/context/AuthContext'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { fullName, roleLabel } from '@/lib/format'
import type { UpdateUserRequest } from '@/types'
import { Button } from '@/ui/Button'
import { Field, Input } from '@/ui/Field'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { z } from 'zod'

const schema = z
  .object({
    firstName: z.string().min(2, 'First name must be between 2 and 50 characters.').max(50),
    lastName: z.string().min(2, 'Last name must be between 2 and 50 characters.').max(50),
    email: z.string().min(1, 'Email is required.').email('Email is not valid.'),
    phone: z.string().min(1, 'Phone is required.'),
    address: z.string().max(200, 'Address cannot exceed 200 characters.').optional(),
    password: z.string(),
  })
  .superRefine((values, ctx) => {
    if (!values.password) return
    if (!/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$/.test(values.password)) {
      ctx.addIssue({
        code: 'custom',
        path: ['password'],
        message: 'Password must be 8+ characters with upper, lower, number, and a symbol.',
      })
    }
  })

type FormValues = z.infer<typeof schema>

export function AccountPage() {
  const { user, setUser } = useAuth()
  const { notify } = useToast()
  const [editing, setEditing] = useState(false)

  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      firstName: user?.firstName ?? '',
      lastName: user?.lastName ?? '',
      email: user?.email ?? '',
      phone: user?.phone ?? '',
      address: user?.address ?? '',
      password: '',
    },
  })

  const save = useMutation({
    mutationFn: async (values: FormValues) => {
      if (!user) throw new Error('You need to be signed in.')
      const payload: UpdateUserRequest = {
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phone: values.phone,
        address: values.address ?? '',
      }
      if (values.password) payload.password = values.password
      await updateUser(user.id, payload)
      return values
    },
    onSuccess: (values) => {
      if (!user) return
      setUser({
        ...user,
        firstName: values.firstName,
        lastName: values.lastName,
        email: values.email,
        phone: values.phone,
        address: values.address ?? '',
      })
      form.reset({ ...values, password: '' })
      setEditing(false)
      notify('Profile updated.', 'success')
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  if (!user) return null

  const openEdit = () => {
    form.reset({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phone: user.phone,
      address: user.address ?? '',
      password: '',
    })
    setEditing(true)
  }

  return (
    <div className="mx-auto max-w-3xl px-5 py-10 sm:px-8 sm:py-14">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.24em] text-muted">Account</p>
          <h1 className="mt-2 font-display text-4xl">{fullName(user.firstName, user.lastName)}</h1>
          <p className="mt-2 text-muted">{roleLabel(user.role)}</p>
        </div>
        {!editing ? (
          <Button onClick={openEdit}>Update profile</Button>
        ) : null}
      </div>

      {editing ? (
        <form
          className="mt-10 grid gap-4 sm:grid-cols-2"
          onSubmit={form.handleSubmit((values) => save.mutate(values))}
        >
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
          <Field label="Address" error={form.formState.errors.address?.message}>
            <Input autoComplete="street-address" {...form.register('address')} />
          </Field>
          <Field
            label="New password"
            hint="Leave blank to keep your current password."
            className="sm:col-span-2"
            error={form.formState.errors.password?.message}
          >
            <Input type="password" autoComplete="new-password" {...form.register('password')} />
          </Field>
          <div className="flex flex-wrap gap-3 sm:col-span-2">
            <Button type="submit" loading={save.isPending}>
              Save changes
            </Button>
            <Button
              type="button"
              variant="soft"
              disabled={save.isPending}
              onClick={() => {
                form.reset()
                setEditing(false)
              }}
            >
              Cancel
            </Button>
          </div>
        </form>
      ) : (
        <dl className="mt-10 divide-y divide-line border-y border-line">
          {[
            ['Email', user.email],
            ['Phone', user.phone],
            ['Address', user.address || '—'],
          ].map(([label, value]) => (
            <div key={label} className="grid gap-1 py-4 sm:grid-cols-[8rem_1fr]">
              <dt className="text-sm text-muted">{label}</dt>
              <dd>{value}</dd>
            </div>
          ))}
        </dl>
      )}
    </div>
  )
}
