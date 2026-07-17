# BankProject

Aplicação de console em C# (.NET 10) que simula operações bancárias básicas — criação de contas, depósitos, saques e consulta de saldo — usando herança e polimorfismo.

## Estrutura do projeto

```
BankProject/
├── BankProject.sln
└── BankProject/
    ├── Program.cs                  # Menu interativo da aplicação
    ├── Entities/
    │   ├── Account.cs              # Classe abstrata base
    │   ├── SavingAccount.cs        # Conta poupança (sem taxa de saque)
    │   └── BusinessAccount.cs      # Conta empresarial (taxa de saque maior)
    └── Misc/
        └── Utils.cs                # Helpers de leitura/validação do console
```

## Classes

### `Account` (abstrata)

Base de todas as contas. Guarda o titular e o saldo, e define as operações comuns:

| Membro | Descrição |
|---|---|
| `BankTax` | Taxa cobrada por saque (virtual, padrão `5.0`) |
| `Deposit(double)` | Deposita um valor (rejeita valores ≤ 0 com `ArgumentException`) |
| `Withdraw(double)` | Saca um valor descontando a taxa (rejeita saldo insuficiente com `ArgumentException`) |
| `GetBalance()` | Imprime o saldo atual no console |

### `SavingAccount`

Conta poupança. Herda de `Account`, tem taxa de saque **zero** e uma taxa de juros (`InterestRate`).

### `BusinessAccount`

Conta empresarial. Herda de `Account` com taxa de saque de **10.0** por operação.

### `Utils`

Métodos estáticos de leitura do console com validação em loop (`ReadConsoleString`, `ReadConsoleDouble`, `ReadConsoleInt`, `ReadConsoleDate`) e formatação de valores (`FormatMoney`).

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd BankProject
dotnet run --project BankProject
```

## Uso

A aplicação apresenta um menu interativo:

```
1 - Criar conta poupança
2 - Criar conta empresarial
3 - Depositar
4 - Sacar
5 - Consultar saldo
0 - Sair
```

Crie uma conta (opção 1 ou 2) antes de operar. Erros de operação (valor inválido, saldo insuficiente, nenhuma conta criada) são tratados com `try/catch` e exibidos como mensagens amigáveis — a aplicação nunca quebra por entrada inválida.
