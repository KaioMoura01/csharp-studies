# KShop

Monorepo com múltiplas APIs (cada uma em Clean Architecture) e um frontend Vue.

## Estrutura

```
KShop/
├── KShop.slnx              # solution única, com todos os serviços agrupados em solution folders
├── docker-compose.yml       # sobe todos os serviços de uma vez
├── src/
│   └── Product/             # serviço "Product" (o nome da pasta = nome do bounded context)
│       ├── KShop.ProductApi                 # camada Api: Controllers, Program.cs, DI
│       ├── KShop.ProductApi.Domain          # Models + interfaces de Repository/UnitOfWork (sem dependências)
│       ├── KShop.ProductApi.Application     # DTOs, Mappings (Mapster), Services
│       └── KShop.ProductApi.Infrastructure  # AppDbContext, Migrations, Repositories concretas
└── frontend/                 # SPA Vue 3 + TS + Tailwind + PrimeVue, consome as APIs direto
```

Cada camada só depende da que está "abaixo": `Domain` não referencia nada; `Application` só `Domain`; `Infrastructure` referencia `Domain` + `Application`; a camada `Api` amarra tudo.

## Como adicionar um novo serviço

Exemplo criando o serviço `Order` (repita o padrão do `Product`):

```bash
mkdir -p src/Order
cd src/Order

dotnet new classlib -n KShop.OrderApi.Domain -o KShop.OrderApi.Domain
dotnet new classlib -n KShop.OrderApi.Application -o KShop.OrderApi.Application
dotnet new classlib -n KShop.OrderApi.Infrastructure -o KShop.OrderApi.Infrastructure
dotnet new webapi -n KShop.OrderApi -o KShop.OrderApi

dotnet add KShop.OrderApi.Application reference KShop.OrderApi.Domain
dotnet add KShop.OrderApi.Infrastructure reference KShop.OrderApi.Domain
dotnet add KShop.OrderApi.Infrastructure reference KShop.OrderApi.Application
dotnet add KShop.OrderApi reference KShop.OrderApi.Application
dotnet add KShop.OrderApi reference KShop.OrderApi.Infrastructure

cd ../..
dotnet sln add src/Order/KShop.OrderApi/KShop.OrderApi.csproj --solution-folder src/Order
dotnet sln add src/Order/KShop.OrderApi.Domain/KShop.OrderApi.Domain.csproj --solution-folder src/Order
dotnet sln add src/Order/KShop.OrderApi.Application/KShop.OrderApi.Application.csproj --solution-folder src/Order
dotnet sln add src/Order/KShop.OrderApi.Infrastructure/KShop.OrderApi.Infrastructure.csproj --solution-folder src/Order
```

Depois:
1. Criar `src/Order/KShop.OrderApi/Dockerfile` (copiar e adaptar o de `src/Product/KShop.ProductApi/Dockerfile`, trocando os nomes dos projetos).
2. Descomentar/adaptar o bloco `order-api` no `docker-compose.yml` na raiz.
3. Registrar as camadas Application/Infrastructure via `AddApplication()`/`AddInfrastructure()` (mesmo padrão do `ProductApi`).

## Como rodar

**Local (sem Docker), por serviço:**
```bash
cd src/Product/KShop.ProductApi
dotnet run
```

**Frontend:**
```bash
cd frontend
npm install   # primeira vez
npm run dev
```

**Tudo junto via Docker:**
```bash
docker compose up --build
```
