import { computed, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { apiService } from '@/services/api_service'
import { useAuthStore } from '@/stores/auth_store'
import type { components } from '@/types/api-schema'

type CreateCustomerRequest = components['schemas']['CreateCustomerRequest']
type CustomerDetailsResponse = components['schemas']['CustomerDetailsResponse']
type CreateAccountRequest = components['schemas']['CreateAccountRequest']
type AccountCreatedResponse = components['schemas']['AccountCreatedResponse']
type LoginResponse = components['schemas']['LoginResponse']
export type DocumentType = components['schemas']['DocumentTypeEnum']
export type AccountType = components['schemas']['AccountTypeEnum']

export function RegisterService() {
  const router = useRouter()
  const auth = useAuthStore()

  const name = ref('')
  const documentType = ref<DocumentType>('Cpf')
  const documentNumber = ref('')
  const accountName = ref('')
  const accountType = ref<AccountType>('Checking')
  const password = ref('')
  const confirmPassword = ref('')

  const loading = ref(false)
  const error = ref('')

  const documentMask = computed(() =>
    documentType.value === 'Cpf' ? '999.999.999-99' : '99.999.999/9999-99',
  )

  watch(documentType, () => {
    documentNumber.value = ''
  })

  async function submit() {
    error.value = ''

    if (
      !name.value ||
      !documentNumber.value ||
      !accountName.value ||
      !password.value ||
      !confirmPassword.value
    ) {
      error.value = 'Preencha todos os campos.'
      return
    }

    if (password.value !== confirmPassword.value) {
      error.value = 'As senhas não coincidem.'
      return
    }

    try {
      loading.value = true

      const { data: customer } = await apiService.post<CustomerDetailsResponse>('customers', {
        name: name.value,
        documentNumber: documentNumber.value,
        documentType: documentType.value,
        password: password.value,
      } satisfies CreateCustomerRequest)

      await apiService.post<AccountCreatedResponse>('accounts', {
        customerId: customer.id,
        name: accountName.value,
        type: accountType.value,
      } satisfies CreateAccountRequest)

      const { data: login } = await apiService.post<LoginResponse>('auth/login', {
        documentNumber: documentNumber.value,
        password: password.value,
      })

      const activeAccount = login.accounts.find((a) => a.id === login.activeAccountId)
      auth.login(login.token, login.customerId, login.activeAccountId, activeAccount?.number ?? '')
      router.push({ name: 'dashboard' })
    } catch {
      error.value = 'Não foi possível concluir o cadastro. Confira os dados e tente novamente.'
    } finally {
      loading.value = false
    }
  }

  return {
    name,
    documentType,
    documentNumber,
    documentMask,
    accountName,
    accountType,
    password,
    confirmPassword,
    loading,
    error,
    submit,
  }
}
