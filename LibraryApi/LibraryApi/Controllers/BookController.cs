using LibraryApi.Dtos.Request;
using LibraryApi.Dtos.Response;
using LibraryApi.Extensions;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("books")]
public class BookController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks([FromQuery] GenericParameters parameters)
    {
        var books = await _uow.Books.ListAll(parameters);

        return Ok(books.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponse>> GetBook(Guid id)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        return book is null ? NotFound() : Ok(book.ToResponse());
    }

    // TODO: [Authorize(Roles = "Admin,Librarian")]
    [HttpPost]
    public async Task<ActionResult<BookResponse>> CreateBook([FromBody] CreateBookRequest request)
    {
        var book = _uow.Books.Create(request.ToEntity());

        await _uow.Commit();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book.ToResponse());
    }

    // TODO: [Authorize(Roles = "Admin")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookResponse>> UpdateBook(Guid id, [FromBody] UpdateBookRequest request)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        if (book is null) return NotFound();

        book.ApplyUpdate(request);

        await _uow.Commit();

        return Ok(book.ToResponse());
    }

    // TODO: [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var book = await _uow.Books.Get(b => b.Id == id);

        if (book is null) return NotFound();

        if (await _uow.Loans.HasActiveLoans(id))
            return Conflict("Não é possível deletar um livro com empréstimos ativos.");

        _uow.Books.Delete(book);

        await _uow.Commit();

        return NoContent();
    }
}
