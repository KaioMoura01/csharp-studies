import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'

type CreateTransferRequest = components['schemas']['CreateTransferRequest']
type TransferResponse = components['schemas']['TransferResponse']

export function TransferService() {
  const destinationAccountNumber = ref('')
  const amount = ref<number | null>(null)
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

    try {
      loading.value = true

      const { data } = await apiService.post<TransferResponse>('transfers', {
        destinationAccountNumber: destinationAccountNumber.value,
        amount: amount.value,
      } satisfies CreateTransferRequest)

      success.value = data
      destinationAccountNumber.value = ''
      amount.value = null
    } catch {
      error.value = 'Não foi possível concluir a transferência. Verifique os dados e tente novamente.'
    } finally {
      loading.value = false
    }
  }

  return { destinationAccountNumber, amount, loading, error, success, submit }
}
