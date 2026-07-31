<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputPassword from 'primevue/inputpassword'
import Button from 'primevue/button'
import Dialog from 'primevue/dialog'
import Message from 'primevue/message'
import CurrencyInput from '@/components/CurrencyInput.vue'
import { FormaterServices } from '@/services/formater_service'
import { TransferService } from '@/services/transfer_service'
import { useBalanceStore } from '@/stores/balance_store'

const { destinationAccountNumber, amount, password, loading, error, success, submit } = TransferService()
const balanceStore = useBalanceStore()

const confirmVisible = ref(false)

const exceedsBalance = computed(
  () => balanceStore.available !== null && (amount.value ?? 0) > balanceStore.available,
)

function openConfirm() {
  error.value = ''

  if (!destinationAccountNumber.value || !amount.value) {
    error.value = 'Informe a conta de destino e o valor.'
    return
  }

  if (exceedsBalance.value) {
    error.value = 'O valor informado é maior que o seu saldo disponível.'
    return
  }

  password.value = ''
  confirmVisible.value = true
}

onMounted(() => {
  balanceStore.refresh()
})

watch(confirmVisible, (visible) => {
  if (!visible) password.value = ''
})

async function confirmSubmit() {
  await submit()
  if (!error.value) confirmVisible.value = false
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between w-full gap-6">
      <h1 class="text-2xl font-bold">Transferir</h1>
    </div>

    <Card class="w-full">
      <template #title>Nova transferência</template>
      <template #content>
        <form class="flex flex-col gap-4" @submit.prevent="openConfirm">
          <div class="flex flex-col gap-2">
            <label for="destination" class="text-sm font-medium">Conta de destino</label>
            <InputText id="destination" v-model="destinationAccountNumber" fluid />
          </div>
          <div class="flex flex-col gap-2">
            <label for="amount" class="text-sm font-medium">Valor</label>
            <CurrencyInput id="amount" v-model="amount" fluid />
            <span class="text-xs text-surface-500 dark:text-surface-400">
              Saldo disponível:
              {{ balanceStore.available !== null ? FormaterServices.FormatCurrency(balanceStore.available) : '—' }}
            </span>
          </div>

          <Message v-if="error && !confirmVisible" severity="error" variant="simple" size="small">{{ error }}</Message>
          <Message v-if="success" severity="success" variant="simple" size="small">
            Transferência de {{ FormaterServices.FormatCurrency(success.amount) }} enviada para
            {{ success.destination.number }} com sucesso.
          </Message>

          <Button type="submit" label="Transferir" icon="pi pi-send" :disabled="exceedsBalance" />
        </form>
      </template>
    </Card>

    <Dialog
      v-model:visible="confirmVisible"
      modal
      header="Confirmar transferência"
      :style="{ width: '24rem' }"
    >
      <form class="flex flex-col gap-4" @submit.prevent="confirmSubmit">
        <p class="text-sm text-surface-600 dark:text-surface-300">
          Transferir {{ FormaterServices.FormatCurrency(amount ?? 0) }} para a conta
          <strong>{{ destinationAccountNumber }}</strong>?
        </p>
        <div class="flex flex-col gap-2">
          <label for="transferPassword" class="text-sm font-medium">Senha</label>
          <InputPassword id="transferPassword" v-model="password" :feedback="false" toggleMask fluid autofocus />
        </div>

        <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

        <div class="flex justify-end gap-2">
          <Button label="Cancelar" text :disabled="loading" @click="confirmVisible = false" />
          <Button type="submit" label="Transferir" icon="pi pi-send" :loading="loading" />
        </div>
      </form>
    </Dialog>
  </div>
</template>
