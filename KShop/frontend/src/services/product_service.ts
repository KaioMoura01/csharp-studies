import { onMounted, ref } from 'vue'
import { productApiService } from '@/services/api_service'

export interface ProductResponse {
  id: string
  name: string
  description: string | null
  imageUrl: string | null
  price: number
  stock: number
  categoryId: string
  categoryName: string | null
}

export interface CategoryResponse {
  id: string
  name: string | null
  description: string | null
}

export function ProductService() {
  const products = ref<ProductResponse[]>([])
  const categories = ref<CategoryResponse[]>([])
  const loading = ref(false)
  const error = ref('')

  async function load() {
    error.value = ''
    try {
      loading.value = true
      const [productsResponse, categoriesResponse] = await Promise.all([
        productApiService.get<ProductResponse[]>('api/products'),
        productApiService.get<CategoryResponse[]>('api/categories'),
      ])
      products.value = productsResponse.data
      categories.value = categoriesResponse.data
    } catch {
      error.value = 'Não foi possível carregar os produtos.'
    } finally {
      loading.value = false
    }
  }

  onMounted(load)

  return { products, categories, loading, error, load }
}
