import { createCategory, deleteCategory, updateCategory } from '@/api/categories'
import { createProduct, deleteProduct, updateProduct } from '@/api/products'
import {
  createRestaurant,
  deleteRestaurant,
  getRestaurant,
  updateRestaurant,
} from '@/api/restaurants'
import { getUsers } from '@/api/users'
import { useAuth } from '@/context/AuthContext'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { dishImage } from '@/lib/foodImages'
import { formatMoney, fromTimeInput, toTimeInput } from '@/lib/format'
import type { Category, Product } from '@/types'
import { Button } from '@/ui/Button'
import { ConfirmDialog } from '@/ui/ConfirmDialog'
import { CoverImage } from '@/ui/CoverImage'
import { Field, Input, Select, Textarea } from '@/ui/Field'
import { PageError, Skeleton } from '@/ui/Feedback'
import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { useForm } from 'react-hook-form'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { z } from 'zod'

const restaurantSchema = z.object({
  name: z.string().min(1, 'Restaurant name is required.'),
  address: z.string().min(1, 'Restaurant address is required.'),
  phone: z.string().min(1, 'Restaurant phone number is required.'),
  description: z.string().min(1, 'Restaurant description is required.'),
  openTime: z.string().min(1, 'Open time is required.'),
  closeTime: z.string().min(1, 'Close time is required.'),
  ownerId: z.string().min(1, 'Owner is required.'),
})

type RestaurantForm = z.infer<typeof restaurantSchema>

export function KitchenRestaurantPage() {
  const { id } = useParams()
  const isNew = !id
  const { user, hasRole } = useAuth()
  const { notify } = useToast()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [deleteOpen, setDeleteOpen] = useState(false)
  const [categoryName, setCategoryName] = useState('')
  const [editingCategory, setEditingCategory] = useState<Category | null>(null)
  const [productForm, setProductForm] = useState({
    name: '',
    description: '',
    price: '',
    categoryId: '',
    isAvailable: true,
  })
  const [editingProduct, setEditingProduct] = useState<Product | null>(null)

  const restaurantQuery = useQuery({
    queryKey: ['restaurant', id],
    queryFn: () => getRestaurant(id!),
    enabled: Boolean(id),
  })

  const ownersQuery = useQuery({
    queryKey: ['users', 'RestaurantOwner'],
    queryFn: () => getUsers('RestaurantOwner'),
    enabled: isNew && hasRole('SystemAdmin'),
  })

  const form = useForm<RestaurantForm>({
    resolver: zodResolver(restaurantSchema),
    defaultValues: {
      name: '',
      address: '',
      phone: '',
      description: '',
      openTime: '09:00',
      closeTime: '22:00',
      ownerId: user?.id ?? '',
    },
  })

  useEffect(() => {
    const restaurant = restaurantQuery.data
    if (!restaurant) return
    form.reset({
      name: restaurant.name,
      address: restaurant.address,
      phone: restaurant.phone,
      description: restaurant.description,
      openTime: toTimeInput(restaurant.openTime),
      closeTime: toTimeInput(restaurant.closeTime),
      ownerId: restaurant.owner?.id ?? user?.id ?? '',
    })
  }, [form, restaurantQuery.data, user?.id])

  const saveRestaurant = useMutation({
    mutationFn: async (values: RestaurantForm) => {
      const payload = {
        name: values.name,
        address: values.address,
        phone: values.phone,
        description: values.description,
        openTime: fromTimeInput(values.openTime),
        closeTime: fromTimeInput(values.closeTime),
        ownerId: hasRole('SystemAdmin') ? values.ownerId : user!.id,
        categoryIds: [],
        productIds: [],
      }
      if (isNew) {
        return createRestaurant(payload)
      }
      await updateRestaurant(id!, {
        name: payload.name,
        address: payload.address,
        phone: payload.phone,
        description: payload.description,
        openTime: payload.openTime,
        closeTime: payload.closeTime,
        ownerId: hasRole('SystemAdmin') ? payload.ownerId : undefined,
      })
      return getRestaurant(id!)
    },
    onSuccess: async (restaurant) => {
      notify(isNew ? 'Kitchen opened.' : 'Kitchen updated.', 'success')
      await queryClient.invalidateQueries({ queryKey: ['restaurants'] })
      if (isNew) navigate(`/kitchen/${restaurant.id}`)
      else await queryClient.invalidateQueries({ queryKey: ['restaurant', id] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const removeRestaurant = useMutation({
    mutationFn: () => deleteRestaurant(id!),
    onSuccess: async () => {
      notify('Kitchen removed.', 'success')
      await queryClient.invalidateQueries({ queryKey: ['restaurants'] })
      navigate('/kitchen')
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const categoryMutation = useMutation({
    mutationFn: async () => {
      if (!id) throw new Error('Save the kitchen first.')
      if (editingCategory) {
        await updateCategory(editingCategory.id, {
          name: categoryName,
          restaurantId: id,
        })
        return
      }
      await createCategory({ name: categoryName, restaurantId: id })
    },
    onSuccess: async () => {
      notify(editingCategory ? 'Category updated.' : 'Category added.', 'success')
      setCategoryName('')
      setEditingCategory(null)
      await queryClient.invalidateQueries({ queryKey: ['restaurant', id] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const deleteCategoryMutation = useMutation({
    mutationFn: (categoryId: string) => deleteCategory(categoryId),
    onSuccess: async () => {
      notify('Category removed.', 'success')
      await queryClient.invalidateQueries({ queryKey: ['restaurant', id] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const productMutation = useMutation({
    mutationFn: async () => {
      if (!id) throw new Error('Save the kitchen first.')
      const price = Number(productForm.price)
      if (editingProduct) {
        await updateProduct(editingProduct.id, {
          id: editingProduct.id,
          name: productForm.name,
          description: productForm.description || null,
          price,
          categoryId: productForm.categoryId,
          isAvailable: productForm.isAvailable,
        })
        return
      }
      await createProduct({
        name: productForm.name,
        description: productForm.description || null,
        price,
        isAvailable: productForm.isAvailable,
        categoryId: productForm.categoryId,
        restaurantId: id,
      })
    },
    onSuccess: async () => {
      notify(editingProduct ? 'Dish updated.' : 'Dish added.', 'success')
      setProductForm({
        name: '',
        description: '',
        price: '',
        categoryId: restaurantQuery.data?.categories[0]?.id ?? '',
        isAvailable: true,
      })
      setEditingProduct(null)
      await queryClient.invalidateQueries({ queryKey: ['restaurant', id] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const deleteProductMutation = useMutation({
    mutationFn: (productId: string) => deleteProduct(productId),
    onSuccess: async () => {
      notify('Dish removed.', 'success')
      await queryClient.invalidateQueries({ queryKey: ['restaurant', id] })
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  if (!isNew && restaurantQuery.isLoading) {
    return (
      <div className="mx-auto max-w-4xl px-5 py-12 sm:px-8">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="mt-8 h-80" />
      </div>
    )
  }

  if (!isNew && (restaurantQuery.isError || !restaurantQuery.data)) {
    return (
      <div className="mx-auto max-w-4xl px-5 py-12 sm:px-8">
        <PageError message="This kitchen could not be loaded." />
      </div>
    )
  }

  const restaurant = restaurantQuery.data
  const categories = restaurant?.categories ?? []

  return (
    <div className="mx-auto max-w-4xl px-5 py-10 sm:px-8 sm:py-14">
      <Link to="/kitchen" className="text-sm text-muted hover:text-ink">
        ← Kitchen
      </Link>
      <h1 className="mt-4 font-display text-4xl">{isNew ? 'Open a kitchen' : restaurant?.name}</h1>
      <p className="mt-2 text-muted">
        {isNew
          ? 'Hours, address, and a short description. Menu comes after you save.'
          : 'Keep the house details, categories, and dishes in one place.'}
      </p>

      <form
        className="mt-8 grid gap-4 sm:grid-cols-2"
        onSubmit={form.handleSubmit((values) => saveRestaurant.mutate(values))}
      >
        <Field label="Name" className="sm:col-span-2" error={form.formState.errors.name?.message}>
          <Input {...form.register('name')} />
        </Field>
        <Field label="Address" className="sm:col-span-2" error={form.formState.errors.address?.message}>
          <Input {...form.register('address')} />
        </Field>
        <Field label="Phone" error={form.formState.errors.phone?.message}>
          <Input {...form.register('phone')} />
        </Field>
        {isNew && hasRole('SystemAdmin') ? (
          <Field label="Owner" error={form.formState.errors.ownerId?.message}>
            <Select {...form.register('ownerId')}>
              <option value="">Select an owner</option>
              {user ? (
                <option value={user.id}>
                  {user.firstName} {user.lastName} (you)
                </option>
              ) : null}
              {(ownersQuery.data ?? [])
                .filter((owner) => owner.id !== user?.id)
                .map((owner) => (
                  <option key={owner.id} value={owner.id}>
                    {owner.firstName} {owner.lastName}
                  </option>
                ))}
            </Select>
          </Field>
        ) : (
          <input type="hidden" {...form.register('ownerId')} />
        )}
        <Field label="Opens" error={form.formState.errors.openTime?.message}>
          <Input type="time" {...form.register('openTime')} />
        </Field>
        <Field label="Closes" error={form.formState.errors.closeTime?.message}>
          <Input type="time" {...form.register('closeTime')} />
        </Field>
        <Field
          label="Description"
          className="sm:col-span-2"
          error={form.formState.errors.description?.message}
        >
          <Textarea {...form.register('description')} />
        </Field>
        <div className="sm:col-span-2">
          <Button type="submit" loading={saveRestaurant.isPending}>
            {isNew ? 'Create kitchen' : 'Save details'}
          </Button>
        </div>
      </form>

      {!isNew && restaurant ? (
        <>
          <section className="mt-16">
            <h2 className="font-display text-2xl">Categories</h2>
            <p className="mt-1 text-sm text-muted">Dishes need a category before they can be added.</p>
            <div className="mt-4 flex flex-col gap-3 sm:flex-row">
              <Input
                value={categoryName}
                onChange={(event) => setCategoryName(event.target.value)}
                placeholder="Category name"
              />
              <Button
                onClick={() => categoryMutation.mutate()}
                loading={categoryMutation.isPending}
                disabled={categoryName.trim().length < 2}
              >
                {editingCategory ? 'Update' : 'Add'}
              </Button>
            </div>
            <div className="mt-5 divide-y divide-line border-y border-line">
              {categories.length === 0 ? (
                <p className="py-4 text-sm text-muted">No categories yet.</p>
              ) : (
                categories.map((category) => (
                  <div key={category.id} className="flex items-center justify-between gap-3 py-3">
                    <p>
                      {category.name}
                      <span className="ml-2 text-sm text-muted">{category.totalProducts}</span>
                    </p>
                    <div className="flex gap-3 text-sm">
                      <button
                        type="button"
                        className="text-muted hover:text-ink"
                        onClick={() => {
                          setEditingCategory(category)
                          setCategoryName(category.name)
                        }}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className="text-muted hover:text-danger"
                        onClick={() => deleteCategoryMutation.mutate(category.id)}
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </section>

          <section className="mt-16">
            <h2 className="font-display text-2xl">Menu</h2>
            {productForm.name ? (
              <div className="mt-4 flex items-center gap-3 text-sm text-muted">
                <CoverImage
                  src={dishImage(
                    productForm.name,
                    categories.find((category) => category.id === productForm.categoryId)?.name ?? '',
                    editingProduct?.id ?? productForm.name,
                    productForm.description,
                  )}
                  alt={productForm.name}
                  className="h-14 w-14 rounded-2xl"
                />
                Photo preview for this dish
              </div>
            ) : null}
            <div className="mt-4 grid gap-3 sm:grid-cols-2">
              <Field label="Dish name">
                <Input
                  value={productForm.name}
                  onChange={(event) =>
                    setProductForm((current) => ({ ...current, name: event.target.value }))
                  }
                />
              </Field>
              <Field label="Price">
                <Input
                  type="number"
                  min="0.01"
                  step="0.01"
                  value={productForm.price}
                  onChange={(event) =>
                    setProductForm((current) => ({ ...current, price: event.target.value }))
                  }
                />
              </Field>
              <Field label="Category">
                <Select
                  value={productForm.categoryId}
                  onChange={(event) =>
                    setProductForm((current) => ({ ...current, categoryId: event.target.value }))
                  }
                >
                  <option value="">Select a category</option>
                  {categories.map((category) => (
                    <option key={category.id} value={category.id}>
                      {category.name}
                    </option>
                  ))}
                </Select>
              </Field>
              <Field label="Available">
                <Select
                  value={productForm.isAvailable ? 'yes' : 'no'}
                  onChange={(event) =>
                    setProductForm((current) => ({
                      ...current,
                      isAvailable: event.target.value === 'yes',
                    }))
                  }
                >
                  <option value="yes">Yes</option>
                  <option value="no">No</option>
                </Select>
              </Field>
              <Field label="Description" className="sm:col-span-2">
                <Textarea
                  value={productForm.description}
                  onChange={(event) =>
                    setProductForm((current) => ({ ...current, description: event.target.value }))
                  }
                />
              </Field>
            </div>
            <Button
              className="mt-4"
              onClick={() => productMutation.mutate()}
              loading={productMutation.isPending}
              disabled={!productForm.name || !productForm.price || !productForm.categoryId}
            >
              {editingProduct ? 'Update dish' : 'Add dish'}
            </Button>

            <div className="mt-6 divide-y divide-line border-y border-line">
              {restaurant.products.length === 0 ? (
                <p className="py-4 text-sm text-muted">No dishes yet.</p>
              ) : (
                restaurant.products.map((product) => (
                  <div
                    key={product.id}
                    className="flex flex-col gap-3 py-4 sm:flex-row sm:items-center sm:justify-between"
                  >
                    <div className="flex min-w-0 items-center gap-4">
                      <CoverImage
                        src={dishImage(
                          product.name,
                          product.categoryName,
                          product.id,
                          product.description ?? '',
                        )}
                        alt={product.name}
                        className="h-16 w-16 shrink-0 rounded-2xl"
                      />
                      <div className="min-w-0">
                        <p className="font-medium">{product.name}</p>
                        <p className="text-sm text-muted">
                          {product.categoryName} · {formatMoney(product.price)}
                          {product.isAvailable ? '' : ' · Unavailable'}
                        </p>
                      </div>
                    </div>
                    <div className="flex gap-3 text-sm">
                      <button
                        type="button"
                        className="text-muted hover:text-ink"
                        onClick={() => {
                          setEditingProduct(product)
                          setProductForm({
                            name: product.name,
                            description: product.description ?? '',
                            price: String(product.price),
                            categoryId: product.categoryId,
                            isAvailable: product.isAvailable,
                          })
                        }}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className="text-muted hover:text-danger"
                        onClick={() => deleteProductMutation.mutate(product.id)}
                      >
                        Remove
                      </button>
                    </div>
                  </div>
                ))
              )}
            </div>
          </section>

          {hasRole('SystemAdmin') ? (
            <button
              type="button"
              className="mt-12 text-sm text-muted hover:text-danger"
              onClick={() => setDeleteOpen(true)}
            >
              Delete this kitchen
            </button>
          ) : null}
        </>
      ) : null}

      <ConfirmDialog
        open={deleteOpen}
        title="Delete this kitchen?"
        description="It will be removed from discovery. This cannot be undone from the app."
        confirmLabel="Delete kitchen"
        danger
        loading={removeRestaurant.isPending}
        onCancel={() => setDeleteOpen(false)}
        onConfirm={() => removeRestaurant.mutate()}
      />
    </div>
  )
}
