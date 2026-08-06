<script setup lang="ts">
import { ref } from 'vue'
import type { CreateOrderPayload, RemoteUser } from '../types'

const props = defineProps<{ users: RemoteUser[]; submitting: boolean }>()
const emit = defineEmits<{ submit: [payload: CreateOrderPayload] }>()

const userId = ref(props.users[0]?.id ?? '')
const product = ref('')
const quantity = ref(1)

function handleSubmit() {
  if (!userId.value || !product.value.trim() || quantity.value < 1) return

  emit('submit', {
    userId: userId.value,
    product: product.value.trim(),
    quantity: quantity.value,
  })

  product.value = ''
  quantity.value = 1
}
</script>

<template>
  <section class="panel">
    <h2>Criar pedido (order-service)</h2>
    <form class="order-form" @submit.prevent="handleSubmit">
      <label>
        Usuário
        <select v-model="userId">
          <option v-for="user in props.users" :key="user.id" :value="user.id">
            {{ user.name }} ({{ user.active ? 'ativo' : 'inativo' }})
          </option>
        </select>
      </label>

      <label>
        Produto
        <input v-model="product" type="text" placeholder="Teclado mecânico" required />
      </label>

      <label>
        Quantidade
        <input v-model.number="quantity" type="number" min="1" required />
      </label>

      <button type="submit" :disabled="props.submitting">
        {{ props.submitting ? 'Enviando...' : 'Criar pedido' }}
      </button>
    </form>
  </section>
</template>

<style scoped>
.panel {
  background: var(--panel-bg);
  border-radius: 12px;
  padding: 1.5rem;
}

.order-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

label {
  display: flex;
  flex-direction: column;
  gap: 0.35rem;
  font-size: 0.85rem;
  color: var(--muted);
}

select,
input {
  padding: 0.5rem 0.6rem;
  border-radius: 8px;
  border: 1px solid var(--border);
  background: var(--input-bg);
  color: inherit;
  font-size: 0.95rem;
}

button {
  padding: 0.6rem;
  border-radius: 8px;
  border: none;
  background: var(--accent);
  color: white;
  font-weight: 600;
  cursor: pointer;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
