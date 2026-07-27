using LibraryApi.Context;
using LibraryApi.Interfaces;
using LibraryApi.Models;

namespace LibraryApi.Repository;

public class UserRepository(AppDbContext context): NamedRepository<User>(context),  IUser
{
    
}