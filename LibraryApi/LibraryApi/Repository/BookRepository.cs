using LibraryApi.Context;
using LibraryApi.Interfaces;
using LibraryApi.Models;

namespace LibraryApi.Repository;

public class BookRepository(AppDbContext context) : NamedRepository<Book>(context), IBook
{
    
}