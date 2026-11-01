<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, RouterView, useRoute } from 'vue-router'
import Avatar from 'primevue/avatar'
import Button from 'primevue/button'
import { useAuthStore } from '@/stores/auth_store'

const auth = useAuthStore()
const route = useRoute()

const navItems = computed(() => {
  const items = [
    { name: 'dashboard', label: 'Dashboard', icon: 'pi pi-th-large' },
    { name: 'todos', label: 'Minhas Tarefas', icon: 'pi pi-check-square' },
  ]

  if (auth.isAdmin) {
    items.push({ name: 'users', label: 'Usuários', icon: 'pi pi-users' })
  }

  return items
})

const initials = computed(() =>
  auth.username
    .split(/[\s._-]+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase())
    .join(''),
)

function handleLogout() {
  auth.logout()
}
</script>

<template>
  <div class="flex h-screen w-screen overflow-hidden bg-surface-50 dark:bg-surface-950">
    <aside class="flex w-64 shrink-0 flex-col bg-slate-900 text-slate-200">
      <div class="flex items-center gap-2 px-6 py-5 text-lg font-semibold text-white">
        <i class="pi pi-sparkles text-emerald-400"></i>
        <span>TodoForSso</span>
      </div>

      <nav class="mt-4 flex flex-1 flex-col gap-1 px-3">
        <RouterLink
          v-for="item in navItems"
          :key="item.name"
          :to="{ name: item.name }"
          class="flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors"
          :class="
            route.name === item.name
              ? 'bg-emerald-500/15 text-emerald-400'
              : 'text-slate-300 hover:bg-slate-800 hover:text-white'
          "
        >
          <i :class="item.icon"></i>
          <span>{{ item.label }}</span>
        </RouterLink>
      </nav>

      <div class="border-t border-slate-800 px-3 py-4">
        <Button
          label="Sair"
          icon="pi pi-sign-out"
          severity="secondary"
          text
          class="w-full !justify-start !text-slate-300 hover:!text-white"
          @click="handleLogout"
        />
      </div>
    </aside>

    <div class="flex min-w-0 flex-1 flex-col">
      <header
        class="flex h-16 shrink-0 items-center justify-between border-b border-surface-200 bg-white px-6 dark:border-surface-800 dark:bg-surface-900"
      >
        <h1 class="text-lg font-semibold text-surface-900 dark:text-surface-0">
          {{ navItems.find((item) => item.name === route.name)?.label ?? 'TodoForSso' }}
        </h1>

        <div class="flex items-center gap-3">
          <span class="text-sm text-surface-600 dark:text-surface-300">{{ auth.username }}</span>
          <Avatar :label="initials || '?'" shape="circle" class="bg-emerald-500 text-white" />
        </div>
      </header>

      <main class="flex-1 overflow-y-auto p-6">
        <RouterView />
      </main>
    </div>
  </div>
</template>
