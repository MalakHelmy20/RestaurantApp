import { deleteOrder, getOrder, updateOrder } from '@/api/orders'
import { useAuth } from '@/context/AuthContext'
import { useToast } from '@/context/ToastContext'
import { getErrorMessage } from '@/lib/errors'
import { dishImage } from '@/lib/foodImages'
import { formatDateTime, formatMoney, statusLabel } from '@/lib/format'
import { ORDER_STATUS_VALUES, type OrderStatus } from '@/types'
import { Button } from '@/ui/Button'
import { ConfirmDialog } from '@/ui/ConfirmDialog'
import { CoverImage } from '@/ui/CoverImage'
import { PageError, Skeleton } from '@/ui/Feedback'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'

const ownerFlow: OrderStatus[] = ['confirm', 'preparing', 'onDelivery', 'delivered']

export function OrderDetailPage() {
  const { id = '' } = useParams()
  const { hasRole } = useAuth()
  const { notify } = useToast()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const [confirmCancel, setConfirmCancel] = useState(false)
  const [confirmDelete, setConfirmDelete] = useState(false)

  const orderQuery = useQuery({
    queryKey: ['order', id],
    queryFn: () => getOrder(id),
    enabled: Boolean(id),
  })

  const invalidate = async () => {
    await Promise.all([
      queryClient.invalidateQueries({ queryKey: ['order', id] }),
      queryClient.invalidateQueries({ queryKey: ['orders'] }),
    ])
  }

  const statusMutation = useMutation({
    mutationFn: (status: number) => updateOrder(id, { status }),
    onSuccess: async () => {
      notify('Order updated.', 'success')
      await invalidate()
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const itemsMutation = useMutation({
    mutationFn: (orderItems: { productId: string; quantity: number }[]) =>
      updateOrder(id, { orderItems }),
    onSuccess: async () => {
      notify('Items updated.', 'success')
      await invalidate()
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  const deleteMutation = useMutation({
    mutationFn: () => deleteOrder(id),
    onSuccess: async () => {
      notify('Order removed.', 'success')
      await queryClient.invalidateQueries({ queryKey: ['orders'] })
      navigate('/orders')
    },
    onError: (error) => notify(getErrorMessage(error), 'error'),
  })

  if (orderQuery.isLoading) {
    return (
      <div className="mx-auto max-w-3xl px-5 py-12 sm:px-8">
        <Skeleton className="h-10 w-48" />
        <Skeleton className="mt-8 h-64" />
      </div>
    )
  }

  if (orderQuery.isError || !orderQuery.data) {
    return (
      <div className="mx-auto max-w-3xl px-5 py-12 sm:px-8">
        <PageError message="This order could not be loaded." onRetry={() => void orderQuery.refetch()} />
      </div>
    )
  }

  const order = orderQuery.data
  const canCustomerEdit = hasRole('Customer') && order.status === 'confirm'
  const canStaffUpdate = hasRole('RestaurantOwner', 'SystemAdmin')
  const canDelete = hasRole('SystemAdmin', 'Customer')

  return (
    <div className="mx-auto max-w-3xl px-5 py-10 sm:px-8 sm:py-14">
      <Link to="/orders" className="text-sm text-muted hover:text-ink">
        ← All orders
      </Link>
      <p className="mt-6 text-xs uppercase tracking-[0.24em] text-muted">
        {statusLabel(order.status)}
      </p>
      <h1 className="mt-2 font-display text-4xl">{order.restaurantName}</h1>
      <p className="mt-2 text-muted">
        {order.customerName} · {formatDateTime(order.orderDate)}
      </p>

      <div className="mt-8 divide-y divide-line border-y border-line">
        {order.orderItems.map((item) => (
          <div key={item.id} className="flex items-center justify-between gap-4 py-4">
            <div className="flex min-w-0 items-center gap-4">
              <CoverImage
                src={dishImage(item.productName, '', item.productId)}
                alt={item.productName}
                className="h-14 w-14 shrink-0 rounded-2xl"
              />
              <div>
                <p className="font-medium">{item.productName}</p>
                <p className="text-sm text-muted">
                  {item.quantity} × {formatMoney(item.unitPrice)}
                </p>
              </div>
            </div>
            {canCustomerEdit ? (
              <div className="flex items-center gap-2">
                <button
                  type="button"
                  className="h-9 w-9 rounded-full border border-line"
                  disabled={itemsMutation.isPending || item.quantity <= 1}
                  onClick={() =>
                    itemsMutation.mutate(
                      order.orderItems.map((entry) => ({
                        productId: entry.productId,
                        quantity:
                          entry.id === item.id ? Math.max(entry.quantity - 1, 1) : entry.quantity,
                      })),
                    )
                  }
                >
                  −
                </button>
                <button
                  type="button"
                  className="h-9 w-9 rounded-full border border-line"
                  disabled={itemsMutation.isPending || item.quantity >= 100}
                  onClick={() =>
                    itemsMutation.mutate(
                      order.orderItems.map((entry) => ({
                        productId: entry.productId,
                        quantity:
                          entry.id === item.id ? Math.min(entry.quantity + 1, 100) : entry.quantity,
                      })),
                    )
                  }
                >
                  +
                </button>
              </div>
            ) : (
              <p className="font-display">{formatMoney(item.unitPrice * item.quantity)}</p>
            )}
          </div>
        ))}
      </div>

      <p className="mt-6 font-display text-3xl">{formatMoney(order.totalPrice)}</p>

      {canStaffUpdate && order.status !== 'cancelled' && order.status !== 'delivered' ? (
        <div className="mt-8 flex flex-wrap gap-2">
          {ownerFlow
            .filter((status) => status !== order.status)
            .map((status) => (
              <Button
                key={status}
                variant="soft"
                size="sm"
                loading={statusMutation.isPending}
                onClick={() => statusMutation.mutate(ORDER_STATUS_VALUES[status])}
              >
                Mark {statusLabel(status).toLowerCase()}
              </Button>
            ))}
          <Button
            variant="ghost"
            size="sm"
            loading={statusMutation.isPending}
            onClick={() => statusMutation.mutate(ORDER_STATUS_VALUES.cancelled)}
          >
            Cancel
          </Button>
        </div>
      ) : null}

      {canCustomerEdit ? (
        <div className="mt-8 flex flex-wrap gap-3">
          <Button variant="soft" onClick={() => setConfirmCancel(true)}>
            Cancel order
          </Button>
        </div>
      ) : null}

      {canDelete ? (
        <button
          type="button"
          className="mt-6 text-sm text-muted hover:text-danger"
          onClick={() => setConfirmDelete(true)}
        >
          Delete this order
        </button>
      ) : null}

      <ConfirmDialog
        open={confirmCancel}
        title="Cancel this order?"
        description="You can only cancel while the kitchen still has it as confirmed."
        confirmLabel="Cancel order"
        danger
        loading={statusMutation.isPending}
        onCancel={() => setConfirmCancel(false)}
        onConfirm={() => {
          statusMutation.mutate(ORDER_STATUS_VALUES.cancelled, {
            onSettled: () => setConfirmCancel(false),
          })
        }}
      />
      <ConfirmDialog
        open={confirmDelete}
        title="Delete this order?"
        description="This removes the order record. It cannot be undone."
        confirmLabel="Delete"
        danger
        loading={deleteMutation.isPending}
        onCancel={() => setConfirmDelete(false)}
        onConfirm={() => deleteMutation.mutate()}
      />
    </div>
  )
}
