import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import { useAuthStore } from '@/stores/auth_store'
import type { components } from '@/types/api-schema'

type LoginResponse = components['schemas']['LoginResponse']

export function SwitchAccountService() {
  const auth = useAuthStore()

  const loading = ref(false)
  const error = ref('')

  async function switchAccount(accountNumber: string, password: string): Promise<boolean> {
    if (!password) {
      error.value = 'Informe a senha dessa conta.'
      return false
    }

    try {
      loading.value = true
      error.value = ''

      const { data } = await apiService.post<LoginResponse>('auth/login', {
        accountNumber,
        password,
      })

      auth.login(data.token, accountNumber)
      return true
    } catch {
      error.value = 'Senha incorreta.'
      return false
    } finally {
      loading.value = false
    }
  }

  return { loading, error, switchAccount }
}
