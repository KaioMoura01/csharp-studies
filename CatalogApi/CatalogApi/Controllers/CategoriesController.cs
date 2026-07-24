using CatalogApi.DTOs;
using CatalogApi.Extensions;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> Get([FromQuery] GenericParameters parameters)
    {
        var categories = await _uow.Categories.ListAll(parameters);

        return Ok(categories.ToResponseDto());
    }
    
    //========================================================
    [HttpGet("{id:int}", Name =  "GetCategoryById")]
    public async Task<ActionResult<CategoryResponseDto>> Get(int id)
    {
        var category = await _uow.Categories.Get(c=> c.CategoryId==id);

        if (category is null)
        {
            return NotFound();
        }
        
        return Ok(category.ToResponseDto());
    }

    //========================================================
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CategoryResponseDto>> Post(CategoryRequestDto category)
    {
        var newCategory = GenerateCategory(category);
        var created = _uow.Categories.Create(newCategory);
        await _uow.Commit();
        return new CreatedAtRouteResult("GetCategoryById", new { id = created.CategoryId }, created.ToResponseDto());
    }
    
    //========================================================
    [Authorize(Roles = "Admin,Financial")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> Put(int id,  CategoryRequestDto category)
    {
        var newCategory = GenerateCategory(category);
        newCategory.CategoryId = id;
        var updated = _uow.Categories.Update(newCategory);
        await _uow.Commit();
        return Ok(updated.ToResponseDto());
    } 

    //========================================================
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> Delete(int id)
    {
        var category = await _uow.Categories.Get(c=> c.CategoryId==id);
        
        if(category is null)
        {
            return NotFound();
        }
        
        var deleted = _uow.Categories.Delete(category);
        await _uow.Commit();
        return Ok(deleted);
    }
}