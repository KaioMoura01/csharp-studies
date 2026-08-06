export interface KeycloakTokenClaims {
  sub: string
  preferred_username?: string
  email?: string
  name?: string
}

export function decodeJwt(token: string): KeycloakTokenClaims {
  const payload = token.split('.')[1] ?? ''
  const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
  return JSON.parse(json)
}
