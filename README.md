# C# Studies

Monorepo com meus estudos de C# / .NET. Cada pasta é um projeto independente.

## Projetos

| Pasta | Descrição |
|---|---|
| [BankProject](BankProject) | Simulador de contas bancárias (herança, polimorfismo, tratamento de exceções). |
| [PayrollProject](PayrollProject) | Folha de pagamento com tipos de funcionário (herança, polimorfismo em `CalculatePayment`). |
| [ProductRegistrationProject](ProductRegistrationProject) | Registro de produtos (nacional, importado, usado) com herança e `ToString` polimórfico. |
| [BookingProject](BookingProject) | Reservas de quarto com validação de datas via exceção de domínio (`DomainException`). |
| [ToDoProject](ToDoProject) | Lista de tarefas (adicionar, concluir, remover, listar por prioridade) com enums e `DomainException`. |
| [SalesReportProject](SalesReportProject) | Cadastro de vendas e relatórios (total por vendedor, média mensal, top 3, período) com LINQ e lambdas. |

## Como rodar um projeto

```bash
cd <PastaDoProjeto>
dotnet run
```
