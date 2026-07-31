import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'

type ReversalResponse = components['schemas']['ReversalResponse']

export function ReversalService() {
  const loading = ref(false)
  const error = ref('')

  async function reverse(transferId: string): Promise<ReversalResponse | null> {
    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.post<ReversalResponse>('reversals', { transferId })
      return data
    } catch {
      error.value = 'Não foi possível solicitar o estorno dessa transação.'
      return null
    } finally {
      loading.value = false
    }
  }

  return { loading, error, reverse }
}
