<script setup lang="ts">
import { RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth_store'
import Button from 'primevue/button'

const router = useRouter()
const auth = useAuthStore()

function logout() {
  auth.logout()
  router.push({ name: 'login' })
}

function home() {
  router.push({ name: 'products' })
}
</script>

<template>
  <div class="min-h-screen bg-surface-50 dark:bg-surface-950">
    <div class="flex items-center justify-between w-full p-2 gap-4 bg-surface-0 dark:bg-surface-900">
      <Button label="KShop" link unstyled class="font-bold text-2xl text-primary cursor-pointer" @click="home" />
      <div class="flex items-center gap-3">
        <span v-if="auth.displayName" class="text-sm text-surface-500">{{ auth.displayName }}</span>
        <Button label="Sair" icon="pi pi-sign-out" severity="secondary" text @click="logout" />
      </div>
    </div>

    <main class="mx-auto max-w-5xl p-6">
      <RouterView />
    </main>
  </div>
</template>
