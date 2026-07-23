using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

[ApiController]
[Route("categories")]
public class CategoriesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;

    private static Category GenerateCategory(CategoryRequestDto category)
    {
        return new Category
        {
            Name = category.Name,
            ImageUrl = category.ImageUrl
        };
    }

    //========================================================
    [HttpGet]
    public ActionResult<IEnumerable<Category>> Get([FromQuery] GenericParameters parameters)
    {
        var categories = _uow.Categories.ListAll(parameters);

        return Ok(categories);
    }
    
    //========================================================
    [HttpGet("{id:int}", Name =  "GetCategoryById")]
    public ActionResult<Category> Get(int id)
    {
        var category = _uow.Categories.Get(c=> c.CategoryId==id);

        if (category is null)
        {
            return NotFound();
        }
        
        return Ok(category);
    }

    //========================================================
    [HttpPost]
    public ActionResult<Category> Post(CategoryRequestDto category)
    {
        var newCategory = GenerateCategory(category);
        var created = _uow.Categories.Create(newCategory);
        _uow.Commit();
        return new CreatedAtRouteResult("GetCategoryById", new { id = created.CategoryId }, created);
    }
    
    //========================================================
    [HttpPut("{id:int}")]
    public ActionResult<Category> Put(int id,  CategoryRequestDto category)
    {
        var newCategory = GenerateCategory(category);
        newCategory.CategoryId = id;
        var updated = _uow.Categories.Update(newCategory);
        _uow.Commit();
        return Ok(updated);
    } 

    //========================================================
    [HttpDelete("{id:int}")]
    public ActionResult<Category> Delete(int id)
    {
        var category = _uow.Categories.Get(c=> c.CategoryId==id);
        
        if(category is null)
        {
            return NotFound();
        }
        
        var deleted = _uow.Categories.Delete(category);
        _uow.Commit();
        return Ok(deleted);
    }
}