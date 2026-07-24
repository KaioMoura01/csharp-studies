# CatalogApi

API REST em ASP.NET Core (.NET 10) para um catálogo de **produtos** e **categorias**, com autenticação JWT, controle de acesso por _roles_ (Identity), persistência em PostgreSQL via Entity Framework Core e documentação interativa com Scalar/OpenAPI.

É o projeto mais completo do repositório de estudos: sai das aplicações de console e exercita uma API web de verdade — camadas, repositórios, Unit of Work, paginação, migrations e infraestrutura em Docker.

## Stack

| Tecnologia | Uso |
|---|---|
| .NET 10 / ASP.NET Core Web API | Framework da API |
| Entity Framework Core + Npgsql | ORM e provider PostgreSQL |
| ASP.NET Core Identity | Usuários e _roles_ |
| JWT Bearer | Autenticação via token |
| Scalar + OpenAPI | Documentação interativa (ambiente de dev) |
| Docker Compose | API + PostgreSQL + pgAdmin |

## Estrutura do projeto

```
CatalogApi/
├── CatalogApi.sln
├── compose.yaml                # API + PostgreSQL + pgAdmin
├── pgadmin/                    # Config pré-carregada do pgAdmin (servers.json, pgpass)
└── CatalogApi/
    ├── Program.cs             # Pipeline, DI e middlewares
    ├── Controllers/           # Auth, Categories, Products, Roles (admin)
    ├── Models/                # Product, Category, ApplicationUser, INamed, ErrorDetails
    ├── DTOs/                  # Request/Response DTOs, LoginModel, RegisterModel, TokenModel, Response
    ├── Context/               # CatalogApiContext (IdentityDbContext)
    ├── Repositories/          # Generic + específicos, Unit of Work
    ├── Services/              # TokenService, BuilderAuthenticationService
    ├── Extensions/            # Mapeamento de DTOs e ExceptionHandler global
    ├── Pagination/            # Parâmetros genéricos de paginação/busca/ordenação
    ├── Enums/                 # Enums de apoio (ex.: ordenação)
    ├── Migrations/            # Migrations do EF Core
    └── Dockerfile
```

## Arquitetura

- **Repository + Unit of Work** — `IUnitOfWork` expõe os repositórios (`Categories`, `Products`) e centraliza o `Commit`. Há um `GenericRepository<T>` com as operações comuns e repositórios específicos para regras próprias de cada entidade.
- **DTOs** — entrada e saída são mapeadas para DTOs (via `Extensions/DtoMappingExtensions`), evitando expor as entidades e ciclos de serialização.
- **Paginação** — endpoints de listagem aceitam `GenericParameters` (`Page`, `PageSize`, `Search`, `OrderByName`).
- **Autenticação/Autorização** — Identity com `ApplicationUser`; emissão de JWT + refresh token no `TokenService`; parâmetros de validação centralizados em `BuilderAuthenticationService`.
- **Tratamento de erros** — middleware global (`ConfigureExceptionHandler`) padroniza respostas de erro com `ErrorDetails`.

## Endpoints

### Auth — `/auth`
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| POST | `/auth/register` | público | Cria um novo usuário |
| POST | `/auth/login` | público | Autentica e retorna JWT + refresh token |
| POST | `/auth/refresh-token` | público | Gera novo access token a partir do refresh token |
| POST | `/auth/revoke/{userName}` | autenticado | Revoga o refresh token de um usuário |

### Categories — `/categories`
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/categories` | público | Lista categorias (paginado) |
| GET | `/categories/{id}` | público | Busca categoria por id |
| POST | `/categories` | autenticado | Cria categoria |
| PUT | `/categories/{id}` | roles `Admin`, `Financial` | Atualiza categoria |
| DELETE | `/categories/{id}` | autenticado | Remove categoria |

### Products — `/products`
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| GET | `/products` | público | Lista produtos (paginado) |
| GET | `/products/{id}` | público | Busca produto por id |
| GET | `/products/category/{categoryId}` | público | Lista produtos de uma categoria |
| POST | `/products` | público | Cria produto |
| PUT | `/products/{id}` | autenticado | Atualiza produto |
| DELETE | `/products/{id}` | autenticado | Remove produto |

### Admin (Roles) — `/admin` · requer role `Admin`
| Método | Rota | Descrição |
|---|---|---|
| POST | `/admin/roles` | Cria uma role |
| GET | `/admin/roles` | Lista roles |
| GET | `/admin/roles/{roleName}` | Busca role por nome |
| PUT | `/admin/roles` | Atualiza uma role |
| DELETE | `/admin/roles` | Remove uma role |
| POST | `/admin/add-role-to-user` | Associa uma role a um usuário |

## Configuração

As chaves relevantes ficam em `appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=catalog;Username=postgres;Password=postgres"
  },
  "JWT": {
    "ValidAudience": "http://localhost:7066",
    "ValidIssuer": "http://localhost:5066",
    "SecretKey": "…",                       // use user-secrets fora de estudo
    "TokenValidityInMinutes": 30,
    "RefreshTokenValidityInMinutes": 60
  }
}
```

> O `SecretKey` está versionado apenas por ser um projeto de estudo. Em qualquer cenário real, mova-o para `dotnet user-secrets` ou variáveis de ambiente.

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

### Opção 1 — Docker Compose (API + banco + pgAdmin)

Sobe tudo já conectado:

```bash
cd CatalogApi
docker compose up --build
```

| Serviço | URL / porta |
|---|---|
| API | http://localhost:8080 |
| PostgreSQL | localhost:5432 (`postgres` / `postgres`, db `catalog`) |
| pgAdmin | http://localhost:5050 (`admin@admin.com` / `admin`) |

### Opção 2 — Local (só a API)

Precisa de um PostgreSQL acessível conforme a `DefaultConnection`. Aplique as migrations e rode:

```bash
cd CatalogApi
dotnet ef database update --project CatalogApi
dotnet run --project CatalogApi
```

A API sobe em `http://localhost:5278`. Em ambiente de desenvolvimento, a documentação Scalar fica disponível em `http://localhost:5278/scalar/v1`.

## Fluxo de autenticação

1. `POST /auth/register` para criar o usuário.
2. `POST /auth/login` para obter o `Token` (JWT) e o `RefreshToken`.
3. Envie o JWT nas rotas protegidas no header:
   ```
   Authorization: Bearer <token>
   ```
4. Quando o access token expirar, use `POST /auth/refresh-token` para renovar.

Rotas com `[Authorize(Roles = "…")]` exigem que o usuário tenha a role correspondente — crie e associe roles pelos endpoints em `/admin` (que por sua vez exigem a role `Admin`).
