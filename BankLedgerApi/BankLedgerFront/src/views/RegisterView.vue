<script setup lang="ts">
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputMask from 'primevue/inputmask'
import InputPassword from 'primevue/inputpassword'
import Select from 'primevue/select'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { RegisterService } from '@/services/register_service'

const router = useRouter()

const {
  name,
  documentType,
  documentNumber,
  documentMask,
  accountName,
  accountType,
  password,
  confirmPassword,
  loading,
  error,
  submit,
} = RegisterService()

const documentTypeOptions = [
  { label: 'CPF', value: 'Cpf' },
  { label: 'CNPJ', value: 'Cnpj' },
]

const accountTypeOptions = [
  { label: 'Corrente', value: 'Checking' },
  { label: 'Poupança', value: 'Savings' },
  { label: 'Depósito', value: 'Deposit' },
]
</script>

<template>
  <div class="flex flex-col gap-4 min-h-screen items-center justify-center bg-surface-50 p-6 dark:bg-surface-950">
    <h2 class="font-bold text-3xl text-primary">BankLedger</h2>
    <Card class="w-full max-w-sm">
      <template #title>Abrir conta</template>
      <template #content>
        <form class="flex flex-col gap-4" @submit.prevent="submit">
          <div class="flex flex-col gap-2">
            <label for="name" class="text-sm font-medium">Nome completo</label>
            <InputText id="name" v-model="name" fluid />
          </div>

          <div class="flex gap-4">
            <div class="flex flex-col gap-2 w-32">
              <label for="documentType" class="text-sm font-medium">Documento</label>
              <Select
                id="documentType"
                v-model="documentType"
                :options="documentTypeOptions"
                optionLabel="label"
                optionValue="value"
                fluid
              />
            </div>
            <div class="flex flex-col gap-2 flex-1">
              <label for="documentNumber" class="text-sm font-medium">Número</label>
              <InputMask
                id="documentNumber"
                v-model="documentNumber"
                :mask="documentMask"
                :placeholder="documentMask"
                fluid
              />
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <label for="accountName" class="text-sm font-medium">Nome da conta</label>
            <InputText id="accountName" v-model="accountName" placeholder="Ex: Conta principal" fluid />
          </div>

          <div class="flex flex-col gap-2">
            <label for="accountType" class="text-sm font-medium">Tipo de conta</label>
            <Select
              id="accountType"
              v-model="accountType"
              :options="accountTypeOptions"
              optionLabel="label"
              optionValue="value"
              fluid
            />
          </div>

          <div class="flex flex-col gap-2">
            <label for="password" class="text-sm font-medium">Senha</label>
            <InputPassword id="password" v-model="password" toggleMask fluid />
          </div>

          <div class="flex flex-col gap-2">
            <label for="confirmPassword" class="text-sm font-medium">Confirmar senha</label>
            <InputPassword id="confirmPassword" v-model="confirmPassword" :feedback="false" toggleMask fluid />
          </div>

          <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>

          <Button type="submit" label="Cadastrar" icon="pi pi-user-plus" :loading="loading" />
          <Button
            type="button"
            :disabled="loading"
            link
            label="Já tenho conta"
            :pt="{ root: { style: { textDecoration: 'none' } } }"
            style="--p-button-padding-y: 0"
            @click="router.push({ name: 'login' })"
          />
        </form>
      </template>
    </Card>
  </div>
</template>
