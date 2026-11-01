<script setup lang="ts">
import { onMounted, ref } from 'vue'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
import { redirectToLogin } from '@/services/keycloak_service'

const error = ref('')

// A tela de login é a do Keycloak (tema "todoforsso"). Aqui só disparamos o redirect —
// não há mais form local. Se já existir sessão SSO, o Keycloak devolve o code na hora e o
// usuário nem chega a ver uma tela; se não existir, ele mostra o login.
onMounted(async () => {
  try {
    await redirectToLogin()
  } catch {
    error.value = 'Não foi possível contatar o servidor de autenticação.'
  }
})
</script>

<template>
  <div
    class="flex min-h-screen w-screen flex-col items-center justify-center gap-4 bg-gradient-to-br from-slate-900 via-slate-900 to-emerald-950 p-4"
  >
    <template v-if="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
    </template>
    <template v-else>
      <ProgressSpinner :strokeWidth="4" />
      <span class="text-sm text-surface-300">Redirecionando para o login...</span>
    </template>
  </div>
</template>
