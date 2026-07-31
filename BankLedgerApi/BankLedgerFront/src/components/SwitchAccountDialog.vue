<script setup lang="ts">
import { watch } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { SwitchAccountService } from '@/services/switch_account_service'

const props = defineProps<{
  visible: boolean
  accountId: string
  accountNumber: string
  accountName: string
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  switched: []
}>()

const { loading, error, switchAccount } = SwitchAccountService()

watch(
  () => props.visible,
  (visible) => {
    if (visible) error.value = ''
  },
)

async function confirm() {
  const ok = await switchAccount(props.accountId, props.accountNumber)
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
    header="Trocar de conta"
    :style="{ width: '22rem' }"
    @update:visible="emit('update:visible', $event)"
  >
    <div class="flex flex-col gap-4">
      <p class="text-sm text-surface-600 dark:text-surface-300">
        Trocar para a conta <strong>{{ accountName }}</strong> (Nº {{ accountNumber }})?
      </p>

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

      <div class="flex justify-end gap-2">
        <Button label="Cancelar" text :disabled="loading" @click="emit('update:visible', false)" />
        <Button label="Trocar conta" icon="pi pi-refresh" :loading="loading" @click="confirm" />
      </div>
    </div>
  </Dialog>
</template>
