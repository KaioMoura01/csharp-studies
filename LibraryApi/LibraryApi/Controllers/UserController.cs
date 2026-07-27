using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibraryApi.DTOs.Request;
using LibraryApi.DTOs.Response;
using LibraryApi.Enums;
using LibraryApi.Extensions;
using LibraryApi.Models;
using LibraryApi.Repository.Interfaces;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UserController(
    IUnitOfWork unitOfWork,
    IPasswordHasher<User> passwordHasher,
    IToken token,
    IConfiguration configuration) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly IToken _token = token;
    private readonly IConfiguration _configuration = configuration;

    [Authorize(Roles = "Admin,Librarian")]
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

    [AllowAnonymous]
    [HttpPost]
    public Task<ActionResult<UserResponse>> Register([FromBody] CreateUserRequest request)
        => CreateUser(request, Role.User);

    [Authorize(Roles = "Admin")]
    [HttpPost("librarians")]
    public Task<ActionResult<UserResponse>> CreateLibrarian([FromBody] CreateUserRequest request)
        => CreateUser(request, Role.Librarian);

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _uow.Users.Get(u => u.Email == request.Email);
        if (user is null) return Unauthorized();

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var jwt = _token.GenerateToken(claims, _configuration);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

        return Ok(new AuthResponse(tokenString, jwt.ValidTo));
    }

    private async Task<ActionResult<UserResponse>> CreateUser(CreateUserRequest request, Role role)
    {
        var exists = await _uow.Users.Get(u => u.Email == request.Email);
        if (exists is not null) return Conflict("Email already registered.");

        var user = request.ToEntity(passwordHash: string.Empty, role: role);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _uow.Users.Create(user);
        await _uow.Commit();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.ToResponse());
    }
}
