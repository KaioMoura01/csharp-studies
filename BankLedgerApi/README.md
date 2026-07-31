# BankLedgerApi

API REST (ASP.NET Core / .NET 10) de um **razão bancário** (_ledger_): clientes, contas, depósitos, transferências entre contas e extrato por período. Autenticação por **JWT** (login na conta), com o saldo derivando de lançamentos.

## Stack

- **.NET 10** / ASP.NET Core (controllers)
- **EF Core** + **SQLite** (arquivo `ledger.db`)
- **JWT Bearer** para autorização
- **Rate limiting** nativo (janela fixa) e **CORS**
- **Scalar** para documentação da API (OpenAPI)
- `PasswordHasher<T>` do ASP.NET Identity para hash de senha

## Conceitos exercitados

- **DTOs por recurso** + **mappings** via _extension methods_ (sem AutoMapper), isolando a montagem de resposta das _services_.
- **Value Object** `TaxDocument` (CPF/CNPJ) como _owned type_ do EF Core, com validação no construtor.
- **Segredos fora do código**: `Jwt` e `RateLimiting` em **user-secrets** (dev), nunca no `appsettings.json`.
- **Transação de banco** na transferência (débito + crédito atômicos).
- **Ledger**: o saldo é a soma dos lançamentos — depósito e transferência geram linhas na tabela `Transfers` (depósito tem origem nula e aparece como `Deposit` no extrato).

## Arquitetura

```
Controllers  ->  Services (regra de negócio)  ->  AppDbContext (EF Core)
                     |
                     +-- Mappings (Model -> DTO)
DTOs/        request e response por recurso
Models/      Customer, Account, Transfer, TaxDocument (value object)
```

## Como rodar

Pré-requisitos: **.NET 10 SDK**.

### 1. Configurar os segredos (obrigatório)

A app lê `Jwt` e `RateLimiting` do **user-secrets** — sem isso ela **não sobe**.

```bash
cd BankLedgerApi
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)"
dotnet user-secrets set "Jwt:Issuer" "BankLedgerApi"
dotnet user-secrets set "Jwt:Audience" "BankLedgerApi"
dotnet user-secrets set "Jwt:ExpiryMinutes" "60"
dotnet user-secrets set "RateLimiting:PermitLimit" "2"
dotnet user-secrets set "RateLimiting:WindowSeconds" "5"
dotnet user-secrets set "RateLimiting:QueueLimit" "0"
```

### 2. Aplicar as migrations (cria o `ledger.db`)

```bash
dotnet ef database update
```

### 3. Rodar

```bash
dotnet run --launch-profile http
```

A documentação abre em **http://localhost:5288/scalar/v1**.

## Fluxo típico

1. `POST /customers` — cria o cliente.
2. `POST /accounts` — abre a conta (guarde o `number` e a senha).
3. `POST /auth/login` — autentica pelo `number` + senha, devolve o **token**.
4. `POST /accounts/deposit` — deposita (com o header `Authorization: Bearer <token>`).
5. `POST /transfers` — transfere para outra conta pelo **número**.
6. `GET /statements?from=...&to=...` — vê o extrato reconciliando com o saldo.

## Endpoints

| Método | Rota | Auth | Descrição |
|---|---|---|---|
| POST | `/auth/login` | — | Autentica a conta e devolve o JWT. |
| POST | `/customers` | — | Cria um cliente (nome + documento fiscal). |
| GET | `/customers/{id}` | — | Cliente + resumo das contas. |
| GET | `/customers/{id}/accounts` | — | Lista as contas do cliente. |
| POST | `/accounts` | — | Abre uma conta para um cliente. |
| GET | `/accounts/me` | JWT | Dados da conta autenticada (inclui o id do dono). |
| POST | `/accounts/deposit` | JWT | Deposita na conta autenticada. |
| GET | `/accounts/{id}` | — | Dados de uma conta por id. |
| POST | `/transfers` | JWT | Transfere para outra conta (por número). |
| POST | `/reversals` | JWT | Estorna uma transferência feita pela conta autenticada. |
| GET | `/statements` | JWT | Extrato da conta autenticada por período. |
| GET | `/health` | — | Health check (retorna `true`) para monitoramento. |

## Estorno (reversal)

O estorno é **append-only**: não apaga a transferência original — cria um **lançamento compensatório** movendo o valor de volta (destino → origem) e vinculado à original por `ReversedTransferId`. No extrato, original e estorno aparecem e se anulam, mantendo saldo e ledger reconciliados.

Regras (`POST /reversals` com `{ "transferId": "..." }`):

- Só a **conta de origem** da transferência pode estorná-la (autenticada por JWT).
- A transferência precisa estar `Completed` e **não** ter sido estornada antes (o vínculo impede estorno duplo).
- **Depósitos não são estornáveis** por esta rota.
- Falha se a conta de **destino não tiver saldo** suficiente para devolver o valor.

## Observações

- **SQLite** guarda `decimal` como texto; a precisão (`18,2`) é declarada por boas práticas/portabilidade.
- O **rate limit** é global mas isenta as rotas de documentação (`/scalar`, `/openapi`).
- O `ledger.db` é gerado e está no `.gitignore` — a fonte da verdade do schema são as **migrations**.
