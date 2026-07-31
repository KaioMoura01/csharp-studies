<script setup lang="ts">
import { ref, watch } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Tag from 'primevue/tag'
import Message from 'primevue/message'
import { useConfirm } from 'primevue/useconfirm'
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

const confirm = useConfirm()
const { loading, error, reverse } = ReversalService()
const reversed = ref(false)

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      reversed.value = false
      error.value = ''
    }
  },
)

function close() {
  emit('update:visible', false)
}

function confirmReversal() {
  if (!props.entry) return

  confirm.require({
    header: 'Confirmar estorno',
    message: `Deseja pedir o estorno de ${FormaterServices.FormatCurrency(props.entry.amount)} enviado para ${props.entry.counterpartyName}?`,
    icon: 'pi pi-exclamation-triangle',
    rejectProps: { label: 'Cancelar', severity: 'secondary', outlined: true },
    acceptProps: { label: 'Pedir estorno', severity: 'danger' },
    accept: requestReversal,
  })
}

async function requestReversal() {
  if (!props.entry) return
  const result = await reverse(props.entry.transferId)
  if (result) {
    reversed.value = true
    emit('reversed')
  }
}

function statusSeverity(status: StatementEntry['status']) {
  switch (status) {
    case 'Completed':
      return 'success'
    case 'Pending':
      return 'warn'
    case 'Failed':
      return 'danger'
    case 'Reversed':
      return 'contrast'
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

      <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>
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
        :loading="loading"
        @click="confirmReversal"
      />
    </template>
  </Dialog>
</template>
