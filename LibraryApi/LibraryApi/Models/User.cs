using LibraryApi.Domain;
using LibraryApi.Enums;

namespace LibraryApi.Models;

public class User:INamed
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public Role Role { get; set; }
    public ICollection<Loan> Loans { get; set; } = [];
}