using LibraryApi.Context;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;

namespace LibraryApi.Repository;

public class BookRepository(AppDbContext context) : NamedRepository<Book>(context), IBook
{
    
}