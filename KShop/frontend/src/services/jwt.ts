export interface KeycloakTokenClaims {
  sub: string
  preferred_username?: string
  email?: string
  name?: string
  iss?: string
  exp?: number
}

export function decodeJwt(token: string): KeycloakTokenClaims {
  const payload = token.split('.')[1] ?? ''
  const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
  return JSON.parse(json)
}
