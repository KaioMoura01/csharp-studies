using LibraryApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Models;

public class Book:INamed
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Publisher { get; set; }
    public int YearOfPublication { get; set; }
    public int TotalQuantity { get; set; }
    public int Stock { get; set; }
}