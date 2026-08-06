import type { CreateOrderPayload, Order, RemoteUser } from '../types'

const BASE_URL = import.meta.env.VITE_ORDER_SERVICE_URL ?? 'http://localhost:5017'

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { 'Content-Type': 'application/json' },
    ...init,
  })

  if (!response.ok && response.status !== 422) {
    throw new Error(`Falha na requisição ${path}: ${response.status}`)
  }

  return response.json() as Promise<T>
}

export function fetchUsers(): Promise<RemoteUser[]> {
  return request<RemoteUser[]>('/api/users')
}

export function fetchOrders(): Promise<Order[]> {
  return request<Order[]>('/api/orders')
}

export function createOrder(payload: CreateOrderPayload): Promise<Order> {
  return request<Order>('/api/orders', {
    method: 'POST',
    body: JSON.stringify(payload),
  })
}
