# TodoForSso

API REST (ASP.NET Core / .NET 10) de uma lista de tarefas com autenticação via **Keycloak** e **gestão de usuários** feita pela própria aplicação (registro e listagem via Keycloak Admin REST API), + front-end em **Vue 3 + PrimeVue** com layout de dashboard (sidebar escura).

A autenticação vem do **SSO compartilhado em [`../sso`](../sso)** (Keycloak, realm `studies`) — o
mesmo usado pelo [KShop](../KShop), então uma conta vale nos dois. Este projeto não sobe Keycloak
próprio; veja o [README do sso](../sso/README.md) para a arquitetura e como plugar novos projetos.

## Estrutura

```
TodoForSso/
├── TodoForSso.slnx
├── docker-compose.yml
├── src/
│   ├── TodoForSso.TodoApi/                  # Controllers, Program.cs, Extensions, Dockerfile
│   ├── TodoForSso.TodoApi.Domain/           # TodoItem, enums, interfaces de repositório
│   ├── TodoForSso.TodoApi.Application/      # DTOs, Mapster, TodoService, IUserAdminService
│   └── TodoForSso.TodoApi.Infrastructure/   # AppDbContext (SQLite), Migrations, KeycloakAdminService
└── frontend/                                 # Vue 3 + Vite + TS + PrimeVue (Aura) + Tailwind + Pinia
```

Camadas seguem Clean Architecture: `Domain` → nada; `Application` → `Domain`; `Infrastructure` → `Domain` + `Application`; `Api` → `Application` + `Infrastructure`.

- Autenticação: [Keycloak.AuthServices.Authentication](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet) (`AddKeycloakWebApiAuthentication`) valida o JWT contra o Keycloak, sem chave simétrica local. [Keycloak.AuthServices.Authorization](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet) mapeia as roles de `realm_access.roles` do token para claims de role, o que permite usar `[Authorize(Roles = "admin")]` normalmente nos controllers.
- Usuários **não** ficam no banco local — moram inteiramente no Keycloak. `TodoForSso.TodoApi.Infrastructure/Keycloak/KeycloakAdminService.cs` usa o cliente [Keycloak.Net.Core](https://github.com/Codecontrol-Oy/Keycloak.Net.Core) para falar com o Admin REST API do Keycloak (cria/lista/ativa-desativa usuários e atribui a role `user`), autenticando como admin do realm master via `KeycloakAdminTokenProvider`.
- Tarefas ficam em SQLite, sempre filtradas pelo `sub` (claim do usuário logado) — cada usuário só vê suas próprias tarefas.
- Migrations são aplicadas automaticamente no startup (`Database.Migrate()`).

## Como rodar

### Tudo via Docker (recomendado)

O SSO compartilhado precisa estar de pé antes:

```bash
cd ../sso && docker compose up -d   # Keycloak em :8082, realm studies
cd ../TodoForSso && docker compose up --build
```

Sobe:
- **API** em `http://localhost:5300` (Scalar em `/scalar/v1`)
- **Frontend** em `http://localhost:5173`

Usuários seed (do realm compartilhado):
| Usuário | Senha | Role |
|---|---|---|
| `testuser` | `testuser123` | `user` + `customer` |
| `admin` | `admin123` | `admin` + `user` + `customer` |

### Local (sem Docker)

```bash
# Backend — precisa do SSO rodando (cd ../sso && docker compose up -d)
cd src/TodoForSso.TodoApi
dotnet run

# Frontend
cd frontend
npm install   # primeira vez
npm run dev
```

As configs de Keycloak para rodar localmente já estão em `appsettings.Development.json` apontando para `http://localhost:8082`.

## Endpoints principais

- `POST /api/users/register` — público, cria usuário no Keycloak com role `user`.
- `GET /api/users` / `PATCH /api/users/{id}/status` — apenas `admin`.
- `GET|POST /api/todos`, `GET|PUT|DELETE /api/todos/{id}`, `PATCH /api/todos/{id}/complete`, `PATCH /api/todos/{id}/reopen` — autenticado, escopado ao usuário logado.

## Login

Três caminhos na tela de login, e a diferença entre eles importa:

- **Form de usuário/senha** — direct grant, resolvido na própria tela. Não cria sessão SSO de
  navegador, então entrar por aqui **não** te deixa logado no KShop.
- **Entrar com SSO** / **Entrar com Google** — redirecionam para o Keycloak (authorization code
  + PKCE). Criam a sessão SSO real: depois disso, o KShop entra direto, sem pedir credencial.

O porquê dessa assimetria está no [README do sso](../sso/README.md).
