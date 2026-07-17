# ToDoProject

Aplicação de console em C# (.NET 10) que gerencia uma lista de tarefas com prioridade e status, usando enums e exceção de domínio personalizada (`DomainException`).

## Estrutura

```
ToDoProject/
└── ToDoProject/
    ├── Program.cs                        # Menu interativo
    ├── Domain/
    │   └── DomainException.cs            # Exceção de domínio personalizada
    ├── Misc/
    │   └── Utils.cs                      # Leitura/validação do console
    └── Entities/
        ├── Task.cs                       # Tarefa (nome, data, prioridade, status)
        └── Enums/
            ├── EnumTaskPriority.cs        # High, Medium, Low
            └── EnumConclusionTaskStatus.cs# Completed, WaitingStart, InProgress, Due
```

## Funcionalidades

| Opção | Ação |
|---|---|
| 1 | Adicionar tarefa (nome, data de vencimento, prioridade) |
| 2 | Concluir tarefa (marca como `Completed`) |
| 3 | Remover tarefa |
| 4 | Listar tarefas **agrupadas por prioridade** (High → Medium → Low) |
| 0 | Sair |

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd ToDoProject
dotnet run
```

## Uso

As tarefas ficam numa lista em memória. A opção 4 usa `GroupBy` pela prioridade para exibir as tarefas separadas por grupo. Datas inválidas ou regras de negócio violadas (data no passado, status já concluído) são capturadas com `try/catch` e exibidas como erro de validação, sem quebrar a aplicação. Toda a interface é em inglês.
