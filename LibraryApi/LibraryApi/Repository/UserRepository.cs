using LibraryApi.Context;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;

namespace LibraryApi.Repository;

public class UserRepository(AppDbContext context): NamedRepository<User>(context),  IUser
{
    
}