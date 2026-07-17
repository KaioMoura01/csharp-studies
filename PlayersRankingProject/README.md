# PlayersRankingProject

Aplicação de console em C# (.NET 10) que mantém um ranking de jogadores, explorando **ordenação** (`IComparable`/`IComparer`) e **unicidade** com `HashSet` (`Equals`/`GetHashCode`).

## Estrutura

```
PlayersRankingProject/
└── PlayersRankingProject/
    ├── Program.cs                     # Menu interativo (HashSet de jogadores)
    ├── Domain/
    │   └── DomainException.cs         # Exceção de domínio personalizada
    ├── Misc/
    │   └── Utils.cs                   # Leitura/validação do console
    └── Entities/
        ├── Player.cs                  # Jogador (nome, pontos, data) — IComparable
        └── Comparers/
            ├── ByNameComparer.cs      # IComparer: ordena por nome
            └── ByDateComparer.cs      # IComparer: ordena por data de registro
```

## Funcionalidades

| Opção | Ação |
|---|---|
| 1 | Adicionar jogador (nome, pontos, data) — duplicados são rejeitados |
| 2 | Ranking **por pontos** (ordenação natural, maior primeiro) |
| 3 | Ordenar **por nome** |
| 4 | Ordenar **por data de registro** |
| 0 | Sair |

## Conceitos demonstrados

### Ordenação natural — `IComparable<Player>`

`Player` implementa `IComparable<Player>`. O `CompareTo` compara por `Score` de forma **decrescente**, então a ordem natural já é o ranking (maior pontuação primeiro). `players.Order()` usa esse comparador padrão.

### Comparadores alternativos — `IComparer<Player>`

Como a ordem natural é única por tipo, os outros critérios ficam em classes separadas que implementam `IComparer<Player>` (`ByNameComparer`, `ByDateComparer`) e são passadas para `Order(comparer)` — sem alterar a entidade.

### Unicidade — `HashSet` + `Equals`/`GetHashCode`

Os jogadores ficam num `HashSet<Player>`, cujo `Add` rejeita duplicados. Isso **só funciona porque `Player` sobrescreve os dois métodos**:

- `GetHashCode` — o `HashSet` usa o hash para achar o "balde"; sem ele, cada objeto seria único (identidade de referência) e nenhum duplicado seria detectado.
- `Equals` — confirma a igualdade real dentro do balde (dois jogadores são iguais se têm o mesmo nome, case-insensitive).

O contrato entre os dois é obrigatório: **objetos iguais têm o mesmo hash**, por isso ambos derivam do mesmo campo (`Name`). O hash usa a identidade estável (`Name`), nunca o `Score` mutável.

> Nuance: a igualdade é por **nome** e a ordenação é por **pontos**. Por isso a unicidade usa `HashSet` (baseado em `Equals`/`GetHashCode`) e não `SortedSet` — este último decidiria duplicidade pelo `CompareTo` e descartaria jogadores de mesma pontuação.

## Como executar

Pré-requisito: [SDK do .NET 10](https://dotnet.microsoft.com/download).

```bash
cd PlayersRankingProject
dotnet run
```
