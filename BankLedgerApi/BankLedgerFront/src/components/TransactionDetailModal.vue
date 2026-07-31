<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import InputPassword from 'primevue/inputpassword'
import { FormaterServices } from '@/services/formater_service'
import { ReversalService } from '@/services/reversal_service'
import type { StatementEntry } from '@/services/statement_service'

const props = defineProps<{
  visible: boolean
  entry: StatementEntry | null
}>()

const emit = defineEmits<{
  'update:visible': [value: boolean]
  reversed: []
}>()

const { loading, error, reverse } = ReversalService()
const reversed = ref(false)
const confirmVisible = ref(false)
const password = ref('')

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      reversed.value = false
      error.value = ''
    }
  },
)

watch(confirmVisible, (visible) => {
  if (!visible) {
    password.value = ''
    error.value = ''
  }
})

function close() {
  emit('update:visible', false)
}

function openConfirmReversal() {
  password.value = ''
  confirmVisible.value = true
}

async function confirmReversal() {
  if (!props.entry) return
  if (!password.value) {
    error.value = 'Informe sua senha.'
    return
  }

  const result = await reverse(props.entry.transferId, password.value)
  if (result) {
    reversed.value = true
    confirmVisible.value = false
    emit('reversed')
  }
}

function statusSeverity(status: StatementEntry['status']) {
  switch (status) {
    case 'Completed':
      return 'success'
    case 'Pending':
      return 'info'
    case 'Failed':
      return 'danger'
    case 'Reversed':
      return 'warn'
  }
}

const canReverse = () =>
  props.entry?.status === 'Completed' && props.entry?.direction === 'Debit' && !reversed.value
</script>

<template>
  <Dialog
    :visible="visible"
    modal
    header="Detalhes da transação"
    :style="{ width: '26rem' }"
    @update:visible="emit('update:visible', $event)"
  >
    <div v-if="entry" class="flex flex-col gap-3">
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">Data</span>
        <span>{{ FormaterServices.FormatDateTime(entry.timestamp) }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">Tipo</span>
        <span>{{ entry.direction === 'Credit' ? 'Recebimento' : 'Envio' }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">
          Conta {{ entry.direction === 'Credit' ? 'de origem' : 'de destino' }}
        </span>
        <span>Nº {{ entry.counterpartyAccountNumber }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">
          Titular da Conta
        </span>
        <span>{{ entry.counterpartyName }}</span>
      </div>
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">Valor</span>
        <span
          class="font-semibold"
          :class="entry.direction === 'Credit' ? 'text-green-500' : 'text-red-500'"
        >
          {{ entry.direction === 'Credit' ? '+' : '-' }}{{ FormaterServices.FormatCurrency(entry.amount) }}
        </span>
      </div>
      <div class="flex justify-between">
        <span class="text-surface-500 dark:text-surface-400">Saldo após</span>
        <span>{{ FormaterServices.FormatCurrency(entry.balanceAfter) }}</span>
      </div>
      <div class="flex items-center justify-between">
        <span class="text-surface-500 dark:text-surface-400">Status</span>
        <Tag :value="FormaterServices.FormatStatus(entry.status)" :severity="statusSeverity(entry.status)" />
      </div>

      <Message v-if="reversed" severity="success" variant="simple" size="small">
        Estorno solicitado com sucesso.
      </Message>
    </div>

    <template #footer>
      <Button label="Fechar" severity="secondary" text @click="close" />
      <Button
        v-if="canReverse()"
        label="Pedir estorno"
        severity="danger"
        @click="openConfirmReversal"
      />
    </template>
  </Dialog>

  <Dialog
    v-model:visible="confirmVisible"
    modal
    header="Confirmar estorno"
    :style="{ width: '22rem' }"
  >
    <form v-if="entry" class="flex flex-col gap-4" @submit.prevent="confirmReversal">
      <p class="text-sm text-surface-600 dark:text-surface-300">
        Deseja pedir o estorno de {{ FormaterServices.FormatCurrency(entry.amount) }} enviado para
        {{ entry.counterpartyName }}?
      </p>
      <div class="flex flex-col gap-2">
        <label for="reversalPassword" class="text-sm font-medium">Senha</label>
        <InputPassword id="reversalPassword" v-model="password" :feedback="false" toggleMask fluid autofocus />
      </div>

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

      <div class="flex justify-end gap-2">
        <Button label="Cancelar" text :disabled="loading" @click="confirmVisible = false" />
        <Button type="submit" label="Pedir estorno" severity="danger" :loading="loading" />
      </div>
    </form>
  </Dialog>
</template>
