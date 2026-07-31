import { useRouter } from 'vue-router'
import { computed, ref } from 'vue'
import { apiService } from '../services/api_service'
import { useAuthStore } from '../stores/auth_store'
import type { components } from '@/types/api-schema'

export function AuthService() {
  type LoginResponse = components['schemas']['LoginResponse']
  type DocumentType = components['schemas']['DocumentTypeEnum']

  const router = useRouter()
  const auth = useAuthStore()

  const documentType = ref<DocumentType>('Cpf')
  const documentNumber = ref('')
  const loading = ref(false)
  const password = ref('')
  const error = ref('')

  const documentMask = computed(() =>
    documentType.value === 'Cpf' ? '999.999.999-99' : '99.999.999/9999-99',
  )

  async function submit() {
    error.value = ''

    if (!documentNumber.value || !password.value) {
      error.value = 'Informe o documento e a senha.'
      return
    }

    try {
      loading.value = true
      const { data } = await apiService.post<LoginResponse>('auth/login', {
        documentNumber: documentNumber.value,
        password: password.value,
      })

      const activeAccount = data.accounts.find((a) => a.id === data.activeAccountId)
      auth.login(data.token, data.customerId, data.activeAccountId, activeAccount?.number ?? '')
      router.push({ name: 'dashboard' })
    } catch {
      error.value = 'Documento ou senha inválidos.'
    } finally {
      loading.value = false
    }
  }

  return { documentType, documentNumber, documentMask, password, error, loading, submit }
}
