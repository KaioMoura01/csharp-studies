<script setup lang="ts">
import { onMounted, reactive, ref } from 'vue'
import Button from 'primevue/button'
import Card from 'primevue/card'
import Column from 'primevue/column'
import DataTable from 'primevue/datatable'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Tag from 'primevue/tag'
import ToggleSwitch from 'primevue/toggleswitch'
import Toast from 'primevue/toast'
import { useToast } from 'primevue/usetoast'
import { userService, type AppUser } from '@/services/user_service'

const toast = useToast()

const users = ref<AppUser[]>([])
const loading = ref(true)
const dialogVisible = ref(false)

const form = reactive({
  username: '',
  email: '',
  password: '',
  firstName: '',
  lastName: '',
})

async function loadUsers() {
  loading.value = true
  try {
    const { data } = await userService.getAll()
    users.value = data
  } finally {
    loading.value = false
  }
}

async function submitForm() {
  await userService.register(form)
  dialogVisible.value = false
  toast.add({ severity: 'success', summary: 'Usuário criado', detail: form.username, life: 2500 })
  Object.assign(form, { username: '', email: '', password: '', firstName: '', lastName: '' })
  await loadUsers()
}

async function toggleEnabled(user: AppUser, enabled: boolean) {
  await userService.setEnabled(user.id, enabled)
  toast.add({
    severity: 'success',
    summary: enabled ? 'Usuário ativado' : 'Usuário desativado',
    detail: user.username,
    life: 2500,
  })
  await loadUsers()
}

onMounted(loadUsers)
</script>

<template>
  <div class="flex flex-col gap-4">
    <Toast />

    <div class="flex items-center justify-between">
      <p class="text-surface-500">Gerencie os usuários cadastrados no Keycloak.</p>
      <Button label="Novo usuário" icon="pi pi-user-plus" @click="dialogVisible = true" />
    </div>

    <Card>
      <template #content>
        <DataTable :value="users" :loading="loading" paginator :rows="10" data-key="id">
          <Column field="username" header="Usuário" />
          <Column field="email" header="Email" />
          <Column header="Nome">
            <template #body="{ data }">{{ [data.firstName, data.lastName].filter(Boolean).join(' ') || '—' }}</template>
          </Column>
          <Column header="Roles">
            <template #body="{ data }">
              <div class="flex gap-1">
                <Tag v-for="role in data.roles" :key="role" :value="role" severity="info" />
              </div>
            </template>
          </Column>
          <Column header="Ativo" style="width: 6rem">
            <template #body="{ data }">
              <ToggleSwitch :model-value="data.enabled" @update:model-value="(v: boolean) => toggleEnabled(data, v)" />
            </template>
          </Column>
        </DataTable>
      </template>
    </Card>

    <Dialog v-model:visible="dialogVisible" modal header="Novo usuário" class="w-full max-w-md">
      <form class="flex flex-col gap-4" @submit.prevent="submitForm">
        <div class="flex gap-3">
          <div class="flex flex-1 flex-col gap-2">
            <label class="text-sm font-medium">Nome</label>
            <InputText v-model="form.firstName" />
          </div>
          <div class="flex flex-1 flex-col gap-2">
            <label class="text-sm font-medium">Sobrenome</label>
            <InputText v-model="form.lastName" />
          </div>
        </div>
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium">Usuário</label>
          <InputText v-model="form.username" required />
        </div>
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium">Email</label>
          <InputText v-model="form.email" type="email" required />
        </div>
        <div class="flex flex-col gap-2">
          <label class="text-sm font-medium">Senha</label>
          <Password v-model="form.password" toggleMask required fluid />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <Button label="Cancelar" text severity="secondary" @click="dialogVisible = false" />
          <Button type="submit" label="Criar" icon="pi pi-check" />
        </div>
      </form>
    </Dialog>
  </div>
</template>
