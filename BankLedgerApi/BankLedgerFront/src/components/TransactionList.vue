<script setup lang="ts">
import Message from 'primevue/message'
import Tag from 'primevue/tag'
import Skeleton from 'primevue/skeleton'
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

    <div class="border dark:border-surface-700 rounded-border overflow-hidden">
      <div
        class="grid grid-cols-[1fr_auto_auto] gap-10 px-4 py-3 text-xs font-semibold uppercase tracking-wide text-surface-500 dark:text-surface-400 bg-surface-100 dark:bg-surface-800 border-b dark:border-surface-700"
      >
        <span>Descrição</span>
        <span class="text-right">Valor</span>
        <span class="text-right">Status</span>
      </div>

      <div class="max-h-[60vh] overflow-y-auto divide-y divide-surface-200 dark:divide-surface-700">
        <div v-if="loading" class="flex flex-col divide-y divide-surface-200 dark:divide-surface-700">
          <div v-for="i in 5" :key="i" class="flex items-center justify-between gap-4 p-4">
            <div class="flex items-center gap-3">
              <Skeleton shape="circle" size="1.5rem" />
              <div class="flex flex-col gap-2">
                <Skeleton width="10rem" height="1rem" />
                <Skeleton width="6rem" height="0.75rem" />
              </div>
            </div>
            <Skeleton width="4rem" height="1rem" />
          </div>
        </div>

        <div v-else-if="entries.length === 0" class="flex flex-col items-center gap-3 py-12 px-6 text-center">
          <svg viewBox="0 0 120 120" class="w-24 h-24 text-surface-300 dark:text-surface-600" fill="none">
            <rect x="20" y="34" width="80" height="58" rx="6" fill="currentColor" opacity="0.15" />
            <rect x="20" y="34" width="80" height="58" rx="6" stroke="currentColor" stroke-width="3" />
            <path d="M20 50h80" stroke="currentColor" stroke-width="3" />
            <path d="M36 66h24M36 76h16" stroke="currentColor" stroke-width="3" stroke-linecap="round" />
            <circle cx="82" cy="74" r="14" fill="currentColor" opacity="0.15" />
            <path d="M77 74h10M82 69v10" stroke="currentColor" stroke-width="3" stroke-linecap="round" />
          </svg>
          <p class="text-sm text-surface-500 dark:text-surface-400">
            Nenhuma transação encontrada no período selecionado.
          </p>
        </div>

        <button
          v-for="entry in entries"
          v-else
          :key="entry.transactionId"
          type="button"
          class="flex items-center justify-between gap-4 p-4 w-full text-left hover:bg-emphasis transition-colors cursor-pointer"
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
  </div>
</template>
