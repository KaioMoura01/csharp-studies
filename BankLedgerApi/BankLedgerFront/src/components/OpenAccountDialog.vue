<script setup lang="ts">
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputPassword from 'primevue/inputpassword'
import Select from 'primevue/select'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { OpenAccountService } from '@/services/open_account_service'

const props = defineProps<{
  visible: boolean
  customerId: string
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  created: []
}>()

const { accountName, accountType, password, loading, error, submit } = OpenAccountService()

const accountTypeOptions = [
  { label: 'Corrente', value: 'Checking' },
  { label: 'Poupança', value: 'Savings' },
  { label: 'Depósito', value: 'Deposit' },
]

async function handleSubmit() {
  const ok = await submit(props.customerId)
  if (ok) {
    emit('created')
    emit('update:visible', false)
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    header="Abrir nova conta"
    :style="{ width: '24rem' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <div class="flex flex-col gap-2">
        <label for="newAccountName" class="text-sm font-medium">Nome da conta</label>
        <InputText id="newAccountName" v-model="accountName" placeholder="Ex: Conta poupança" fluid />
      </div>
      <div class="flex flex-col gap-2">
        <label for="newAccountType" class="text-sm font-medium">Tipo</label>
        <Select
          id="newAccountType"
          v-model="accountType"
          :options="accountTypeOptions"
          optionLabel="label"
          optionValue="value"
          fluid
        />
      </div>
      <div class="flex flex-col gap-2">
        <label for="newAccountPassword" class="text-sm font-medium">Senha da nova conta</label>
        <InputPassword id="newAccountPassword" v-model="password" toggleMask fluid />
      </div>

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

      <Button type="submit" label="Abrir conta" icon="pi pi-plus" :loading="loading" />
    </form>
  </Dialog>
</template>
