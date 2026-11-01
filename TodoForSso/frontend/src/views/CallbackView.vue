<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { RouterLink, useRoute, useRouter } from 'vue-router'
import Message from 'primevue/message'
import ProgressSpinner from 'primevue/progressspinner'
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
    await auth.completeSsoLogin(code, state)
    router.replace({ name: 'dashboard' })
  } catch {
    error.value = 'Não foi possível concluir o login.'
  }
})
</script>

<template>
  <div class="flex min-h-screen w-screen flex-col items-center justify-center gap-4 bg-gradient-to-br from-slate-900 via-slate-900 to-emerald-950 p-4">
    <template v-if="error">
      <Message severity="error" :closable="false">{{ error }}</Message>
      <RouterLink to="/login" class="text-sm font-medium text-emerald-400 hover:underline">Voltar ao login</RouterLink>
    </template>
    <template v-else>
      <ProgressSpinner :strokeWidth="4" />
      <span class="text-sm text-surface-300">Finalizando login...</span>
    </template>
  </div>
</template>
