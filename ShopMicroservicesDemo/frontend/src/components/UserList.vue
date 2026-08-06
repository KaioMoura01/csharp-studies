<script setup lang="ts">
import type { RemoteUser } from '../types'

defineProps<{ users: RemoteUser[] }>()
</script>

<template>
  <section class="panel">
    <h2>Usuários (user-service)</h2>
    <ul class="user-list">
      <li v-for="user in users" :key="user.id" :class="{ inactive: !user.active }">
        <span class="name">{{ user.name }}</span>
        <span class="email">{{ user.email }}</span>
        <span class="badge">{{ user.active ? 'ativo' : 'inativo' }}</span>
      </li>
    </ul>
  </section>
</template>

<style scoped>
.panel {
  background: var(--panel-bg);
  border-radius: 12px;
  padding: 1.5rem;
}

.user-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.user-list li {
  display: flex;
  flex-direction: column;
  align-items: start;
  gap: 0.75rem;
  padding: 0.6rem 0.8rem;
  border-radius: 8px;
  background: var(--row-bg);
}

.user-list li.inactive {
  opacity: 0.55;
}

.name {
  font-weight: 600;
}

.email {
  color: var(--muted);
  font-size: 0.85rem;
  flex: 1;
}

.badge {
  font-size: 0.75rem;
  padding: 0.15rem 0.5rem;
  border-radius: 999px;
  background: var(--badge-bg);
}
</style>
