using LibraryApi.Dtos.Request;
using LibraryApi.Dtos.Response;
using LibraryApi.Models;

namespace LibraryApi.Extensions;

public static class BookMappingExtensions
{
    public static BookResponse ToResponse(this Book book) => new(
        book.Id,
        book.Name,
        book.Description,
        book.Publisher,
        book.YearOfPublication,
        book.TotalQuantity,
        book.Stock);

    public static IEnumerable<BookResponse> ToResponse(this IEnumerable<Book> books)
        => books.Select(b => b.ToResponse());

    public static Book ToEntity(this CreateBookRequest request) => new()
    {
        Name = request.Name,
        Description = request.Description,
        Publisher = request.Publisher,
        YearOfPublication = request.YearOfPublication,
        TotalQuantity = request.TotalQuantity,
        Stock = request.TotalQuantity
    };

    public static void ApplyUpdate(this Book book, UpdateBookRequest request)
    {
        book.Name = request.Name;
        book.Description = request.Description;
        book.Publisher = request.Publisher;
        book.YearOfPublication = request.YearOfPublication;
        book.TotalQuantity = request.TotalQuantity;
    }
}
