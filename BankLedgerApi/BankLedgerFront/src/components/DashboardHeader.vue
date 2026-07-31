<script setup lang="ts">
import { ref } from 'vue'
import {FormaterServices} from "@/services/formater_service.ts";
import Card from "primevue/card";
import Button from "primevue/button";
import Skeleton from 'primevue/skeleton'
import { useRouter } from 'vue-router'
import DateRangeSelector, { type DateRange } from './DateRangeSelector.vue'
import AccountSwitcher from './AccountSwitcher.vue'
import DepositDialog from './DepositDialog.vue'
import type { components } from '@/types/api-schema'

type MyAccount = components['schemas']['AccountDetailsResponse']

defineProps<{
  myAccount?: MyAccount
  loading?: boolean
}>()

const emit = defineEmits<{
  'update:range': [range: DateRange]
  refresh: []
}>()

const router = useRouter()
const depositVisible = ref(false)
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between w-full gap-6">
      <h1 class="text-2xl font-bold">Dashboard</h1>
      <div class="flex gap-3">
        <DateRangeSelector @update:range="emit('update:range', $event)" />
        <Button label="Transferir" icon="pi pi-send" @click="router.push({ name: 'transfer' })" />
      </div>
    </div>
    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <Card>
        <template #title>Saldo</template>
        <template #content>
          <div class="flex items-center justify-between gap-4">
            <Skeleton v-if="loading" width="8rem" height="2rem" />
            <p v-else class="text-2xl font-semibold text-primary">
              {{ FormaterServices.FormatCurrency(myAccount ? myAccount.currentBalance : '0') }}
            </p>
            <Button
              label="Depositar"
              icon="pi pi-wallet"
              severity="secondary"
              size="small"
              @click="depositVisible = true"
            />
          </div>
        </template>
      </Card>
      <Card>
        <template #title>Conta</template>
        <template #content>
          <Skeleton v-if="loading" width="100%" height="2.5rem" />
          <AccountSwitcher
            v-else
            :current-account-number="myAccount?.number"
            :customer-id="myAccount?.owner.id"
            @refresh="emit('refresh')"
          />
        </template>
      </Card>
    </div>

    <DepositDialog v-model:visible="depositVisible" @deposited="emit('refresh')" />
  </div>
</template>

<style scoped>

</style>
