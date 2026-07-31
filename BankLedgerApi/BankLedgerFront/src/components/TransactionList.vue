<script setup lang="ts">
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import ProgressSpinner from 'primevue/progressspinner'
import { FormaterServices } from '@/services/formater_service'
import type { StatementEntry } from '@/services/statement_service'

defineProps<{
  entries: StatementEntry[]
  loading: boolean
  error?: string
}>()

const emit = defineEmits<{
  select: [entry: StatementEntry]
}>()

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
</script>

<template>
  <div class="flex flex-col gap-3">
    <h2 class="text-lg font-semibold">Transações</h2>

    <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

    <div v-if="loading" class="flex justify-center py-8">
      <ProgressSpinner style="width: 2.5rem; height: 2.5rem" />
    </div>

    <p v-else-if="entries.length === 0" class="text-sm text-surface-500 dark:text-surface-400">
      Nenhuma transação encontrada no período selecionado.
    </p>

    <div
      v-else
      class="flex flex-col divide-y divide-surface-200 dark:divide-surface-700 border dark:border-surface-700 rounded-border overflow-hidden"
    >
      <button
        v-for="entry in entries"
        :key="entry.transactionId"
        type="button"
        class="flex items-center justify-between gap-4 p-4 text-left hover:bg-emphasis transition-colors cursor-pointer"
        @click="emit('select', entry)"
      >
        <div class="flex items-center gap-3">
          <i
            :class="
              entry.direction === 'Credit'
                ? 'pi pi-arrow-down-left text-green-500'
                : 'pi pi-arrow-up-right text-red-500'
            "
          />
          <div class="flex flex-col">
            <span class="font-medium">
              {{ entry.direction === 'Credit' ? 'Recebido de' : 'Enviado para' }}
              {{ entry.counterpartyAccountNumber }}
            </span>
            <span class="text-xs text-surface-500 dark:text-surface-400">
              {{ FormaterServices.FormatDateTime(entry.timestamp) }}
            </span>
          </div>
        </div>
        <div class="flex items-center gap-3">
          <span
            class="font-semibold"
            :class="entry.direction === 'Credit' ? 'text-green-500' : 'text-red-500'"
          >
            {{ entry.direction === 'Credit' ? '+' : '-' }}{{ FormaterServices.FormatCurrency(entry.amount) }}
          </span>
          <Tag :value="FormaterServices.FormatStatus(entry.status)" :severity="statusSeverity(entry.status)" />
        </div>
      </button>
    </div>
  </div>
</template>
