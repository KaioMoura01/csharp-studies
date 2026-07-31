import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import { FormaterServices } from '@/services/formater_service'
import type { components } from '@/types/api-schema'

type StatementResponse = components['schemas']['StatementResponse']
export type StatementEntry = components['schemas']['StatementEntryResponse']

export function StatementService() {
  const entries = ref<StatementEntry[]>([])
  const loading = ref(false)
  const error = ref('')

  async function fetchStatement(from: Date, to: Date) {
    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.get<StatementResponse>('statements', {
        params: {
          InitDate: FormaterServices.FormatDateOnly(from),
          EndDate: FormaterServices.FormatDateOnly(to),
        },
      })

      entries.value = data.entries
    } catch {
      error.value = 'Erro ao buscar as transações, por favor tente mais tarde'
    } finally {
      loading.value = false
    }
  }

  return { entries, loading, error, fetchStatement }
}
