import axios from 'axios'

const keycloakUrl = import.meta.env.VITE_KEYCLOAK_URL
const realm = import.meta.env.VITE_KEYCLOAK_REALM
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID

const tokenEndpoint = `${keycloakUrl}/realms/${realm}/protocol/openid-connect/token`
const authEndpoint = `${keycloakUrl}/realms/${realm}/protocol/openid-connect/auth`
const logoutEndpoint = `${keycloakUrl}/realms/${realm}/protocol/openid-connect/logout`

const PKCE_VERIFIER_KEY = 'oidc_pkce_verifier'
const PKCE_STATE_KEY = 'oidc_state'

export interface TokenResponse {
  access_token: string
  refresh_token: string
  id_token: string
  expires_in: number
}

/** Troca o refresh_token por tokens novos. Usado para obter um id_token válido no logout. */
export async function refreshTokens(refreshTokenValue: string): Promise<TokenResponse> {
  const { data } = await axios.post<TokenResponse>(
    tokenEndpoint,
    new URLSearchParams({
      grant_type: 'refresh_token',
      client_id: clientId,
      refresh_token: refreshTokenValue,
    }),
    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
  )
  return data
}

/**
 * Encerra a sessão no Keycloak (não só o token local). Sem isso, o cookie de sessão do
 * Keycloak continua vivo e um novo login (inclusive com outra conta Google) é resolvido
 * por SSO silencioso, devolvendo a sessão antiga em vez de pedir autenticação de novo.
 *
 * O `id_token_hint` precisa estar VÁLIDO: o id_token dura o mesmo que o access token
 * (5 min por padrão), mas fica guardado até o usuário clicar em "Sair" — que costuma ser
 * bem depois. Mandar um expirado faz o Keycloak responder "Invalid parameter: id_token_hint".
 * Por isso quem chama deve renovar antes; ver `logout()` na store.
 */
export function redirectToLogout(idTokenHint: string): void {
  const params = new URLSearchParams({
    id_token_hint: idTokenHint,
    post_logout_redirect_uri: `${window.location.origin}/login`,
  })
  window.location.href = `${logoutEndpoint}?${params.toString()}`
}

function getCallbackUrl(): string {
  return `${window.location.origin}/callback`
}

function base64UrlEncode(bytes: ArrayBuffer | Uint8Array): string {
  const array = bytes instanceof Uint8Array ? bytes : new Uint8Array(bytes)
  let binary = ''
  for (const byte of array) binary += String.fromCharCode(byte)
  return btoa(binary).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '')
}

function generateRandomString(length: number): string {
  const array = new Uint8Array(length)
  crypto.getRandomValues(array)
  return base64UrlEncode(array)
}

async function sha256(value: string): Promise<ArrayBuffer> {
  return crypto.subtle.digest('SHA-256', new TextEncoder().encode(value))
}

/**
 * Único caminho de login: manda o navegador para a tela do Keycloak (authorization code + PKCE).
 * Por ser um fluxo interativo, cria a sessão SSO de navegador — é o que faz o login valer nos
 * outros projetos do realm compartilhado.
 *
 * Não precisa de `prompt=none` para detectar sessão existente: o Keycloak já resolve os dois
 * casos sozinho — havendo sessão devolve o code na hora, sem UI; não havendo, mostra o login.
 */
export async function redirectToLogin(): Promise<void> {
  const codeVerifier = generateRandomString(64)
  const codeChallenge = base64UrlEncode(await sha256(codeVerifier))
  const state = generateRandomString(32)

  sessionStorage.setItem(PKCE_VERIFIER_KEY, codeVerifier)
  sessionStorage.setItem(PKCE_STATE_KEY, state)

  const params = new URLSearchParams({
    client_id: clientId,
    redirect_uri: getCallbackUrl(),
    response_type: 'code',
    scope: 'openid profile email',
    state,
    code_challenge: codeChallenge,
    code_challenge_method: 'S256',
  })

  window.location.href = `${authEndpoint}?${params.toString()}`
}

/** Troca o "code" recebido no callback pelo token, validando o "state" salvo antes do redirect. */
export async function exchangeCodeForToken(code: string, state: string): Promise<TokenResponse> {
  const savedState = sessionStorage.getItem(PKCE_STATE_KEY)
  const codeVerifier = sessionStorage.getItem(PKCE_VERIFIER_KEY)
  sessionStorage.removeItem(PKCE_STATE_KEY)
  sessionStorage.removeItem(PKCE_VERIFIER_KEY)

  if (!savedState || savedState !== state || !codeVerifier) {
    throw new Error('Estado OIDC inválido ou expirado.')
  }

  const { data } = await axios.post<TokenResponse>(
    tokenEndpoint,
    new URLSearchParams({
      grant_type: 'authorization_code',
      client_id: clientId,
      code,
      redirect_uri: getCallbackUrl(),
      code_verifier: codeVerifier,
    }),
    { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } },
  )
  return data
}
