import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const accountNumber = ref<string | null>(localStorage.getItem('accountNumber'))

  const isAuthenticated = computed(() => token.value !== null)

  function login(newToken: string, number: string) {
    token.value = newToken
    accountNumber.value = number
    localStorage.setItem('token', newToken)
    localStorage.setItem('accountNumber', number)
  }

  function logout() {
    token.value = null
    accountNumber.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('accountNumber')
  }

  return { token, accountNumber, isAuthenticated, login, logout }
})
