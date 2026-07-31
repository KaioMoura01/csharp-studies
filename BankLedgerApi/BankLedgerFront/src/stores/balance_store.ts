import { defineStore } from 'pinia'
import { ref } from 'vue'
import { apiService } from '@/services/api_service'
import type { components } from '@/types/api-schema'

type AccountDetailsResponse = components['schemas']['AccountDetailsResponse']

export const useBalanceStore = defineStore('balance', () => {
  const available = ref<number | null>(null)

  function setAvailable(value: number | string) {
    available.value = Number(value)
  }

  async function refresh() {
    const { data } = await apiService.get<AccountDetailsResponse>('accounts/me')
    setAvailable(data.currentBalance)
    return data
  }

  return { available, setAvailable, refresh }
})
