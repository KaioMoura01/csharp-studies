using System.Globalization;
using PlayersRankingProject.Domain;

namespace PlayersRankingProject.Entities;

public class Player : IComparable<Player>
{
    public string Name { get; }
    private int Score { get; set; }
    public DateTime RegistrationDate { get; set; }

    public Player(string name, int score, DateTime registrationDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Player name is required.");
        }

        if (score < 0)
        {
            throw new DomainException("Score cannot be negative.");
        }

        Name = name;
        Score = score;
        RegistrationDate = registrationDate;
    }

    public int CompareTo(Player? other)
    {
        return other is null ? 1 : other.Score.CompareTo(Score);
    }

    public override bool Equals(object? obj)
    {
        return obj is Player other &&
               Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Name.ToLowerInvariant().GetHashCode();
    }

    public override string ToString()
    {
        return $"{Name} - {Score} pts ({RegistrationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)})";
    }
}
