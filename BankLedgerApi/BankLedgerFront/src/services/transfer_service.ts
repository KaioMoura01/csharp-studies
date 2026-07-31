import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'
import { useBalanceStore } from '@/stores/balance_store'

type CreateTransferRequest = components['schemas']['CreateTransferRequest']
type TransferResponse = components['schemas']['TransferResponse']

export function TransferService() {
  const balanceStore = useBalanceStore()
  const destinationAccountNumber = ref('')
  const amount = ref<number | null>(null)
  const password = ref('')
  const loading = ref(false)
  const error = ref('')
  const success = ref<TransferResponse | null>(null)

  async function submit() {
    error.value = ''
    success.value = null

    if (!destinationAccountNumber.value || !amount.value) {
      error.value = 'Informe a conta de destino e o valor.'
      return
    }

    if (amount.value <= 0) {
      error.value = 'O valor deve ser maior que zero.'
      return
    }

    if (!password.value) {
      error.value = 'Informe sua senha.'
      return
    }

    try {
      loading.value = true

      const { data } = await apiService.post<TransferResponse>('transfers', {
        destinationAccountNumber: destinationAccountNumber.value,
        amount: amount.value,
        password: password.value,
      } satisfies CreateTransferRequest)

      success.value = data
      await balanceStore.refresh()
      destinationAccountNumber.value = ''
      amount.value = null
      password.value = ''
    } catch {
      error.value = 'Não foi possível concluir a transferência. Verifique os dados e a senha e tente novamente.'
    } finally {
      loading.value = false
    }
  }

  return { destinationAccountNumber, amount, password, loading, error, success, submit }
}
