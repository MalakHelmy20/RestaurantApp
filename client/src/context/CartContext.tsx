import { storage } from '@/lib/storage'
import type { CartItem, Product, Restaurant } from '@/types'
import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

type CartContextValue = {
  items: CartItem[]
  restaurantId: string | null
  restaurantName: string | null
  count: number
  total: number
  addItem: (product: Product, restaurant: Restaurant) => { replaced: boolean }
  setQuantity: (productId: string, quantity: number) => void
  removeItem: (productId: string) => void
  clear: () => void
}

const CartContext = createContext<CartContextValue | null>(null)

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>(() => storage.getCart<CartItem[]>([]))

  const persist = useCallback((next: CartItem[]) => {
    setItems(next)
    storage.setCart(next)
  }, [])

  const addItem = useCallback(
    (product: Product, restaurant: Restaurant) => {
      const currentRestaurant = items[0]?.restaurantId
      let next = items
      let replaced = false
      if (currentRestaurant && currentRestaurant !== restaurant.id) {
        next = []
        replaced = true
      }
      const existing = next.find((item) => item.productId === product.id)
      if (existing) {
        persist(
          next.map((item) =>
            item.productId === product.id
              ? { ...item, quantity: Math.min(item.quantity + 1, 100) }
              : item,
          ),
        )
      } else {
        persist([
          ...next,
          {
            productId: product.id,
            name: product.name,
            price: product.price,
            quantity: 1,
            restaurantId: restaurant.id,
            restaurantName: restaurant.name,
          },
        ])
      }
      return { replaced }
    },
    [items, persist],
  )

  const setQuantity = useCallback(
    (productId: string, quantity: number) => {
      if (quantity < 1) {
        persist(items.filter((item) => item.productId !== productId))
        return
      }
      persist(
        items.map((item) =>
          item.productId === productId ? { ...item, quantity: Math.min(quantity, 100) } : item,
        ),
      )
    },
    [items, persist],
  )

  const removeItem = useCallback(
    (productId: string) => persist(items.filter((item) => item.productId !== productId)),
    [items, persist],
  )

  const clear = useCallback(() => persist([]), [persist])

  const value = useMemo<CartContextValue>(
    () => ({
      items,
      restaurantId: items[0]?.restaurantId ?? null,
      restaurantName: items[0]?.restaurantName ?? null,
      count: items.reduce((sum, item) => sum + item.quantity, 0),
      total: items.reduce((sum, item) => sum + item.price * item.quantity, 0),
      addItem,
      setQuantity,
      removeItem,
      clear,
    }),
    [items, addItem, setQuantity, removeItem, clear],
  )

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>
}

export function useCart() {
  const context = useContext(CartContext)
  if (!context) throw new Error('useCart must be used within CartProvider')
  return context
}
