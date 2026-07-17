# ToDoProject

Aplicação de console em C# (.NET 10) que gerencia uma lista de tarefas com prioridade e status, usando enums, exceção de domínio personalizada (`DomainException`) e **persistência em JSON entre sessões**.

## Estrutura

```
ToDoProject/
└── ToDoProject/
    ├── Program.cs                         # Menu interativo
    ├── Domain/
    │   └── DomainException.cs             # Exceção de domínio personalizada
    ├── Misc/
    │   └── Utils.cs                       # Leitura/validação do console
    ├── Services/
    │   └── TaskRepository.cs              # Persistência das tarefas em tasks.json
    └── Entities/
        ├── TaskItem.cs                    # Tarefa (nome, data, prioridade, status)
        └── Enums/
            ├── EnumTaskPriority.cs         # High, Medium, Low
            └── EnumConclusionTaskStatus.cs # Completed, WaitingStart, InProgress, Due
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

A opção 4 usa `GroupBy` pela prioridade para exibir as tarefas separadas por grupo. Datas inválidas ou regras de negócio violadas (data no passado, status já concluído) são capturadas com `try/catch` e exibidas como erro de validação, sem quebrar a aplicação. Toda a interface é em inglês.

## Persistência (JSON)

As tarefas **duram entre sessões**: são gravadas num arquivo `tasks.json` e recarregadas ao iniciar a aplicação.

- Ao abrir, o `Program` chama `TaskRepository.Load()` e informa quantas tarefas foram lidas.
- Após cada operação que muda o estado (**adicionar**, **concluir**, **remover**), o `Program` chama `TaskRepository.Save()`, reescrevendo o arquivo inteiro.
- O `TaskRepository` usa um **DTO interno** (em vez de serializar a entidade direto), desacoplando o formato do arquivo da classe de domínio, e um `JsonStringEnumConverter` para os enums saírem legíveis (`"High"`, `"Completed"`).
- O arquivo fica em `AppContext.BaseDirectory` (pasta do executável), por isso sobrevive entre execuções.

### Reidratação vs. criação

A `TaskItem` tem duas portas de entrada:

- **Construtor público** — valida as regras (ex.: data não pode estar no passado). Usado ao criar uma tarefa nova.
- **`TaskItem.FromStorage(...)`** — reconstrói uma tarefa salva **sem revalidar** a data. Essencial para carregar tarefas antigas cuja data de vencimento já passou (o construtor normal as rejeitaria).

Exemplo de `tasks.json`:

```json
[
  { "Name": "Buy milk", "DueDate": "2027-08-20T00:00:00", "Priority": "High", "TaskStatus": "Completed" },
  { "Name": "Read book", "DueDate": "2027-08-21T00:00:00", "Priority": "Low", "TaskStatus": "WaitingStart" }
]
```
