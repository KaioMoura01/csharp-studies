# Shop Microservices Demo

Exemplo de arquitetura de microsserviços com **backend em .NET (Clean Architecture)** e **frontend em Vue 3**.

Dois microsserviços .NET independentes, cada um com sua própria solução e suas próprias camadas, mais uma SPA em Vue que consome a API pública:

```
ShopMicroservicesDemo/
├── proto/
│   └── user.proto              # contrato gRPC compartilhado entre os dois serviços
├── services/
│   ├── UserService/             # dono dos dados de usuário — só expõe gRPC (uso interno)
│   │   ├── UserService.Domain
│   │   ├── UserService.Application
│   │   ├── UserService.Infrastructure
│   │   └── UserService.Api
│   └── OrderService/            # expõe REST para o frontend; é cliente gRPC do UserService
│       ├── OrderService.Domain
│       ├── OrderService.Application
│       ├── OrderService.Infrastructure
│       └── OrderService.Api
└── frontend/                    # Vue 3 + Vite + TypeScript
```

## Por que dois serviços diferentes

- **UserService**: só é chamado por outros serviços do backend. Faz sentido expor via **gRPC** (contrato tipado, binário, rápido).
- **OrderService**: é chamado pelo navegador. Expõe **REST/JSON** (o que qualquer frontend consome nativamente) e, internamente, é **cliente gRPC** do UserService para validar o usuário antes de criar um pedido.

Isso reproduz um padrão comum: serviços internos falam gRPC entre si; um serviço de borda ("BFF"/gateway) expõe REST para o cliente externo.

## Clean Architecture em cada serviço

Cada microsserviço é dividido em 4 projetos com dependência apontando sempre para dentro:

```
Api  →  Infrastructure  →  Application  →  Domain
```

- **Domain**: entidades puras (`User`, `Order`), sem nenhuma dependência externa.
- **Application**: casos de uso (`GetUserHandler`, `CreateOrderHandler`) e **interfaces** (`IUserRepository`, `IUserServiceClient`) — as portas que a camada externa precisa implementar. Não sabe o que é gRPC, EF Core ou HTTP.
- **Infrastructure**: implementa as portas da Application. É aqui que mora o `UserServiceGrpcClient` (implementa `IUserServiceClient` usando gRPC) e os repositórios em memória.
- **Api**: camada de apresentação — controllers REST (OrderService) ou o serviço gRPC (UserService) — e a composição da injeção de dependência (`Program.cs`).

O ponto central do Clean Architecture aparece em `OrderService.Application/Abstractions/IUserServiceClient.cs`: a camada de aplicação do OrderService define *o que* precisa (buscar um usuário), sem saber *como* isso é feito. Só a Infrastructure sabe que por baixo é uma chamada gRPC — se amanhã trocasse para REST ou uma fila, só a Infrastructure mudaria.

## Rodando o backend

```bash
cd services/UserService/UserService.Api
dotnet run --launch-profile http    # sobe em http://localhost:5229 (gRPC)
```

```bash
cd services/OrderService/OrderService.Api
dotnet run --launch-profile http    # sobe em http://localhost:5017 (REST)
```

O endereço do UserService usado pelo cliente gRPC está em `OrderService.Api/appsettings.json` (`UserService:Address`).

### Endpoints do OrderService (REST)

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/users` | Lista usuários (proxy REST → gRPC para o UserService) |
| GET | `/api/orders` | Lista pedidos criados |
| POST | `/api/orders` | Cria um pedido — valida o usuário via gRPC antes de aceitar |

## Rodando o frontend

```bash
cd frontend
npm install
npm run dev    # http://localhost:5173
```

O frontend consome `http://localhost:5017` (configurável via `VITE_ORDER_SERVICE_URL`). O OrderService já libera CORS para `http://localhost:5173`.

## Fluxo de uma criação de pedido

1. Usuário preenche o formulário no Vue e envia `POST /api/orders`.
2. `OrdersController` (OrderService.Api) chama `CreateOrderHandler` (Application).
3. `CreateOrderHandler` pede o usuário via `IUserServiceClient.GetUserAsync` — implementado por `UserServiceGrpcClient` (Infrastructure), que faz a chamada gRPC real para o UserService.
4. UserService resolve a query no seu próprio `GetUserHandler`, contra o repositório em memória.
5. Se o usuário existe e está ativo, o pedido é criado (`status: Created`); caso contrário, é rejeitado (`status: Rejected`) — mas sempre com HTTP 200/422, o motivo vai no campo `message`.

## Limitações intencionais (é um exemplo de estudo)

- Repositórios em memória (`ConcurrentDictionary`/`ConcurrentBag`) — sem banco de dados, dados somem ao reiniciar.
- Sem autenticação/autorização.
- Comunicação gRPC sem TLS (`http://`, `Http2UnencryptedSupport`) — em produção use HTTPS.
- Endereço do UserService fixo em configuração — em produção normalmente viria de service discovery.
