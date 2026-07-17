# ProductRegistrationProject

Aplicação de console em C# (.NET 10) que registra diferentes tipos de produto usando herança e polimorfismo (`ToString` sobrescrito em cada subclasse).

## Estrutura

```
ProductRegistrationProject/
└── ProductRegistrationProject/
    ├── Program.cs                     # Menu interativo
    ├── Misc/
    │   └── Utils.cs                   # Leitura/validação do console (números, texto, data)
    └── Entities/
        ├── Product.cs                 # Classe abstrata base
        ├── NationalProduct.cs         # Produto nacional
        ├── ImportedProduct.cs         # Produto importado (com taxa de importação)
        └── UsedProduct.cs             # Produto usado (com data de fabricação)
```

## Classes

| Classe | Extras |
|---|---|
| `Product` (abstrata) | Nome e preço. |
| `NationalProduct` | Sem campos extras. |
| `ImportedProduct` | `ImportDuty` (taxa de importação). |
| `UsedProduct` | `FabricationDate` (data de fabricação). |

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd ProductRegistrationProject
dotnet run
```

## Uso

Menu interativo:

```
1 - Add national product
2 - Add imported product
3 - Add used product
4 - List products
0 - Exit
```

Cadastre vários produtos e a opção 4 lista todos. Entradas inválidas (número, data no formato `dd/MM/yyyy`, valores ≤ 0) são tratadas e repetidas; erros inesperados são capturados com `try/catch`.
