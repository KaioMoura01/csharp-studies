import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { decodeJwt } from '@/services/jwt'
import { redirectToLogout, refreshTokens } from '@/services/keycloak_service'

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
  const sub = ref<string | null>(localStorage.getItem('sub'))
  const displayName = ref<string | null>(localStorage.getItem('displayName'))
  const email = ref<string | null>(localStorage.getItem('email'))

  const isAuthenticated = computed(() => token.value !== null)

  function login(newToken: string, newRefreshToken: string, newIdToken: string) {
    const claims = decodeJwt(newToken)

    token.value = newToken
    refreshToken.value = newRefreshToken
    idToken.value = newIdToken
    sub.value = claims.sub
    displayName.value = claims.name ?? claims.preferred_username ?? null
    email.value = claims.email ?? null

    localStorage.setItem('token', newToken)
    localStorage.setItem('refreshToken', newRefreshToken)
    localStorage.setItem('idToken', newIdToken)
    localStorage.setItem('sub', claims.sub)
    if (displayName.value) localStorage.setItem('displayName', displayName.value)
    if (email.value) localStorage.setItem('email', email.value)
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
        const t = await refreshTokens(refreshToken.value)
        login(t.access_token, t.refresh_token, t.id_token)
        return
      } catch {
        // refresh recusado: sessão morreu de vez, cai no clearSession abaixo
      }
    }

    clearSession()
  }

  function clearSession() {
    token.value = null
    refreshToken.value = null
    idToken.value = null
    sub.value = null
    displayName.value = null
    email.value = null

    localStorage.removeItem('token')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('idToken')
    localStorage.removeItem('sub')
    localStorage.removeItem('displayName')
    localStorage.removeItem('email')
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

  return { token, refreshToken, sub, displayName, email, isAuthenticated, initSession, login, logout }
})
