namespace PlayersRankingProject.Entities.Comparers;

public class ByNameComparer : IComparer<Player>
{
    public int Compare(Player? x, Player? y)
    {
        return string.Compare(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
    }
}
