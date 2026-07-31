<script setup lang="ts">
import { watch } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Message from 'primevue/message'
import CurrencyInput from '@/components/CurrencyInput.vue'
import { DepositService } from '@/services/deposit_service'

const props = defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  deposited: []
}>()

const { amount, loading, error, submit } = DepositService()

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      amount.value = null
      error.value = ''
    }
  },
)

async function handleSubmit() {
  const ok = await submit()
  if (ok) {
    emit('deposited')
    emit('update:visible', false)
  }
}
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    header="Depositar"
    :style="{ width: '22rem' }"
    @update:visible="emit('update:visible', $event)"
  >
    <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
      <div class="flex flex-col gap-2">
        <label for="depositAmount" class="text-sm font-medium">Valor</label>
        <CurrencyInput id="depositAmount" v-model="amount" autofocus fluid />
      </div>

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

      <Button type="submit" label="Confirmar" icon="pi pi-wallet" :loading="loading" />
    </form>
  </Dialog>
</template>
