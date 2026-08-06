import axios, { type AxiosInstance, type AxiosRequestConfig, type AxiosResponse } from 'axios'

class ApiService {
  private readonly client: AxiosInstance

  constructor(baseURL: string) {
    this.client = axios.create({ baseURL })

    this.client.interceptors.request.use((config) => {
      const token = localStorage.getItem('token')
      if (token) {
        config.headers.Authorization = `Bearer ${token}`
      }
      return config
    })
  }

  get<T>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.client.get<T>(url, config)
  }

  post<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.client.post<T>(url, data, config)
  }

  put<T>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.client.put<T>(url, data, config)
  }

  delete<T>(url: string, config?: AxiosRequestConfig): Promise<AxiosResponse<T>> {
    return this.client.delete<T>(url, config)
  }
}

export const productApiService = new ApiService(import.meta.env.VITE_PRODUCT_API_URL)
export const userApiService = new ApiService(import.meta.env.VITE_USER_API_URL)
