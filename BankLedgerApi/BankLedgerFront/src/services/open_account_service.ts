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

  const loading = ref(false)
  const error = ref('')

  async function submit(customerId: string): Promise<boolean> {
    error.value = ''

    if (!accountName.value) {
      error.value = 'Preencha o nome da conta.'
      return false
    }

    try {
      loading.value = true

      const { data: account } = await apiService.post<AccountCreatedResponse>('accounts', {
        customerId,
        name: accountName.value,
        type: accountType.value,
      } satisfies CreateAccountRequest)

      const { data: switched } = await apiService.post<LoginResponse>(
        `auth/switch-account/${account.id}`,
      )

      auth.login(switched.token, switched.customerId, switched.activeAccountId, account.number)

      accountName.value = ''
      return true
    } catch {
      error.value = 'Não foi possível abrir a conta. Tente novamente.'
      return false
    } finally {
      loading.value = false
    }
  }

  return { accountName, accountType, loading, error, submit }
}
