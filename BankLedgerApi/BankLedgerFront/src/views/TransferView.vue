<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useConfirm } from 'primevue/useconfirm'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { FormaterServices } from '@/services/formater_service'
import { TransferService } from '@/services/transfer_service'

const router = useRouter()
const confirm = useConfirm()
const { destinationAccountNumber, amount, loading, error, success, submit } = TransferService()

function confirmSubmit() {
  error.value = ''

  if (!destinationAccountNumber.value || !amount.value) {
    error.value = 'Informe a conta de destino e o valor.'
    return
  }

  confirm.require({
    header: 'Confirmar transferência',
    message: `Transferir ${FormaterServices.FormatCurrency(amount.value)} para a conta ${destinationAccountNumber.value}?`,
    icon: 'pi pi-send',
    rejectProps: { label: 'Cancelar', severity: 'secondary', outlined: true },
    acceptProps: { label: 'Transferir' },
    accept: submit,
  })
}
</script>

<template>
  <div class="flex flex-col gap-6">
    <div class="flex items-center justify-between w-full gap-6">
<!--      <Button-->
<!--        label="Voltar"-->
<!--        icon="pi pi-arrow-left"-->
<!--        severity="secondary"-->
<!--        text-->
<!--        @click="router.push({ name: 'dashboard' })"-->
<!--      />-->
      <h1 class="text-2xl font-bold">Transferir</h1>
    </div>

    <Card class="w-full">
      <template #title>Nova transferência</template>
      <template #content>
        <form class="flex flex-col gap-4" @submit.prevent="confirmSubmit">
          <div class="flex flex-col gap-2">
            <label for="destination" class="text-sm font-medium">Conta de destino</label>
            <InputText id="destination" v-model="destinationAccountNumber" fluid />
          </div>
          <div class="flex flex-col gap-2">
            <label for="amount" class="text-sm font-medium">Valor</label>
            <InputNumber
              id="amount"
              v-model="amount"
              mode="currency"
              currency="BRL"
              locale="pt-BR"
              :min="0"
              fluid
            />
          </div>

          <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>
          <Message v-if="success" severity="success" variant="simple" size="small">
            Transferência de {{ FormaterServices.FormatCurrency(success.amount) }} enviada para
            {{ success.destination.number }} com sucesso.
          </Message>

          <Button type="submit" label="Transferir" icon="pi pi-send" :loading="loading" />
        </form>
      </template>
    </Card>
  </div>
</template>
