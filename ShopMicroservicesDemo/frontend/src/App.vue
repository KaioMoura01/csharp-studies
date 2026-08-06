<script setup lang="ts">
import { onMounted, ref } from 'vue'
import UserList from './components/UserList.vue'
import OrderForm from './components/OrderForm.vue'
import OrderList from './components/OrderList.vue'
import { createOrder, fetchOrders, fetchUsers } from './api/client'
import type { CreateOrderPayload, Order, RemoteUser } from './types'

const users = ref<RemoteUser[]>([])
const orders = ref<Order[]>([])
const loading = ref(true)
const submitting = ref(false)
const errorMessage = ref('')

async function loadData() {
  loading.value = true
  errorMessage.value = ''
  try {
    const [usersResult, ordersResult] = await Promise.all([fetchUsers(), fetchOrders()])
    users.value = usersResult
    orders.value = ordersResult
  } catch {
    errorMessage.value = 'Não foi possível falar com o order-service. Ele está rodando em http://localhost:5017?'
  } finally {
    loading.value = false
  }
}

async function handleCreateOrder(payload: CreateOrderPayload) {
  submitting.value = true
  try {
    await createOrder(payload)
    await loadData()
  } finally {
    submitting.value = false
  }
}

onMounted(loadData)
</script>

<template>
  <main class="app">
    <header>
      <h1>Shop Microservices Demo</h1>
      <p class="subtitle">Vue 3 &rarr; OrderService (REST) &rarr; UserService (gRPC)</p>
    </header>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>

    <div v-else-if="loading" class="loading">Carregando...</div>

    <div v-else class="grid">
      <UserList :users="users" />
      <OrderForm :users="users" :submitting="submitting" @submit="handleCreateOrder" />
      <OrderList :orders="orders" />
    </div>
  </main>
</template>

<style scoped>
.app {
  max-width: 1000px;
  margin: 0 auto;
  padding: 2rem 1.5rem 4rem;
}

header {
  margin-bottom: 2rem;
}

h1 {
  margin: 0 0 0.35rem;
  font-size: 1.8rem;
}

.subtitle {
  margin: 0;
  color: var(--muted);
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  align-items: start;
}

.error {
  color: #e74c3c;
  font-weight: 600;
}

.loading {
  color: var(--muted);
}
</style>
