namespace PlayersRankingProject.Entities.Comparers;

public class ByDateComparer : IComparer<Player>
{
    public int Compare(Player? x, Player? y)
    {
        if (x is null || y is null)
        {
            return Comparer<object>.Default.Compare(x, y);
        }

        return x.RegistrationDate.CompareTo(y.RegistrationDate);
    }
}
