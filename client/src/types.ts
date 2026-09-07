export const USER_ROLES = ['SystemAdmin', 'Customer', 'RestaurantOwner'] as const
export type UserRole = (typeof USER_ROLES)[number]

export const USER_ROLE_VALUES = {
  SystemAdmin: 1,
  Customer: 2,
  RestaurantOwner: 3,
} as const

export const ORDER_STATUSES = [
  'confirm',
  'preparing',
  'onDelivery',
  'delivered',
  'cancelled',
] as const
export type OrderStatus = (typeof ORDER_STATUSES)[number]

export const ORDER_STATUS_VALUES = {
  confirm: 1,
  preparing: 2,
  onDelivery: 3,
  delivered: 4,
  cancelled: 5,
} as const

export type User = {
  id: string
  firstName: string
  lastName: string
  email: string
  phone: string
  role: UserRole | string
  address: string
}

export type LoginResponse = {
  user: User
  token: string
}

export type LoginRequest = {
  email: string
  password: string
}

export type RegisterRequest = {
  firstName: string
  lastName: string
  email: string
  phone: string
  password: string
  confirmPassword: string
  address?: string | null
}

export type CreateUserRequest = {
  firstName: string
  lastName: string
  email: string
  phone: string
  password: string
  role: number
  address?: string
}

export type UpdateUserRequest = {
  firstName?: string | null
  lastName?: string | null
  email?: string | null
  phone?: string | null
  password?: string | null
  address?: string | null
}

export type Category = {
  id: string
  name: string
  totalProducts: number
  restaurantId: string
}

export type Product = {
  id: string
  name: string
  description: string | null
  price: number
  isAvailable: boolean
  categoryId: string
  categoryName: string
  restaurantId: string
}

export type Restaurant = {
  id: string
  name: string
  description: string
  address: string
  phone: string
  openTime: string
  closeTime: string
  categories: Category[]
  products: Product[]
  owner: User | null
  averageRating: number
  totalRatingsCount: number
}

export type RestaurantSummary = {
  id: string
  name: string
  address: string
  averageRating: number
}

export type CreateRestaurantRequest = {
  name: string
  address: string
  phone: string
  description: string
  openTime: string
  closeTime: string
  categoryIds?: string[]
  ownerId: string
  productIds?: string[]
}

export type UpdateRestaurantRequest = {
  id?: string
  name?: string | null
  address?: string | null
  phone?: string | null
  description?: string | null
  openTime?: string | null
  closeTime?: string | null
  categoryIds?: string[] | null
  productIds?: string[] | null
  ownerId?: string | null
}

export type RestaurantFilter = {
  searchTerm?: string
  categoryId?: string
  productId?: string
  pageNumber?: number
  pageSize?: number
}

export type ProductFilter = {
  searchTerm?: string
  categoryId?: string
  restaurantId?: string
  price?: number
  isAvailable?: boolean
  pageNumber?: number
  pageSize?: number
}

export type CreateProductRequest = {
  name: string
  description?: string | null
  price: number
  isAvailable?: boolean
  categoryId: string
  restaurantId: string
}

export type UpdateProductRequest = {
  id: string
  name: string
  description?: string | null
  price: number
  categoryId: string
  isAvailable: boolean
}

export type CreateCategoryRequest = {
  name: string
  restaurantId: string
}

export type UpdateCategoryRequest = {
  id?: string
  name?: string | null
  restaurantId: string
}

export type OrderItem = {
  id: string
  productId: string
  productName: string
  quantity: number
  unitPrice: number
}

export type Order = {
  id: string
  status: OrderStatus | string
  orderDate: string
  totalPrice: number
  customerId: string
  customerName: string
  restaurantId: string
  restaurantName: string
  orderItems: OrderItem[]
}

export type CreateOrderItemRequest = {
  productId: string
  quantity: number
}

export type CreateOrderRequest = {
  restaurantId: string
  orderItems: CreateOrderItemRequest[]
}

export type UpdateOrderRequest = {
  status?: number | null
  orderItems?: CreateOrderItemRequest[] | null
}

export type Rating = {
  id: string
  userId: string
  userName: string
  restaurantId: string
  restaurantName: string
  ratingValue: number
}

export type CreateRatingRequest = {
  restaurantId: string
  ratingValue: number
}

export type UpdateRatingRequest = {
  ratingValue: number
}

export type CartItem = {
  productId: string
  name: string
  price: number
  quantity: number
  restaurantId: string
  restaurantName: string
}
