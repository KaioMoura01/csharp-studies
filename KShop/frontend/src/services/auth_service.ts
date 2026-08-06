import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login as keycloakLogin } from '@/services/keycloak_service'
import { useAuthStore } from '@/stores/auth_store'

export function AuthService() {
  const router = useRouter()
  const auth = useAuthStore()

  const username = ref('')
  const password = ref('')
  const loading = ref(false)
  const error = ref('')

  async function submit() {
    error.value = ''

    if (!username.value || !password.value) {
      error.value = 'Informe usuário e senha.'
      return
    }

    try {
      loading.value = true
      const { access_token, refresh_token } = await keycloakLogin(username.value, password.value)
      auth.login(access_token, refresh_token)
      router.push({ name: 'products' })
    } catch {
      error.value = 'Usuário ou senha inválidos.'
    } finally {
      loading.value = false
    }
  }

  return { username, password, loading, error, submit }
}
