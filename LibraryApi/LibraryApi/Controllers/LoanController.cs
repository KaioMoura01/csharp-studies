using LibraryApi.DTOs.Request;
using LibraryApi.DTOs.Response;
using LibraryApi.Extensions;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("loans")]
[Authorize(Roles = "Admin,Librarian")]
public class LoanController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;
    private const bool UseMaxActiveLoansPerUser = false;
    private const int MaxActiveLoansPerUser = 3;

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
        if (user is null) return BadRequest("User not found.");

        var book = await _uow.Books.Get(b => b.Id == request.BookId);
        if (book is null) return BadRequest("Book not found.");

        if (book.Stock <= 0)
            return Conflict("Book is out of stock.");

        if (UseMaxActiveLoansPerUser)
        {
            var activeLoans = await _uow.Loans.CountActiveByUser(user.Id);
            if (activeLoans >= MaxActiveLoansPerUser)
                return Conflict($"User already has {MaxActiveLoansPerUser} books on loan.");
        }
        

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
        if (loan.Returned) return Conflict("Loan has already been returned.");

        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.Stock++;

        await _uow.Commit();

        return Ok(loan.ToResponse());
    }
}
