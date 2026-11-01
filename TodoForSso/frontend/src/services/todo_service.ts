import { todoApiService } from './api_service'

export type TodoPriority = 'Low' | 'Medium' | 'High'
export type TodoStatus = 'Pending' | 'InProgress' | 'Done'

export interface Todo {
  id: string
  title: string
  description?: string | null
  priority: TodoPriority
  status: TodoStatus
  createdAt: string
  dueDate?: string | null
  completedAt?: string | null
}

export interface CreateTodoPayload {
  title: string
  description?: string
  priority: TodoPriority
  dueDate?: string | null
}

export interface UpdateTodoPayload extends CreateTodoPayload {
  status: TodoStatus
}

export const todoService = {
  getAll: () => todoApiService.get<Todo[]>('/api/todos'),
  getById: (id: string) => todoApiService.get<Todo>(`/api/todos/${id}`),
  create: (payload: CreateTodoPayload) => todoApiService.post<Todo>('/api/todos', payload),
  update: (id: string, payload: UpdateTodoPayload) => todoApiService.put<Todo>(`/api/todos/${id}`, payload),
  remove: (id: string) => todoApiService.delete<void>(`/api/todos/${id}`),
  complete: (id: string) => todoApiService.patch<Todo>(`/api/todos/${id}/complete`),
  reopen: (id: string) => todoApiService.patch<Todo>(`/api/todos/${id}/reopen`),
}
