<script setup lang="ts">
import { RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth_store'
import Button from 'primevue/button'

const router = useRouter()
const auth = useAuthStore()

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}

function home(){
  router.push({ name: 'dashboard' })
}
</script>

<template>
  <div class="min-h-screen bg-surface-50 dark:bg-surface-950">
    <div class="flex items-center justify-between w-full p-2 gap-4 bg-slate-900">
      <Button label="BankLedger" link class="font-bold text-2xl text-primary no-underline!" @click="home" />
      <Button label="Sair" icon="pi pi-sign-out" severity="secondary" text @click="logout" />
    </div>

    <main class="mx-auto max-w-4xl p-6">
      <RouterView />
    </main>
  </div>
</template>
