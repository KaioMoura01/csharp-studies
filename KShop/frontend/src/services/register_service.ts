import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { userApiService } from '@/services/api_service'
import { login as keycloakLogin } from '@/services/keycloak_service'
import { useAuthStore } from '@/stores/auth_store'

interface RegisterRequest {
  username: string
  email: string
  firstName: string
  lastName: string
  password: string
}

export function RegisterService() {
  const router = useRouter()
  const auth = useAuthStore()

  const firstName = ref('')
  const lastName = ref('')
  const username = ref('')
  const email = ref('')
  const password = ref('')
  const confirmPassword = ref('')

  const loading = ref(false)
  const error = ref('')

  async function submit() {
    error.value = ''

    if (
      !firstName.value ||
      !lastName.value ||
      !username.value ||
      !email.value ||
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

      await userApiService.post('api/auth/register', {
        username: username.value,
        email: email.value,
        firstName: firstName.value,
        lastName: lastName.value,
        password: password.value,
      } satisfies RegisterRequest)

      const { access_token, refresh_token } = await keycloakLogin(username.value, password.value)
      auth.login(access_token, refresh_token)
      router.push({ name: 'products' })
    } catch {
      error.value = 'Não foi possível concluir o cadastro. Confira os dados e tente novamente.'
    } finally {
      loading.value = false
    }
  }

  return {
    firstName,
    lastName,
    username,
    email,
    password,
    confirmPassword,
    loading,
    error,
    submit,
  }
}
