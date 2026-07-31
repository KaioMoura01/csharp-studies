<script setup lang="ts">
import {FormaterServices} from "@/services/formater_service.ts";
import Card from "primevue/card";
import Button from "primevue/button";
import { useRouter } from 'vue-router'
import DateRangeSelector, { type DateRange } from './DateRangeSelector.vue'
import AccountSwitcher from './AccountSwitcher.vue'
import type { components } from '@/types/api-schema'

type MyAccount = components['schemas']['AccountDetailsResponse']

defineProps<{
  myAccount?: MyAccount
}>()

const emit = defineEmits<{
  'update:range': [range: DateRange]
  refresh: []
}>()

const router = useRouter()
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
          <p class="text-2xl font-semibold text-primary">
            {{ FormaterServices.FormatCurrency(myAccount ? myAccount.currentBalance : '0') }}
          </p>
        </template>
      </Card>
      <Card>
        <template #title>Conta</template>
        <template #content>
          <AccountSwitcher
            :current-account-number="myAccount?.number"
            :customer-id="myAccount?.owner.id"
            @refresh="emit('refresh')"
          />
        </template>
      </Card>
    </div>
  </div>
</template>

<style scoped>

</style>
