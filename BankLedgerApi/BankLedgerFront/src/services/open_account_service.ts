import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import { useAuthStore } from '@/stores/auth_store'
import type { components } from '@/types/api-schema'

type CreateAccountRequest = components['schemas']['CreateAccountRequest']
type AccountCreatedResponse = components['schemas']['AccountCreatedResponse']
type LoginResponse = components['schemas']['LoginResponse']
export type AccountType = components['schemas']['AccountTypeEnum']

export function OpenAccountService() {
  const auth = useAuthStore()

  const accountName = ref('')
  const accountType = ref<AccountType>('Checking')
  const password = ref('')

  const loading = ref(false)
  const error = ref('')

  async function submit(customerId: string): Promise<boolean> {
    error.value = ''

    if (!accountName.value || !password.value) {
      error.value = 'Preencha o nome da conta e a senha.'
      return false
    }

    try {
      loading.value = true

      const { data: account } = await apiService.post<AccountCreatedResponse>('accounts', {
        customerId,
        name: accountName.value,
        type: accountType.value,
        password: password.value,
      } satisfies CreateAccountRequest)

      const { data: login } = await apiService.post<LoginResponse>('auth/login', {
        accountNumber: account.number,
        password: password.value,
      })

      auth.login(login.token, account.number)

      accountName.value = ''
      password.value = ''
      return true
    } catch {
      error.value = 'Não foi possível abrir a conta. Tente novamente.'
      return false
    } finally {
      loading.value = false
    }
  }

  return { accountName, accountType, password, loading, error, submit }
}
