import axios from 'axios'

const keycloakUrl = import.meta.env.VITE_KEYCLOAK_URL
const realm = import.meta.env.VITE_KEYCLOAK_REALM
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID

export interface TokenResponse {
  access_token: string
  refresh_token: string
}

export async function login(username: string, password: string): Promise<TokenResponse> {
  const { data } = await axios.post<TokenResponse>(
    `${keycloakUrl}/realms/${realm}/protocol/openid-connect/token`,
    new URLSearchParams({
      grant_type: 'password',
      client_id: clientId,
      username,
      password,
    }),
  )
  return data
}
