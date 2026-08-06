using KShop.ProductApi.Application.DTOs.Categories;
using KShop.ProductApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KShop.ProductApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponse>>> GetAll()
    {
        return Ok(await categoryService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id)
    {
        var category = await categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(CreateCategoryRequest request)
    {
        var category = await categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryResponse>> Update(Guid id, UpdateCategoryRequest request)
    {
        if (id != request.Id) return BadRequest("The route ID does not match the request body ID.");

        var category = await categoryService.UpdateAsync(id, request);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await categoryService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
