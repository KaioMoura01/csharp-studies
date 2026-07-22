using CatalogApi.Context;
using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Repositories;

public class CategoryRepository(CatalogApiContext context) : GenericRepository<Category>(context), ICategoryRepository
{
}