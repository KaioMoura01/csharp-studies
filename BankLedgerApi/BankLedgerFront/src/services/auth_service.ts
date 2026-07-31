import {useRouter} from "vue-router";
import {ref} from "vue";
import { apiService } from '../services/api_service'
import { useAuthStore } from '../stores/auth_store'
import type {components} from "@/types/api-schema";

export function AuthService(){
  type LoginResponse = components['schemas']['LoginResponse']

  const router = useRouter()
  const auth = useAuthStore()

  const accountNumber = ref('')
  const loading = ref(false);
  const password = ref('')
  const error = ref('')

  async function submit() {
    error.value = ''

    if (!accountNumber.value || !password.value) {
      error.value = 'Informe o número da conta e a senha.'
      return
    }

    try {
      loading.value = true;
      const { data } = await apiService.post<LoginResponse>('auth/login', {
        accountNumber: accountNumber.value,
        password: password.value,
      })

      auth.login(data.token, accountNumber.value)
      router.push({ name: 'dashboard' })
    } catch {
      error.value = 'Número da conta ou senha inválidos.'
    } finally {
      loading.value = false
    }
  }

  return { accountNumber, password, error, loading, submit }
}
