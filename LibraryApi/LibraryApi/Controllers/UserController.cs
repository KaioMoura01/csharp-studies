using LibraryApi.Dtos.Request;
using LibraryApi.Dtos.Response;
using LibraryApi.Enums;
using LibraryApi.Extensions;
using LibraryApi.Interfaces;
using LibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    // TODO: [Authorize(Roles = "Admin,Librarian")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers([FromQuery] GenericParameters parameters)
    {
        var users = await _uow.Users.ListAll(parameters);

        return Ok(users.ToResponse());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUser(Guid id)
    {
        var user = await _uow.Users.Get(u => u.Id == id);

        return user is null ? NotFound() : Ok(user.ToResponse());
    }

    [HttpPost]
    public Task<ActionResult<UserResponse>> Register([FromBody] CreateUserRequest request)
        => CreateUser(request, Role.User);

    // TODO: [Authorize(Roles = "Admin")]
    [HttpPost("librarians")]
    public Task<ActionResult<UserResponse>> CreateLibrarian([FromBody] CreateUserRequest request)
        => CreateUser(request, Role.Librarian);

    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _uow.Users.Get(u => u.Email == request.Email);
        if (user is null) return Unauthorized();

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized();

        // TODO: emitir um JWT aqui em vez de devolver o usuário.
        return Ok(user.ToResponse());
    }

    private async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request, Role role)
    {
        var exists = await _uow.Users.Get(u => u.Email == request.Email);
        if (exists is not null) return Conflict("E-mail já cadastrado.");

        var user = request.ToEntity(passwordHash: string.Empty, role: role);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _uow.Users.Create(user);
        await _uow.Commit();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.ToResponse());
    }
}
