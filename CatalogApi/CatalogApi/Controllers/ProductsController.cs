using CatalogApi.DTOs;
using CatalogApi.Models;
using CatalogApi.Pagination;
using CatalogApi.Repositories;
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
    public ActionResult<IEnumerable<Product>> GetByCategoryId(int categoryId)
    {
        var products = _uow.Products.GetProductsByCategory(categoryId);
        return Ok(products);
    }
    
    //========================================================
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] GenericParameters parameters)
    {
        var products = _uow.Products.ListAll(parameters);
        return Ok(products);
    }
    
    //========================================================
    [HttpGet("{id:int}", Name =  "GetProductById")]
    public ActionResult<Product> Get(int id)
    {
        var product = _uow.Products.Get(p=>p.ProductId==id);
        
        if (product is null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }

    //========================================================
    [HttpPost]
    public ActionResult Post(ProductRequestDto product)
    {
        var newProduct = GenerateProduct(product);
        var created = _uow.Products.Create(newProduct);
        _uow.Commit();
        return new CreatedAtRouteResult("GetProductById", new { id = created.ProductId }, created);
    }

    //========================================================
    [HttpPut("{id:int}")]
    public ActionResult<Product> Put(int id, ProductRequestDto product)
    {
        var existing = Get(id);

        if (existing.Value is null)
        {
            return NotFound();
        }
        
        var newProduct = GenerateProduct(product);
        newProduct.ProductId = id;
        newProduct.CreatedAt = existing.Value.CreatedAt;
        
        var updated = _uow.Products.Update(newProduct);
        _uow.Commit();
        return Ok(updated);
    }

    //========================================================
    [HttpDelete("{id:int}")]
    public ActionResult<Product> Delete(int id)
    {
        var product = _uow.Products.Get(p=>p.ProductId==id);

        if (product is null)
        {
            return NotFound();
        }
        
        var deleted = _uow.Products.Delete(product);
        _uow.Commit();
        return Ok(deleted);
        
    }
}