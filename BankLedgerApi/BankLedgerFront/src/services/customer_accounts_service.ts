import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'

export type AccountSummary = components['schemas']['AccountSummaryResponse']

export function CustomerAccountsService() {
  const accounts = ref<AccountSummary[]>([])
  const loading = ref(false)
  const error = ref('')

  async function fetchAccounts(customerId: string) {
    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.get<AccountSummary[]>(`customers/${customerId}/accounts`)
      accounts.value = data
    } catch {
      error.value = 'Não foi possível carregar suas contas.'
    } finally {
      loading.value = false
    }
  }

  return { accounts, loading, error, fetchAccounts }
}
