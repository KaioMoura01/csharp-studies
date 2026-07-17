# BookingProject

Aplicação de console em C# (.NET 10) que registra reservas de quarto com validação de datas via exceção de domínio personalizada (`DomainException`).

## Estrutura

```
BookingProject/
└── BookingProject/
    ├── Program.cs                     # Menu interativo com lista de reservas
    ├── Domain/
    │   └── DomainException.cs         # Exceção de domínio personalizada
    ├── Entities/
    │   └── Reservation.cs             # Reserva (quarto, check-in, check-out)
    └── Misc/
        └── Utils.cs                   # Leitura/validação do console (texto, números, data)
```

## Classes

| Classe | Papel |
|---|---|
| `Reservation` | Guarda quarto, check-in e check-out. Valida as datas no construtor e calcula a duração (`Duration()`). |
| `DomainException` | Exceção lançada quando uma regra de negócio é violada (datas inválidas). |
| `Utils` | Helpers estáticos de leitura do console com validação em loop. |

## Regras de validação

- Check-out precisa ser depois do check-in.
- Check-in precisa ser uma data futura.

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd BookingProject
dotnet run
```

## Uso

Menu interativo:

```
1 - Add reservation
2 - List reservations
0 - Exit
```

As reservas ficam numa lista em memória; a opção 2 lista todas. Datas inválidas ou regras violadas são capturadas com `try/catch` e exibidas como erro de validação, sem quebrar a aplicação.
