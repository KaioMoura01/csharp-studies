<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Checkbox from 'primevue/checkbox'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Select from 'primevue/select'
import DatePicker from 'primevue/datepicker'
import Tag from 'primevue/tag'
import SelectButton from 'primevue/selectbutton'
import { useToast } from 'primevue/usetoast'
import { useConfirm } from 'primevue/useconfirm'
import ConfirmDialog from 'primevue/confirmdialog'
import Toast from 'primevue/toast'
import { todoService, type Todo, type TodoPriority, type TodoStatus } from '@/services/todo_service'

const toast = useToast()
const confirm = useConfirm()

const todos = ref<Todo[]>([])
const loading = ref(true)

const priorityOptions: TodoPriority[] = ['Low', 'Medium', 'High']
const statusFilterOptions: Array<{ label: string; value: TodoStatus | 'All' }> = [
  { label: 'Todas', value: 'All' },
  { label: 'Pendentes', value: 'Pending' },
  { label: 'Em andamento', value: 'InProgress' },
  { label: 'Concluídas', value: 'Done' },
]
const statusFilter = ref<TodoStatus | 'All'>('All')

const priorityColor: Record<TodoPriority, 'success' | 'warn' | 'danger'> = {
  Low: 'success',
  Medium: 'warn',
  High: 'danger',
}

const filteredTodos = computed(() =>
  statusFilter.value === 'All' ? todos.value : todos.value.filter((t) => t.status === statusFilter.value),
)

const dialogVisible = ref(false)
const editingId = ref<string | null>(null)
const form = reactive({
  title: '',
  description: '',
  priority: 'Medium' as TodoPriority,
  status: 'Pending' as TodoStatus,
  dueDate: null as Date | null,
})

function resetForm() {
  editingId.value = null
  form.title = ''
  form.description = ''
  form.priority = 'Medium'
  form.status = 'Pending'
  form.dueDate = null
}

function openCreate() {
  resetForm()
  dialogVisible.value = true
}

function openEdit(todo: Todo) {
  editingId.value = todo.id
  form.title = todo.title
  form.description = todo.description ?? ''
  form.priority = todo.priority
  form.status = todo.status
  form.dueDate = todo.dueDate ? new Date(todo.dueDate) : null
  dialogVisible.value = true
}

async function loadTodos() {
  loading.value = true
  try {
    const { data } = await todoService.getAll()
    todos.value = data
  } finally {
    loading.value = false
  }
}

async function submitForm() {
  const payload = {
    title: form.title,
    description: form.description || undefined,
    priority: form.priority,
    status: form.status,
    dueDate: form.dueDate ? form.dueDate.toISOString() : null,
  }

  if (editingId.value) {
    await todoService.update(editingId.value, payload)
  } else {
    await todoService.create(payload)
  }

  dialogVisible.value = false
  toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Tarefa salva.', life: 2500 })
  await loadTodos()
}

async function toggleComplete(todo: Todo) {
  if (todo.status === 'Done') {
    await todoService.reopen(todo.id)
  } else {
    await todoService.complete(todo.id)
  }
  await loadTodos()
}

function askDelete(todo: Todo) {
  confirm.require({
    message: `Excluir a tarefa "${todo.title}"?`,
    header: 'Confirmar exclusão',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Excluir',
    rejectLabel: 'Cancelar',
    acceptProps: { severity: 'danger' },
    accept: async () => {
      await todoService.remove(todo.id)
      toast.add({ severity: 'success', summary: 'Excluída', detail: 'Tarefa removida.', life: 2500 })
      await loadTodos()
    },
  })
}

onMounted(loadTodos)
</script>

<template>
  <div class="flex flex-col gap-4">
    <Toast />
    <ConfirmDialog />

    <div class="flex flex-wrap items-center justify-between gap-3">
      <SelectButton v-model="statusFilter" :options="statusFilterOptions" option-label="label" option-value="value" />
      <Button label="Nova tarefa" icon="pi pi-plus" @click="openCreate" />
    </div>

    <Card>
      <template #content>
        <div v-if="loading" class="py-8 text-center text-surface-500">Carregando...</div>
        <p v-else-if="filteredTodos.length === 0" class="py-8 text-center text-surface-500">
          Nenhuma tarefa encontrada.
        </p>
        <ul v-else class="flex flex-col divide-y divide-surface-200 dark:divide-surface-800">
          <li
            v-for="todo in filteredTodos"
            :key="todo.id"
            class="flex items-center gap-4 py-3"
          >
            <Checkbox :model-value="todo.status === 'Done'" binary @update:model-value="toggleComplete(todo)" />
            <div class="min-w-0 flex-1">
              <p class="truncate font-medium" :class="{ 'line-through text-surface-400': todo.status === 'Done' }">
                {{ todo.title }}
              </p>
              <p v-if="todo.description" class="truncate text-sm text-surface-500">{{ todo.description }}</p>
            </div>
            <Tag :value="todo.priority" :severity="priorityColor[todo.priority]" />
            <Button icon="pi pi-pencil" text rounded severity="secondary" @click="openEdit(todo)" />
            <Button icon="pi pi-trash" text rounded severity="danger" @click="askDelete(todo)" />
          </li>
        </ul>
      </template>
    </Card>

    <Dialog v-model:visible="dialogVisible" modal :header="editingId ? 'Editar tarefa' : 'Nova tarefa'" class="w-full max-w-md">
      <form class="flex flex-col gap-4" @submit.prevent="submitForm">
        <div class="flex flex-col gap-2">
          <label for="title" class="text-sm font-medium">Título</label>
          <InputText id="title" v-model="form.title" required autofocus />
        </div>
        <div class="flex flex-col gap-2">
          <label for="description" class="text-sm font-medium">Descrição</label>
          <Textarea id="description" v-model="form.description" rows="3" autoResize />
        </div>
        <div class="flex gap-3">
          <div class="flex flex-1 flex-col gap-2">
            <label class="text-sm font-medium">Prioridade</label>
            <Select v-model="form.priority" :options="priorityOptions" />
          </div>
          <div class="flex flex-1 flex-col gap-2">
            <label class="text-sm font-medium">Prazo</label>
            <DatePicker v-model="form.dueDate" showIcon dateFormat="dd/mm/yy" />
          </div>
        </div>
        <div v-if="editingId" class="flex flex-col gap-2">
          <label class="text-sm font-medium">Status</label>
          <Select v-model="form.status" :options="['Pending', 'InProgress', 'Done']" />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <Button label="Cancelar" text severity="secondary" @click="dialogVisible = false" />
          <Button type="submit" label="Salvar" icon="pi pi-check" />
        </div>
      </form>
    </Dialog>
  </div>
</template>
