# C# Studies

Monorepo com meus estudos de C# / .NET. Cada pasta é um projeto independente.

## Projetos

| Pasta | Descrição |
|---|---|
| [BankProject](BankProject) | Simulador de contas bancárias (herança, polimorfismo, tratamento de exceções). |
| [PayrollProject](PayrollProject) | Folha de pagamento com tipos de funcionário (herança, polimorfismo em `CalculatePayment`). |
| [ProductRegistrationProject](ProductRegistrationProject) | Registro de produtos (nacional, importado, usado) com herança e `ToString` polimórfico. |
| [BookingProject](BookingProject) | Reservas de quarto com validação de datas via exceção de domínio (`DomainException`). |
| [ToDoProject](ToDoProject) | Lista de tarefas (adicionar, concluir, remover, listar por prioridade) com enums, `DomainException` e persistência em JSON entre sessões. |
| [SalesReportProject](SalesReportProject) | Cadastro de vendas e relatórios (total por vendedor, média mensal, top 3, período) com LINQ e lambdas. |
| [PlayersRankingProject](PlayersRankingProject) | Ranking de jogadores com `IComparable`/`IComparer` e unicidade via `HashSet` (`Equals`/`GetHashCode`). |
| [CatalogApi](CatalogApi) | API REST (ASP.NET Core / .NET 10) de catálogo de produtos e categorias: EF Core + PostgreSQL, autenticação JWT, _roles_ com Identity, paginação e Docker Compose. |
| [LibraryApi](LibraryApi) | API REST (ASP.NET Core / .NET 10) de biblioteca: livros, usuários e empréstimos com EF Core + PostgreSQL, JWT, autorização por papéis, Repository/Unit of Work e Docker Compose. |
| [BankLedgerApi](BankLedgerApi) | API REST (ASP.NET Core / .NET 10) de razão bancário: clientes, contas, depósitos, transferências e extrato com EF Core + SQLite, JWT (user-secrets), rate limiting, CORS e mappings. |
| [ShopMicroservicesDemo](ShopMicroservicesDemo) | Dois microsserviços .NET 10 em Clean Architecture (UserService via gRPC, OrderService via REST + cliente gRPC) e frontend em Vue 3 + Vite/TS consumindo a API. |

## Como rodar um projeto

A maioria são aplicações de console:

```bash
cd <PastaDoProjeto>
dotnet run
```

Os projetos de API web têm passos próprios (banco, migrations, segredos, Docker) — veja o README de cada um: [CatalogApi](CatalogApi/README.md), [LibraryApi](LibraryApi/README.md), [BankLedgerApi](BankLedgerApi/README.md).
