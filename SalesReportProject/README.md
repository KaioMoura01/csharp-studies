# SalesReportProject

Aplicação de console em C# (.NET 10) que cadastra vendas e gera relatórios usando **LINQ** e **expressões lambda**.

## Estrutura

```
SalesReportProject/
└── SalesReportProject/
    ├── Program.cs                     # Menu interativo + relatórios (LINQ/lambda)
    ├── Domain/
    │   └── DomainException.cs         # Exceção de domínio personalizada
    ├── Misc/
    │   └── Utils.cs                   # Leitura/validação do console
    └── Entities/
        ├── Seller.cs                  # Vendedor (id, nome)
        ├── Product.cs                 # Produto (nome, preço, quantidade) com validação
        └── Sell.cs                    # Venda (id, data, vendedor, lista de produtos)
```

## Funcionalidades

| Opção | Ação |
|---|---|
| 1 | Cadastrar venda (vendedor, data, e um ou mais produtos com valor e quantidade) |
| 2 | Relatório: **total por vendedor** |
| 3 | Relatório: **média mensal** (total por mês + média) |
| 4 | Relatório: **top 3 produtos** (por receita) |
| 5 | Relatório: **filtro por período** (intervalo de datas) |
| 6 | Listar todas as vendas |
| 0 | Sair |

## Técnicas usadas

Os relatórios são construídos com LINQ e lambdas — `GroupBy`, `Select`, `Sum`, `OrderByDescending`, `Take`, `Where`, `SelectMany`, `Average`. O total de cada venda é calculado como `preço × quantidade` de cada produto.

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd SalesReportProject
dotnet run
```

## Uso

Os vendedores são reutilizados por nome (mesmo nome = mesmo vendedor), para que o agrupamento dos relatórios funcione corretamente. Entradas inválidas e regras de negócio (nome de produto curto, valor/quantidade ≤ 0) são tratadas com `try/catch` e exibidas como erro de validação. Toda a interface é em inglês.
