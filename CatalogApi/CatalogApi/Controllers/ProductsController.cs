using CatalogApi.DTOs;
using CatalogApi.Extensions;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;

    private static Product GenerateProduct(ProductRequestDto product)
    {
        return new Product
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
        };
    }

    [HttpGet("category/{categoryId:int}")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetByCategoryId(int categoryId)
    {
        var products = await _uow.Products.GetProductsByCategory(categoryId);
        return Ok(products.ToResponseDto());
    }
    
    //========================================================
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get([FromQuery] GenericParameters parameters)
    {
        var products = await _uow.Products.ListAll(parameters);
        return Ok(products.ToResponseDto());
    }
    
    //========================================================
    [HttpGet("{id:int}", Name =  "GetProductById")]
    public async Task<ActionResult<ProductResponseDto>> Get(int id)
    {
        var product = await _uow.Products.Get(p=>p.ProductId==id);
        
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product.ToResponseDto());
    }

    //========================================================
    [HttpPost]
    public async Task<ActionResult> Post(ProductRequestDto product)
    {
        var newProduct = GenerateProduct(product);
        var created = _uow.Products.Create(newProduct);
        await _uow.Commit();
        return new CreatedAtRouteResult("GetProductById", new { id = created.ProductId }, created.ToResponseDto());
    }

    //========================================================
    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> Put(int id, ProductRequestDto product)
    {
        var existing = await _uow.Products.Get(p => p.ProductId == id);

        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.ImageUrl = product.ImageUrl;
        existing.Stock = product.Stock;
        existing.CategoryId = product.CategoryId;

        await _uow.Commit();
        return Ok(existing.ToResponseDto());
    }

    //========================================================
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> Delete(int id)
    {
        var product = await _uow.Products.Get(p=>p.ProductId==id);

        if (product is null)
        {
            return NotFound();
        }
        
        var deleted = _uow.Products.Delete(product);
        await _uow.Commit();
        return Ok(deleted);
        
    }
}