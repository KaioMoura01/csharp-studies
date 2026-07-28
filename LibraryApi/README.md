# LibraryApi

API REST (ASP.NET Core / .NET 10) de uma **biblioteca**: cadastro de livros, usuários e empréstimos (_loans_) com devolução e controle de atraso. Autenticação por **JWT** e **autorização por papéis** (`Admin`, `Librarian`, `User`).

## Stack

- **.NET 10** / ASP.NET Core (controllers)
- **EF Core** + **PostgreSQL** (Npgsql)
- **JWT Bearer** com autorização por _role_ (claims)
- **Scalar** para documentação da API (OpenAPI)
- **Docker Compose**: API + PostgreSQL + pgAdmin
- `PasswordHasher<T>` do ASP.NET Identity para hash de senha

## Conceitos exercitados

- **Repository + Unit of Work**: `IRepository<T>`, repositórios especializados (`IBook`, `ILoan`, `IUser`) e `IUnitOfWork` coordenando o `SaveChanges`.
- **Autorização por papel** com `[Authorize(Roles = "...")]` e `[AllowAnonymous]` por endpoint.
- **Mapping** via _extension methods_ (`ToEntity`/`ToResponse`) separando Model de DTO.
- **Regras de domínio**: empréstimo com prazo de 14 dias, `Returned` calculado a partir de `ReturnDate`, baixa de `Stock` no livro.
- **Migrations** versionando o schema no PostgreSQL.

## Arquitetura

```
Controllers  ->  Repository / UnitOfWork  ->  AppDbContext (EF Core / PostgreSQL)
                     |
                     +-- Extensions (Model <-> DTO)
TokenService     geração do JWT
Models/          Book, User, Loan
```

## Papéis

| Papel | Pode |
|---|---|
| `User` | listar/ver livros, se cadastrar, logar, ver os próprios dados |
| `Librarian` | tudo do User + criar/editar livros, gerenciar empréstimos |
| `Admin` | tudo + apagar livros, criar bibliotecários, promover papéis |

O cadastro público (`POST /users`) sempre cria um `User`. Bibliotecário e Admin só são criados/promovidos por um Admin — por isso o **primeiro Admin** precisa ser semeado (veja abaixo).

## Como rodar

Pré-requisitos: **.NET 10 SDK** e **Docker** (para o PostgreSQL/pgAdmin).

### Opção A — tudo via Docker

```bash
cd LibraryApi
docker compose up -d          # sobe api (8080), postgres (5432) e pgadmin (5050)
dotnet ef database update --project LibraryApi   # aplica as migrations no postgres
```

- API: **http://localhost:8080/scalar/v1**
- pgAdmin: **http://localhost:5050** (login `admin@admin.com` / `admin`, servidor já pré-configurado)

### Opção B — banco no Docker, API local

```bash
cd LibraryApi
docker compose up -d postgres
dotnet ef database update --project LibraryApi
dotnet run --project LibraryApi --launch-profile http
```

- API: **http://localhost:5230/scalar/v1**

> As migrations **não** são aplicadas automaticamente no startup — rode `dotnet ef database update` antes do primeiro uso.

### Semear o primeiro Admin

Não há endpoint para criar o primeiro Admin. Cadastre um usuário normal (`POST /users`) e promova-o direto no banco (via pgAdmin ou SQL):

```sql
UPDATE "User" SET "Role" = 0 WHERE "Email" = 'seu-email@exemplo.com';
```

(`Role`: `0 = Admin`, `1 = Librarian`, `2 = User`.)

## Endpoints

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/users` | — | Cadastro (cria um `User`). |
| POST | `/users/login` | — | Autentica e devolve o JWT. |
| GET | `/users` | Admin, Librarian | Lista usuários. |
| GET | `/users/{id}` | JWT | Dados de um usuário. |
| POST | `/users/librarians` | Admin | Cria um bibliotecário. |
| GET | `/books` | — | Lista livros. |
| GET | `/books/{id}` | — | Detalhe de um livro. |
| POST | `/books` | Admin, Librarian | Cadastra um livro. |
| PUT | `/books/{id}` | Admin, Librarian | Edita um livro. |
| DELETE | `/books/{id}` | Admin | Remove um livro. |
| GET | `/loans` | Admin, Librarian | Lista empréstimos. |
| GET | `/loans/overdue` | Admin, Librarian | Empréstimos em atraso. |
| GET | `/loans/{id}` | Admin, Librarian | Detalhe de um empréstimo. |
| POST | `/loans` | Admin, Librarian | Registra um empréstimo. |
| POST | `/loans/{id}/return` | Admin, Librarian | Registra a devolução. |
| GET | `/admin/roles` | Admin | Lista papéis. |
| PUT | `/admin/roles/{userId}` | Admin | Altera o papel de um usuário. |

## Observações

- A config de **JWT** (`SecretKey`, `Issuer`, `Audience`, `TokenValidityInMinutes`) está no `appsettings.json` com um segredo de desenvolvimento. Em produção, mova o `SecretKey` para user-secrets/variável de ambiente.
- O pgAdmin já vem com o servidor cadastrado via `pgadmin/servers.json`.
