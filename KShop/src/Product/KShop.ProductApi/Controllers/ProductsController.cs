using KShop.ProductApi.Application.Abstractions;
using KShop.ProductApi.Application.DTOs.Products;
using KShop.ProductApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KShop.ProductApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductResponse>>> GetAll()
    {
        return Ok(await productService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var product = await productService.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
    {
        try
        {
            var product = await productService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (UnrecognizedUserException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> Update(Guid id, UpdateProductRequest request)
    {
        if (id != request.Id) return BadRequest("The route ID does not match the request body ID.");

        var product = await productService.UpdateAsync(id, request);
        return product is null ? NotFound() : Ok(product);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
