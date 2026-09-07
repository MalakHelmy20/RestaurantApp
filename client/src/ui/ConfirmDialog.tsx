import { Button } from '@/ui/Button'
import { Modal } from '@/ui/Modal'

type ConfirmDialogProps = {
  open: boolean
  title: string
  description: string
  confirmLabel?: string
  danger?: boolean
  loading?: boolean
  onCancel: () => void
  onConfirm: () => void
}

export function ConfirmDialog({
  open,
  title,
  description,
  confirmLabel = 'Confirm',
  danger,
  loading,
  onCancel,
  onConfirm,
}: ConfirmDialogProps) {
  return (
    <Modal open={open} title={title} description={description} onClose={onCancel}>
      <div className="mt-6 flex justify-end gap-3">
        <Button variant="ghost" onClick={onCancel} disabled={loading}>
          Cancel
        </Button>
        <Button
          variant={danger ? 'danger' : 'secondary'}
          onClick={onConfirm}
          loading={loading}
        >
          {confirmLabel}
        </Button>
      </div>
    </Modal>
  )
}
