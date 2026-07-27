using LibraryApi.DTOs.Request;
using LibraryApi.DTOs.Response;
using LibraryApi.Extensions;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("books")]
[Authorize]
public class BookController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks([FromQuery] GenericParameters parameters)
    {
        var books = await _uow.Books.ListAll(parameters);

        return Ok(books.ToResponse());
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponse>> GetBook(Guid id)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        return book is null ? NotFound() : Ok(book.ToResponse());
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook([FromBody] CreateBookRequest request)
    {
        var book = _uow.Books.Create(request.ToEntity());

        await _uow.Commit();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book.ToResponse());
    }

    [Authorize(Roles = "Admin,Librarian")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookResponse>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        if (book is null) return NotFound();

        book.ApplyUpdate(request);

        await _uow.Commit();

        return Ok(book.ToResponse());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        if (book is null) return NotFound();

        if (await _uow.Loans.HasActiveLoans(id))
            return Conflict("Cannot delete a book with active loans.");

        _uow.Books.Delete(book);

        await _uow.Commit();

        return NoContent();
    }
}
