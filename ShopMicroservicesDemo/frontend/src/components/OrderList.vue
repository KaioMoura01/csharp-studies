<script setup lang="ts">
import type { Order } from '../types'

defineProps<{ orders: Order[] }>()
</script>

<template>
  <section class="panel">
    <h2>Pedidos</h2>
    <p v-if="orders.length === 0" class="empty">Nenhum pedido ainda.</p>
    <ul class="order-list">
      <li v-for="order in orders" :key="order.id" :class="order.status.toLowerCase()">
        <div class="row">
          <span class="product">{{ order.product }} x{{ order.quantity }}</span>
          <span class="status">{{ order.status === 'Created' ? 'criado' : 'rejeitado' }}</span>
        </div>
        <p class="message">{{ order.message }}</p>
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

.empty {
  color: var(--muted);
}

.order-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.order-list li {
  padding: 0.7rem 0.9rem;
  border-radius: 8px;
  background: var(--row-bg);
  border-left: 4px solid var(--muted);
}

.order-list li.created {
  border-left-color: #2ecc71;
}

.order-list li.rejected {
  border-left-color: #e74c3c;
}

.row {
  display: flex;
  justify-content: space-between;
  font-weight: 600;
}

.status {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: var(--muted);
}

.message {
  margin: 0.3rem 0 0;
  font-size: 0.85rem;
  color: var(--muted);
}
</style>
