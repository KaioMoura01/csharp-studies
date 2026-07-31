import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'
import { useBalanceStore } from '@/stores/balance_store'

type DepositRequest = components['schemas']['DepositRequest']
type AccountDetailsResponse = components['schemas']['AccountDetailsResponse']

export function DepositService() {
  const balanceStore = useBalanceStore()
  const amount = ref<number | null>(null)
  const loading = ref(false)
  const error = ref('')

  async function submit(): Promise<boolean> {
    error.value = ''

    if (!amount.value || amount.value <= 0) {
      error.value = 'Informe um valor maior que zero.'
      return false
    }

    try {
      loading.value = true

      const { data } = await apiService.post<AccountDetailsResponse>('accounts/deposit', {
        amount: amount.value,
      } satisfies DepositRequest)

      balanceStore.setAvailable(data.currentBalance)
      amount.value = null
      return true
    } catch {
      error.value = 'Não foi possível concluir o depósito. Tente novamente.'
      return false
    } finally {
      loading.value = false
    }
  }

  return { amount, loading, error, submit }
}
