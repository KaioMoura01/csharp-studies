import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { decodeJwt, type JwtClaims } from '@/services/jwt'
import {
  exchangeCodeForToken,
  redirectToLogout,
  refreshTokens,
  type TokenResponse,
} from '@/services/keycloak_service'

const expectedIssuer = `${import.meta.env.VITE_KEYCLOAK_URL}/realms/${import.meta.env.VITE_KEYCLOAK_REALM}`

type TokenStatus = 'ok' | 'expired' | 'invalid'

/**
 * `invalid` = não dá para aproveitar de jeito nenhum (malformado, ou emitido por outro realm —
 * sobra típica de migração). `expired` = o realm é o certo, só passou da validade, então ainda
 * dá para renovar pelo refresh_token.
 */
function checkToken(value: string | null): TokenStatus {
  if (!value) return 'invalid'
  try {
    const c = decodeJwt(value)
    if (c.iss !== expectedIssuer) return 'invalid'
    if (typeof c.exp !== 'number' || c.exp * 1000 <= Date.now()) return 'expired'
    return 'ok'
  } catch {
    return 'invalid'
  }
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const refreshToken = ref<string | null>(localStorage.getItem('refreshToken'))
  const idToken = ref<string | null>(localStorage.getItem('idToken'))
  const claims = ref<JwtClaims | null>(
    localStorage.getItem('claims') ? (JSON.parse(localStorage.getItem('claims')!) as JwtClaims) : null,
  )

  const isAuthenticated = computed(() => !!token.value)
  const username = computed(() => claims.value?.preferred_username ?? claims.value?.name ?? '')
  const roles = computed(() => claims.value?.realm_access?.roles ?? [])
  const isAdmin = computed(() => roles.value.includes('admin'))

  function setSession(response: TokenResponse) {
    token.value = response.access_token
    refreshToken.value = response.refresh_token
    idToken.value = response.id_token
    claims.value = decodeJwt(response.access_token)

    localStorage.setItem('token', token.value)
    localStorage.setItem('refreshToken', refreshToken.value)
    localStorage.setItem('idToken', idToken.value)
    localStorage.setItem('claims', JSON.stringify(claims.value))
  }

  /**
   * Confere o token guardado antes do app renderizar. Sem isso, `isAuthenticated` acredita em
   * qualquer string no localStorage — foi assim que um token de realm antigo fez o app parecer
   * logado e quebrou o logout com "Invalid parameter: id_token_hint".
   *
   * Token só expirado não desloga ninguém: o access token dura 5 min mas a sessão dura bem mais,
   * então renovamos pelo refresh_token. Só limpa de fato quando não há como recuperar.
   */
  async function initSession() {
    const status = checkToken(token.value)
    if (status === 'ok') return

    if (status === 'expired' && refreshToken.value) {
      try {
        setSession(await refreshTokens(refreshToken.value))
        return
      } catch {
        // refresh recusado: sessão morreu de vez, cai no clearSession abaixo
      }
    }

    clearSession()
  }

  async function completeSsoLogin(code: string, state: string) {
    setSession(await exchangeCodeForToken(code, state))
  }

  function clearSession() {
    token.value = null
    refreshToken.value = null
    idToken.value = null
    claims.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('idToken')
    localStorage.removeItem('claims')
  }

  /**
   * Encerra a sessão local e a sessão no Keycloak (sem isso, um próximo login seria resolvido
   * por SSO silencioso e devolveria a conta antiga).
   *
   * O hint precisa ser um id_token que ESTE realm reconheça. O que está no localStorage pode
   * ser de outro realm (sobra de migração) ou inválido, e aí o Keycloak responde
   * "Invalid parameter: id_token_hint". Por isso nunca reaproveitamos o id_token guardado:
   * só usamos um recém-obtido pelo refresh. Se o refresh falhar, a sessão já não é válida —
   * basta limpar o estado local e voltar para o login.
   */
  async function logout() {
    const currentRefreshToken = refreshToken.value
    let hint: string | null = null

    if (currentRefreshToken) {
      try {
        hint = (await refreshTokens(currentRefreshToken)).id_token
      } catch {
        hint = null
      }
    }

    clearSession()

    if (hint) {
      redirectToLogout(hint)
    } else {
      window.location.href = '/login'
    }
  }

  return {
    token,
    claims,
    isAuthenticated,
    username,
    roles,
    isAdmin,
    initSession,
    completeSsoLogin,
    logout,
  }
})
