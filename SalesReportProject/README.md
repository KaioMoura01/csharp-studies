# SalesReportProject

Aplicação de console em C# (.NET 10) que cadastra vendas e gera relatórios usando **LINQ** e **expressões lambda**.

## Estrutura

```
SalesReportProject/
└── SalesReportProject/
    ├── Program.cs                     # Menu interativo + relatórios (LINQ/lambda)
    ├── sales.csv                      # Dados de exemplo (copiado para a pasta de saída)
    ├── Domain/
    │   └── DomainException.cs         # Exceção de domínio personalizada
    ├── Misc/
    │   └── Utils.cs                   # Leitura/validação do console
    ├── Services/
    │   └── AutomatorCSV.cs            # Lê vendas de um .csv e grava o relatório em arquivo
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
| 7 | **Importar vendas de um `.csv`** |
| 8 | **Exportar o relatório completo para um arquivo** |
| 0 | Sair |

## Importação/exportação via CSV (`AutomatorCSV`)

A classe [`AutomatorCSV`](SalesReportProject/Services/AutomatorCSV.cs) concentra a leitura do `.csv` e a gravação do relatório em arquivo:

- **`ReadSales(path)`** — lê o CSV, agrupa as linhas por `SaleId` e reconstrói as vendas (reutilizando vendedores pelo nome).
- **`WriteReport(path, content)`** — grava o texto do relatório em outro arquivo.

O fluxo pedido — **ler um `.csv`, processar com LINQ e gravar o relatório em outro arquivo** — corresponde a: opção **7** (importar `sales.csv`) → opções de relatório (LINQ) → opção **8** (gravar `sales-report.txt`).

### Formato do CSV

Uma linha por item de venda; linhas com o mesmo `SaleId` pertencem à mesma venda:

```csv
SaleId,Date,Seller,Product,Price,Quantity
1,10/01/2027,Ana,Mouse,50.00,2
1,10/01/2027,Ana,Keyboard,100.00,1
2,15/01/2027,Bob,Mouse,50.00,1
```

O arquivo `sales.csv` é copiado para a pasta de saída do build, então o caminho padrão sugerido nas opções 7 e 8 (`AppContext.BaseDirectory`) já o encontra — basta pressionar Enter.

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
