import { todoApiService } from './api_service'

export interface AppUser {
  id: string
  username: string
  email: string
  firstName?: string | null
  lastName?: string | null
  enabled: boolean
  roles: string[]
}

export interface CreateUserPayload {
  username: string
  email: string
  password: string
  firstName?: string
  lastName?: string
}

export const userService = {
  /** Criação pela tela administrativa (o auto-cadastro é feito na tela do Keycloak). */
  register: (payload: CreateUserPayload) => todoApiService.post<AppUser>('/api/users/register', payload),
  getAll: () => todoApiService.get<AppUser[]>('/api/users'),
  setEnabled: (id: string, enabled: boolean) =>
    todoApiService.patch<void>(`/api/users/${id}/status`, { enabled }),
}
