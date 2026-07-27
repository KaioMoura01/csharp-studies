using LibraryApi.Dtos.Request;
using LibraryApi.Dtos.Response;
using LibraryApi.Extensions;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("loans")]
// TODO: [Authorize(Roles = "Admin,Librarian")]
public class LoanController(IUnitOfWork unitOfWork) : ControllerBase
{
    private const int MaxActiveLoansPerUser = 3;
    private readonly IUnitOfWork _uow = unitOfWork;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetLoans([FromQuery] GenericParameters parameters)
    {
        var loans = await _uow.Loans.ListAllWithDetails(parameters);

        return Ok(loans.ToResponse());
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetOverdue()
    {
        var loans = await _uow.Loans.GetOverdue();

        return Ok(loans.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanResponse>> GetLoan(Guid id)
    {
        var loan = await _uow.Loans.GetWithDetails(id);

        return loan is null ? NotFound() : Ok(loan.ToResponse());
    }

    [HttpPost]
    public async Task<ActionResult<LoanResponse>> CreateLoan([FromBody] CreateLoanRequest request)
    {
        var user = await _uow.Users.Get(u => u.Id == request.UserId);
        if (user is null) return BadRequest("Usuário não encontrado.");

        var book = await _uow.Books.Get(b => b.Id == request.BookId);
        if (book is null) return BadRequest("Livro não encontrado.");

        if (book.Stock <= 0)
            return Conflict("Livro sem estoque disponível.");

        var activeLoans = await _uow.Loans.CountActiveByUser(user.Id);
        if (activeLoans >= MaxActiveLoansPerUser)
            return Conflict($"Usuário já possui {MaxActiveLoansPerUser} livros emprestados.");

        var loan = new Loan
        {
            User = user,
            UserId = user.Id,
            Book = book,
            BookId = book.Id
        };
        book.Stock--;

        _uow.Loans.Create(loan);
        await _uow.Commit();

        return CreatedAtAction(nameof(GetLoan), new { id = loan.Id }, loan.ToResponse());
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<LoanResponse>> ReturnLoan(Guid id)
    {
        var loan = await _uow.Loans.GetWithDetails(id);
        if (loan is null) return NotFound();
        if (loan.Returned) return Conflict("Empréstimo já foi devolvido.");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.Stock++;

        await _uow.Commit();

        return Ok(loan.ToResponse());
    }
}
