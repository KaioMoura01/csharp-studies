using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoForSso.TodoApi.Application.Dtos;
using TodoForSso.TodoApi.Application.Services;

namespace TodoForSso.TodoApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IUserAdminService userAdminService) : ControllerBase
{
    /// <summary>
    /// Criação de usuário pela tela administrativa. O auto-cadastro migrou para o Keycloak,
    /// então este endpoint deixou de ser público: só admin cria conta por aqui.
    /// </summary>
    [HttpPost("register")]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<UserDto>> Register(RegisterUserDto dto)
    {
        var user = await userAdminService.RegisterAsync(dto);
        return CreatedAtAction(nameof(GetAll), user);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<ActionResult<List<UserDto>>> GetAll() => Ok(await userAdminService.GetAllAsync());

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SetStatus(string id, UpdateUserStatusDto dto)
    {
        var updated = await userAdminService.SetEnabledAsync(id, dto.Enabled);
        return updated ? NoContent() : NotFound();
    }
}
