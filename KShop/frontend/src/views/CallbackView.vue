<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import { exchangeCodeForToken } from '@/services/keycloak_service'
import { userApiService } from '@/services/api_service'
import { useAuthStore } from '@/stores/auth_store'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const error = ref('')

onMounted(async () => {
  const code = route.query.code as string | undefined
  const state = route.query.state as string | undefined
  const oauthError = route.query.error as string | undefined

  if (oauthError) {
    error.value = 'Login cancelado ou negado.'
    return
  }

  if (!code || !state) {
    error.value = 'Retorno de login inválido.'
    return
  }

  try {
    const { access_token, refresh_token, id_token } = await exchangeCodeForToken(code, state)
    auth.login(access_token, refresh_token, id_token)

    // Garante o perfil local antes de liberar o app: o cadastro acontece no Keycloak, que não
    // conhece a tabela UserProfile. A UserApi cria o registro na primeira chamada a /me — sem
    // isso, criar produto falharia com UnrecognizedUserException.
    await userApiService.get('api/userprofiles/me')

    router.replace({ name: 'products' })
  } catch {
    error.value = 'Não foi possível concluir o login.'
  }
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center gap-4 bg-surface-50 p-6 dark:bg-surface-950">
    <template v-if="error">
      <Message severity="error" variant="simple">{{ error }}</Message>
      <RouterLink to="/login" class="text-sm font-medium text-primary hover:underline">Voltar ao login</RouterLink>
    </template>
    <template v-else>
      <ProgressSpinner :strokeWidth="4" />
      <span class="text-sm text-surface-500">Finalizando login...</span>
    </template>
  </div>
</template>
