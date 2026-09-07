import { createUser, deleteUser, getUsers, updateUser } from '@/api/users'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { fullName, roleLabel } from '@/lib/format'
import { USER_ROLE_VALUES, USER_ROLES, type User, type UserRole } from '@/types'
import { Button } from '@/ui/Button'
import { ConfirmDialog } from '@/ui/ConfirmDialog'
import { Field, Input, Select } from '@/ui/Field'
import { EmptyState, PageError, Skeleton } from '@/ui/Feedback'
import { Modal } from '@/ui/Modal'
import { SearchBar } from '@/ui/SearchBar'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Users } from 'lucide-react'
import { useMemo, useState } from 'react'

const emptyForm = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: '',
  address: '',
  role: 'Customer' as UserRole,
}

export function UsersPage() {
  const { notify } = useToast()
  const queryClient = useQueryClient()
  const [draft, setDraft] = useState('')
  const [query, setQuery] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [open, setOpen] = useState(false)
  const [editing, setEditing] = useState<User | null>(null)
  const [form, setForm] = useState(emptyForm)
  const [deleting, setDeleting] = useState<User | null>(null)
  const trimmed = query.trim().toLowerCase()

  const usersQuery = useQuery({
    queryKey: ['users', roleFilter],
    queryFn: () => getUsers(roleFilter || undefined),
  })

  const people = useMemo(() => {
    const list = usersQuery.data ?? []
    if (!trimmed) return list
    return list.filter((person) => {
      const blob = [
        fullName(person.firstName, person.lastName),
        person.email,
        person.phone,
        roleLabel(person.role),
        person.role,
      ]
        .join(' ')
        .toLowerCase()
      return blob.includes(trimmed)
    })
  }, [trimmed, usersQuery.data])

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (editing) {
        await updateUser(editing.id, {
          firstName: form.firstName,
          lastName: form.lastName,
          email: form.email,
          phone: form.phone,
          address: form.address,
          password: form.password || null,
        })
        return
      }
      await createUser({
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        phone: form.phone,
        password: form.password,
        role: USER_ROLE_VALUES[form.role],
        address: form.address,
      })
    },
    onSuccess: async () => {
      notify(editing ? 'Person updated.' : 'Person added.', 'success')
      setOpen(false)
      setEditing(null)
      setForm(emptyForm)
      await queryClient.invalidateQueries({ queryKey: ['users'] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const deleteMutation = useMutation({
    mutationFn: (userId: string) => deleteUser(userId),
    onSuccess: async () => {
      notify('Person removed.', 'success')
      setDeleting(null)
      await queryClient.invalidateQueries({ queryKey: ['users'] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const openCreate = () => {
    setEditing(null)
    setForm(emptyForm)
    setOpen(true)
  }

  const openEdit = (user: User) => {
    setEditing(user)
    setForm({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      phone: user.phone,
      password: '',
      address: user.address,
      role: (USER_ROLES.includes(user.role as UserRole) ? user.role : 'Customer') as UserRole,
    })
    setOpen(true)
  }

  return (
    <div className="mx-auto max-w-6xl px-5 py-10 sm:px-8 sm:py-14">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-xs uppercase tracking-[0.24em] text-muted">Admin</p>
          <h1 className="mt-2 font-display text-4xl">People</h1>
          <p className="mt-2 text-muted">Guests, owners, and admins. Roles can be set when creating.</p>
        </div>
        <Button onClick={openCreate}>Add person</Button>
      </div>

      <SearchBar
        value={draft}
        onChange={setDraft}
        onSearch={() => setQuery(draft)}
        placeholder="Search by name, email, phone, or role"
        label="Search people"
      />

      <div className="mt-5 flex flex-wrap gap-2">
        {['', ...USER_ROLES].map((role) => (
          <button
            key={role || 'all'}
            type="button"
            onClick={() => setRoleFilter(role)}
            className={`rounded-full px-3 py-1.5 text-sm ${
              roleFilter === role ? 'bg-ink text-cream' : 'bg-cream text-ink-soft'
            }`}
          >
            {role ? roleLabel(role) : 'All'}
          </button>
        ))}
      </div>

      <div className="mt-8">
        {usersQuery.isLoading ? (
          <Skeleton className="h-40" />
        ) : usersQuery.isError ? (
          <PageError message="People could not be loaded." onRetry={() => void usersQuery.refetch()} />
        ) : !usersQuery.data || usersQuery.data.length === 0 ? (
          <EmptyState icon={Users} title="No people found" description="Try another role filter." />
        ) : people.length === 0 ? (
          <EmptyState
            icon={Users}
            title="No people match"
            description="Try another name, email, phone, or role filter."
          />
        ) : (
          <div className="divide-y divide-line border-y border-line">
            {people.map((person) => (
              <div
                key={person.id}
                className="flex flex-col gap-3 py-4 sm:flex-row sm:items-center sm:justify-between"
              >
                <div>
                  <p className="font-medium">{fullName(person.firstName, person.lastName)}</p>
                  <p className="text-sm text-muted">
                    {person.email} · {person.phone} · {roleLabel(person.role)}
                  </p>
                </div>
                <div className="flex gap-3 text-sm">
                  <button type="button" className="text-muted hover:text-ink" onClick={() => openEdit(person)}>
                    Edit
                  </button>
                  <button
                    type="button"
                    className="text-muted hover:text-danger"
                    onClick={() => setDeleting(person)}
                  >
                    Remove
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <Modal
        open={open}
        title={editing ? 'Edit person' : 'Add person'}
        description={
          editing
            ? 'Role cannot be changed after creation.'
            : 'Choose a role. Owners can open kitchens; guests can order.'
        }
        onClose={() => setOpen(false)}
      >
        <div className="grid gap-3 sm:grid-cols-2">
          <Field label="First name">
            <Input
              value={form.firstName}
              onChange={(event) => setForm((current) => ({ ...current, firstName: event.target.value }))}
            />
          </Field>
          <Field label="Last name">
            <Input
              value={form.lastName}
              onChange={(event) => setForm((current) => ({ ...current, lastName: event.target.value }))}
            />
          </Field>
          <Field label="Email" className="sm:col-span-2">
            <Input
              type="email"
              value={form.email}
              onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
            />
          </Field>
          <Field label="Phone">
            <Input
              value={form.phone}
              onChange={(event) => setForm((current) => ({ ...current, phone: event.target.value }))}
            />
          </Field>
          {!editing ? (
            <Field label="Role">
              <Select
                value={form.role}
                onChange={(event) =>
                  setForm((current) => ({ ...current, role: event.target.value as UserRole }))
                }
              >
                {USER_ROLES.map((role) => (
                  <option key={role} value={role}>
                    {roleLabel(role)}
                  </option>
                ))}
              </Select>
            </Field>
          ) : null}
          <Field label="Address" className="sm:col-span-2">
            <Input
              value={form.address}
              onChange={(event) => setForm((current) => ({ ...current, address: event.target.value }))}
            />
          </Field>
          <Field
            label={editing ? 'New password' : 'Password'}
            hint={editing ? 'Leave blank to keep the current password.' : undefined}
            className="sm:col-span-2"
          >
            <Input
              type="password"
              value={form.password}
              onChange={(event) => setForm((current) => ({ ...current, password: event.target.value }))}
            />
          </Field>
        </div>
        <Button className="mt-5 w-full" onClick={() => saveMutation.mutate()} loading={saveMutation.isPending}>
          Save
        </Button>
      </Modal>

      <ConfirmDialog
        open={Boolean(deleting)}
        title="Remove this person?"
        description="They will no longer be able to sign in."
        confirmLabel="Remove"
        danger
        loading={deleteMutation.isPending}
        onCancel={() => setDeleting(null)}
        onConfirm={() => deleting && deleteMutation.mutate(deleting.id)}
      />
    </div>
  )
}
