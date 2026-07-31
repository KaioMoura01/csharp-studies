<script setup lang="ts">
import { ref, watch } from 'vue'
import Select from 'primevue/select'
import Button from 'primevue/button'
import { FormaterServices } from '@/services/formater_service'
import { CustomerAccountsService, type AccountSummary } from '@/services/customer_accounts_service'
import SwitchAccountDialog from './SwitchAccountDialog.vue'
import OpenAccountDialog from './OpenAccountDialog.vue'

const props = defineProps<{
  currentAccountNumber?: string
  customerId?: string
}>()

const emit = defineEmits<{
  refresh: []
}>()

const { accounts, loading, fetchAccounts } = CustomerAccountsService()

const switchTarget = ref<AccountSummary | null>(null)
const switchDialogVisible = ref(false)
const openAccountVisible = ref(false)

watch(
  () => props.customerId,
  (customerId) => {
    if (customerId) fetchAccounts(customerId)
  },
  { immediate: true },
)

function handleSelect(account: AccountSummary) {
  if (account.number === props.currentAccountNumber) return
  switchTarget.value = account
  switchDialogVisible.value = true
}

function handleSwitched() {
  switchTarget.value = null
  emit('refresh')
}

function handleAccountOpened() {
  if (props.customerId) fetchAccounts(props.customerId)
  emit('refresh')
}
</script>

<template>
  <div>
    <Select
      :modelValue="accounts.find((a) => a.number === currentAccountNumber)"
      :options="accounts"
      optionLabel="name"
      :loading="loading"
      placeholder="Carregando..."
      class="w-full"
      @update:modelValue="handleSelect"
    >
      <template #value="slotProps">
        <div v-if="slotProps.value" class="flex flex-col text-left">
          <span class="font-medium">{{ slotProps.value.name }}</span>
          <span class="text-xs text-surface-500 dark:text-surface-400">Nº {{ slotProps.value.number }}</span>
        </div>
        <span v-else>{{ slotProps.placeholder }}</span>
      </template>
      <template #option="slotProps">
        <div class="flex flex-col">
          <span class="font-medium">{{ slotProps.option.name }}</span>
          <span class="text-xs text-surface-500 dark:text-surface-400">
            Nº {{ slotProps.option.number }} · {{ FormaterServices.FormatCurrency(slotProps.option.currentBalance) }}
          </span>
        </div>
      </template>
      <template #footer>
        <div class="p-2 border-t border-surface-200 dark:border-surface-700">
          <Button
            label="Abrir nova conta"
            icon="pi pi-plus"
            text
            size="small"
            fluid
            @click="openAccountVisible = true"
          />
        </div>
      </template>
    </Select>

    <SwitchAccountDialog
      v-if="switchTarget"
      v-model:visible="switchDialogVisible"
      :account-number="switchTarget.number"
      :account-name="switchTarget.name"
      @switched="handleSwitched"
    />

    <OpenAccountDialog
      v-if="customerId"
      v-model:visible="openAccountVisible"
      :customer-id="customerId"
      @created="handleAccountOpened"
    />
  </div>
</template>
