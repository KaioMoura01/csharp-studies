<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { RouterLink } from 'vue-router'
import Card from 'primevue/card'
import Tag from 'primevue/tag'
import ProgressSpinner from 'primevue/progressspinner'
import { todoService, type Todo } from '@/services/todo_service'
import { useAuthStore } from '@/stores/auth_store'

const auth = useAuthStore()
const todos = ref<Todo[]>([])
const loading = ref(true)

const total = computed(() => todos.value.length)
const done = computed(() => todos.value.filter((t) => t.status === 'Done').length)
const pending = computed(() => todos.value.filter((t) => t.status !== 'Done').length)
const recent = computed(() => todos.value.slice(0, 5))

const priorityColor: Record<Todo['priority'], 'success' | 'warn' | 'danger'> = {
  Low: 'success',
  Medium: 'warn',
  High: 'danger',
}

onMounted(async () => {
  try {
    const { data } = await todoService.getAll()
    todos.value = data
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div class="flex flex-col gap-6">
    <div>
      <h2 class="text-2xl font-semibold text-surface-900 dark:text-surface-0">Olá, {{ auth.username }} 👋</h2>
      <p class="text-surface-500">Aqui está um resumo das suas tarefas.</p>
    </div>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-3">
      <Card>
        <template #content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-surface-500">Total</p>
              <p class="text-3xl font-bold">{{ total }}</p>
            </div>
            <i class="pi pi-list text-3xl text-slate-400"></i>
          </div>
        </template>
      </Card>
      <Card>
        <template #content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-surface-500">Pendentes</p>
              <p class="text-3xl font-bold text-amber-500">{{ pending }}</p>
            </div>
            <i class="pi pi-clock text-3xl text-amber-400"></i>
          </div>
        </template>
      </Card>
      <Card>
        <template #content>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-surface-500">Concluídas</p>
              <p class="text-3xl font-bold text-emerald-500">{{ done }}</p>
            </div>
            <i class="pi pi-check-circle text-3xl text-emerald-400"></i>
          </div>
        </template>
      </Card>
    </div>

    <Card>
      <template #title>Tarefas recentes</template>
      <template #content>
        <div v-if="loading" class="flex justify-center py-8">
          <ProgressSpinner style="width: 2.5rem; height: 2.5rem" />
        </div>
        <p v-else-if="recent.length === 0" class="py-6 text-center text-surface-500">
          Nenhuma tarefa ainda.
          <RouterLink to="/todos" class="text-emerald-600 hover:underline">Crie a primeira</RouterLink>.
        </p>
        <ul v-else class="flex flex-col divide-y divide-surface-200 dark:divide-surface-800">
          <li v-for="todo in recent" :key="todo.id" class="flex items-center justify-between py-3">
            <div>
              <p
                class="font-medium"
                :class="{ 'line-through text-surface-400': todo.status === 'Done' }"
              >
                {{ todo.title }}
              </p>
              <p v-if="todo.dueDate" class="text-xs text-surface-500">
                Vence em {{ new Date(todo.dueDate).toLocaleDateString('pt-BR') }}
              </p>
            </div>
            <Tag :value="todo.priority" :severity="priorityColor[todo.priority]" />
          </li>
        </ul>
      </template>
    </Card>
  </div>
</template>
