export interface JwtClaims {
  sub: string
  preferred_username?: string
  email?: string
  name?: string
  realm_access?: { roles: string[] }
  iss?: string
  exp: number
  [key: string]: unknown
}

export function decodeJwt(token: string): JwtClaims {
  const payload = token.split('.')[1]
  const normalized = payload.replace(/-/g, '+').replace(/_/g, '/')
  const decoded = atob(normalized)
  return JSON.parse(decodeURIComponent(escape(decoded))) as JwtClaims
}
