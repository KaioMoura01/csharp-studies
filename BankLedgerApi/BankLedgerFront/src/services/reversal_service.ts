import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'
import { useBalanceStore } from '@/stores/balance_store'

type ReversalRequest = components['schemas']['ReversalRequest']
type ReversalResponse = components['schemas']['ReversalResponse']

export function ReversalService() {
  const balanceStore = useBalanceStore()
  const loading = ref(false)
  const error = ref('')

  async function reverse(transferId: string, password: string): Promise<ReversalResponse | null> {
    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.post<ReversalResponse>('reversals', {
        transferId,
        password,
      } satisfies ReversalRequest)
      await balanceStore.refresh()
      return data
    } catch {
      error.value = 'Não foi possível solicitar o estorno dessa transação. Confira a senha e tente novamente.'
      return null
    } finally {
      loading.value = false
    }
  }

  return { loading, error, reverse }
}
