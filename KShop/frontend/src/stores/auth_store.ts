import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { decodeJwt } from '@/services/jwt'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const sub = ref<string | null>(localStorage.getItem('sub'))
  const displayName = ref<string | null>(localStorage.getItem('displayName'))
  const email = ref<string | null>(localStorage.getItem('email'))

  const isAuthenticated = computed(() => token.value !== null)

  function login(newToken: string, newRefreshToken: string) {
    const claims = decodeJwt(newToken)

    token.value = newToken
    refreshToken.value = newRefreshToken
    sub.value = claims.sub
    displayName.value = claims.name ?? claims.preferred_username ?? null
    email.value = claims.email ?? null

    localStorage.setItem('token', newToken)
    localStorage.setItem('refreshToken', newRefreshToken)
    localStorage.setItem('sub', claims.sub)
    if (displayName.value) localStorage.setItem('displayName', displayName.value)
    if (email.value) localStorage.setItem('email', email.value)
  }

  function logout() {
    token.value = null
    refreshToken.value = null
    sub.value = null
    displayName.value = null
    email.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('sub')
    localStorage.removeItem('displayName')
    localStorage.removeItem('email')
  }

  return { token, refreshToken, sub, displayName, email, isAuthenticated, login, logout }
})
