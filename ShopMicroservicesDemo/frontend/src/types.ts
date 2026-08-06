export interface RemoteUser {
  id: string
  name: string
  email: string
  active: boolean
}

export interface Order {
  id: string
  userId: string
  product: string
  quantity: number
  status: 'Created' | 'Rejected'
  message: string
  createdAt: string
}

export interface CreateOrderPayload {
  userId: string
  product: string
  quantity: number
}
