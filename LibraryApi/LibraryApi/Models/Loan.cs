namespace LibraryApi.Models;

public class Loan
{
    public Guid Id { get; set; }
    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime LatestReturnDate { get; set; } = DateTime.UtcNow.AddDays(14);
    public DateTime? ReturnDate { get; set; }

    public Guid UserId { get; set; }
    public required User User { get; set; }

    public Guid BookId { get; set; }
    public required Book Book { get; set; }
    
    public bool Returned => ReturnDate.HasValue;
}