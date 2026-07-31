import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const customerId = ref<string | null>(localStorage.getItem('customerId'))
  const accountId = ref<string | null>(localStorage.getItem('accountId'))
  const accountNumber = ref<string | null>(localStorage.getItem('accountNumber'))

  const isAuthenticated = computed(() => token.value !== null)

  function login(newToken: string, newCustomerId: string, newAccountId: string, newAccountNumber: string) {
    token.value = newToken
    customerId.value = newCustomerId
    accountId.value = newAccountId
    accountNumber.value = newAccountNumber
    localStorage.setItem('token', newToken)
    localStorage.setItem('customerId', newCustomerId)
    localStorage.setItem('accountId', newAccountId)
    localStorage.setItem('accountNumber', newAccountNumber)
  }

  function logout() {
    token.value = null
    customerId.value = null
    accountId.value = null
    accountNumber.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('customerId')
    localStorage.removeItem('accountId')
    localStorage.removeItem('accountNumber')
  }

  return { token, customerId, accountId, accountNumber, isAuthenticated, login, logout }
})
