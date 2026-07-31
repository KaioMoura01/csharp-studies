<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import InputPassword from 'primevue/inputpassword'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { SwitchAccountService } from '@/services/switch_account_service'

const props = defineProps<{
  visible: boolean
  accountNumber: string
  accountName: string
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  switched: []
}>()

const password = ref('')
const { loading, error, switchAccount } = SwitchAccountService()

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      password.value = ''
      error.value = ''
    }
  },
)

async function confirm() {
  const ok = await switchAccount(props.accountNumber, password.value)
  if (ok) {
    emit('switched')
    emit('update:visible', false)
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    header="Confirmar troca de conta"
    :style="{ width: '22rem' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="flex flex-col gap-4" @submit.prevent="confirm">
      <p class="text-sm text-surface-600 dark:text-surface-300">
        Informe a senha da conta <strong>{{ accountName }}</strong> (Nº {{ accountNumber }}) para trocar.
      </p>
      <div class="flex flex-col gap-2">
        <label for="switchPassword" class="text-sm font-medium">Senha</label>
        <InputPassword id="switchPassword" v-model="password" :feedback="false" toggleMask fluid autofocus />
      </div>

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

      <Button type="submit" label="Trocar conta" icon="pi pi-refresh" :loading="loading" />
    </form>
  </Dialog>
</template>
