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

## Como rodar um projeto

A maioria são aplicações de console:

```bash
cd <PastaDoProjeto>
dotnet run
```

O [CatalogApi](CatalogApi) é uma API web e tem passos próprios (banco, migrations, Docker) — veja o [README dele](CatalogApi/README.md).
