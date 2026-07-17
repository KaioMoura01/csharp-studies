# PayrollProject

Aplicação de console em C# (.NET 10) que calcula a folha de pagamento de diferentes tipos de funcionário, usando herança e polimorfismo (`CalculatePayment` sobrescrito em cada subclasse).

## Estrutura

```
PayrollProject/
└── PayrollProject/
    ├── Program.cs                     # Menu interativo
    └── Entities/
        ├── Employee.cs                # Classe abstrata base
        ├── SalariedEmployee.cs        # Salário fixo
        ├── HourlyEmployee.cs          # Pagamento por hora trabalhada
        └── Freelancer.cs              # Pagamento por entrega (deliverable)
```

## Classes

| Classe | `CalculatePayment()` |
|---|---|
| `Employee` (abstrata) | Retorna o salário base. |
| `SalariedEmployee` | Herda direto: salário fixo. |
| `HourlyEmployee` | `salário/hora × horas trabalhadas`. |
| `Freelancer` | `valor por entrega × nº de entregas`. |

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd PayrollProject
dotnet run
```

## Uso

Menu interativo:

```
1 - Add salaried employee
2 - Add hourly employee
3 - Add freelancer
4 - List employees and payments
0 - Exit
```

Você cadastra vários funcionários e a opção 4 lista todos com o pagamento calculado. Entradas inválidas são tratadas com `try/catch` e não quebram a aplicação.
