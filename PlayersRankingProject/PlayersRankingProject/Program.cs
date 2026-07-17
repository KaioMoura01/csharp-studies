using PlayersRankingProject.Domain;
using PlayersRankingProject.Entities;
using PlayersRankingProject.Entities.Comparers;
using PlayersRankingProject.Misc;

var players = new HashSet<Player>();

Console.WriteLine("=== Welcome to PlayersRankingProject ===");

var running = true;
while (running)
{
    Console.WriteLine();
    Console.WriteLine("1 - Add player");
    Console.WriteLine("2 - Ranking (by points)");
    Console.WriteLine("3 - Sort by name");
    Console.WriteLine("4 - Sort by registration date");
    Console.WriteLine("0 - Exit");

    var option = Utils.ReadInt("Choose an option: ");
    Console.WriteLine();

    try
    {
        switch (option)
        {
            case 1:
            {
                var name = Utils.ReadString("Player name: ");
                var score = Utils.ReadInt("Score: ");
                var date = Utils.ReadDate("Registration date (dd/MM/yyyy): ");

                var player = new Player(name, score, date);
                if (players.Add(player))
                {
                    Console.WriteLine("Player added successfully!");
                }
                else
                {
                    Console.WriteLine($"Player \"{name}\" already exists.");
                }

                break;
            }
            case 2:
                PrintPlayers("Ranking by points", players.Order());
                break;
            case 3:
                PrintPlayers("Sorted by name", players.Order(new ByNameComparer()));
                break;
            case 4:
                PrintPlayers("Sorted by registration date", players.Order(new ByDateComparer()));
                break;
            case 0:
                running = false;
                Console.WriteLine("Goodbye!");
                break;
            default:
                Console.WriteLine("Invalid option, please try again.");
                break;
        }
    }
    catch (DomainException e)
    {
        Console.WriteLine($"Validation error: {e.Message}");
    }
    catch (Exception e)
    {
        Console.WriteLine($"Unexpected error: {e.Message}");
    }
}

return;

void PrintPlayers(string title, IEnumerable<Player> ordered)
{
    if (players.Count == 0)
    {
        Console.WriteLine("No players registered yet.");
        return;
    }

    Console.WriteLine($"=== {title} ===");
    var position = 1;
    foreach (var player in ordered)
    {
        Console.WriteLine($"{position++}) {player}");
    }
}
