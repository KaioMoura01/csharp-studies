<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import { redirectToIdpLogin } from '@/services/keycloak_service'

const error = ref('')

// A tela de login é a do Keycloak (tema "kshop"). Aqui só disparamos o redirect —
// não há mais form local. Se já existir sessão SSO, o Keycloak devolve o code na hora e o
// usuário nem chega a ver uma tela; se não existir, ele mostra o login.
onMounted(async () => {
  try {
    await redirectToIdpLogin()
  } catch {
    error.value = 'Não foi possível contatar o servidor de autenticação.'
  }
})
</script>

<template>
  <div class="flex min-h-screen flex-col items-center justify-center gap-4 bg-surface-50 p-6 dark:bg-surface-950">
    <template v-if="error">
      <Message severity="error" variant="simple">{{ error }}</Message>
    </template>
    <template v-else>
      <ProgressSpinner :strokeWidth="4" />
      <span class="text-sm text-surface-500">Redirecionando para o login...</span>
    </template>
  </div>
</template>
