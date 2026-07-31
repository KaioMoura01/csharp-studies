import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import { useAuthStore } from '@/stores/auth_store'
import type { components } from '@/types/api-schema'

type LoginResponse = components['schemas']['LoginResponse']

export function SwitchAccountService() {
  const auth = useAuthStore()

  const loading = ref(false)
  const error = ref('')

  async function switchAccount(accountId: string, accountNumber: string): Promise<boolean> {
    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.post<LoginResponse>(`auth/switch-account/${accountId}`)

      auth.login(data.token, data.customerId, data.activeAccountId, accountNumber)
      return true
    } catch {
      error.value = 'Não foi possível trocar de conta.'
      return false
    } finally {
      loading.value = false
    }
  }

  return { loading, error, switchAccount }
}
