<script setup lang="ts">
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputPassword from 'primevue/inputpassword'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { RegisterService } from '@/services/register_service'

const router = useRouter()

const {
  firstName,
  lastName,
  username,
  email,
  password,
  confirmPassword,
  loading,
  error,
  submit,
} = RegisterService()
</script>

<template>
  <div class="flex flex-col gap-4 min-h-screen items-center justify-center bg-surface-50 p-6 dark:bg-surface-950">
    <h2 class="font-bold text-3xl text-primary">KShop</h2>
    <Card class="w-full max-w-sm">
      <template #title>Criar conta</template>
      <template #content>
        <form class="flex flex-col gap-4" @submit.prevent="submit">
          <div class="flex gap-4">
            <div class="flex flex-col gap-2 flex-1">
              <label for="firstName" class="text-sm font-medium">Nome</label>
              <InputText id="firstName" v-model="firstName" fluid />
            </div>
            <div class="flex flex-col gap-2 flex-1">
              <label for="lastName" class="text-sm font-medium">Sobrenome</label>
              <InputText id="lastName" v-model="lastName" fluid />
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <label for="username" class="text-sm font-medium">Usuário</label>
            <InputText id="username" v-model="username" autocomplete="username" fluid />
          </div>

          <div class="flex flex-col gap-2">
            <label for="email" class="text-sm font-medium">E-mail</label>
            <InputText id="email" v-model="email" type="email" fluid />
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
