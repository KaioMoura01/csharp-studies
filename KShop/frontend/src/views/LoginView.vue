<script setup lang="ts">
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import InputPassword from 'primevue/inputpassword'
import Button from 'primevue/button'
import Message from 'primevue/message'
import Divider from 'primevue/divider'
import { AuthService } from '@/services/auth_service'

const router = useRouter()
const { username, password, error, loading, submit } = AuthService()
</script>

<template>
  <div class="flex flex-col gap-4 min-h-screen items-center justify-center bg-surface-50 p-6 dark:bg-surface-950">
    <h2 class="font-bold text-3xl text-primary">KShop</h2>
    <Card class="w-full max-w-sm">
      <template #title>Entrar</template>
      <template #content>
        <form class="flex flex-col gap-4" @submit.prevent="submit">
          <div class="flex flex-col gap-2">
            <label for="username" class="text-sm font-medium">Usuário</label>
            <InputText id="username" v-model="username" autocomplete="username" fluid />
          </div>
          <div class="flex flex-col gap-2">
            <label for="password" class="text-sm font-medium">Senha</label>
            <InputPassword id="password" v-model="password" :feedback="false" toggleMask fluid />
          </div>
          <Message v-if="error" severity="error" variant="simple" size="small">{{ error }}</Message>
          <Button type="submit" label="Entrar" :loading="loading" icon="pi pi-sign-in" />
          <Divider align="center" type="dotted" style="--p-divider-horizontal-margin: 0">
            <code class="uppercase text-xs">Ou</code>
          </Divider>
          <Button
            type="button"
            :disabled="loading"
            link
            label="Crie sua conta"
            style="--p-button-padding-y: 0"
            @click="router.push({ name: 'register' })"
          />
        </form>
      </template>
    </Card>
  </div>
</template>
