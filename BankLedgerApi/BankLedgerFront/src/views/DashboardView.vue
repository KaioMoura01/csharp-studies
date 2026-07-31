<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { DashboardService } from '../services/dashboard_service'
import { StatementService, type StatementEntry } from '../services/statement_service'
import DashboardHeader from '../components/DashboardHeader.vue'
import TransactionList from '../components/TransactionList.vue'
import TransactionDetailModal from '../components/TransactionDetailModal.vue'
import type { DateRange } from '../components/DateRangeSelector.vue'

const { myAccount, loading: accountLoading, GetData } = DashboardService()
const { entries, loading: statementLoading, error: statementError, fetchStatement } = StatementService()

const selectedEntry = ref<StatementEntry | null>(null)
const detailVisible = ref(false)
const currentRange = ref<DateRange | null>(null)

function openDetail(entry: StatementEntry) {
  selectedEntry.value = entry
  detailVisible.value = true
}

function handleRangeChange(range: DateRange) {
  currentRange.value = range
  fetchStatement(range.from, range.to)
}

function handleReversed() {
  detailVisible.value = false
  GetData()
  if (currentRange.value) fetchStatement(currentRange.value.from, currentRange.value.to)
}

function handleAccountRefresh() {
  GetData()
  if (currentRange.value) fetchStatement(currentRange.value.from, currentRange.value.to)
}

onMounted(() => {
  GetData()
})
</script>

<template>
  <div class="flex flex-col gap-6">
    <DashboardHeader
      :my-account="myAccount"
      :loading="accountLoading"
      @update:range="handleRangeChange"
      @refresh="handleAccountRefresh"
    />

    <TransactionList
      :entries="entries"
      :loading="statementLoading"
      :error="statementError"
      @select="openDetail"
    />

    <TransactionDetailModal
      v-model:visible="detailVisible"
      :entry="selectedEntry"
      @reversed="handleReversed"
    />
  </div>
</template>
