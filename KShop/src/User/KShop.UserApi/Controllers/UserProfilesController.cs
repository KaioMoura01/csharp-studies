using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KShop.UserApi.Controllers;

[ApiController]
[Route("api/userprofiles")]
public class UserProfilesController(IUserProfileService userProfileService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserProfileResponse>>> GetAll()
    {
        return Ok(await userProfileService.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileResponse>> GetById(Guid id)
    {
        var userProfile = await userProfileService.GetByIdAsync(id);
        return userProfile is null ? NotFound() : Ok(userProfile);
    }

    /// <summary>
    /// Perfil local do usuário autenticado, criado sob demanda no primeiro acesso.
    /// O cadastro agora acontece no Keycloak, que não conhece esta tabela — então o perfil
    /// nasce aqui, a partir das claims do token, na primeira vez que o usuário aparece.
    /// Sem isso o gRPC GetUserBySub não acharia ninguém e criar produto falharia.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMe()
    {
        var sub = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub)) return Unauthorized();

        var userProfile = await userProfileService.GetBySubAsync(sub);
        if (userProfile is not null) return Ok(userProfile);

        var displayName = User.FindFirst("name")?.Value
            ?? User.FindFirst("preferred_username")?.Value
            ?? sub;
        var email = User.FindFirst("email")?.Value ?? string.Empty;

        return Ok(await userProfileService.CreateAsync(new CreateUserProfileRequest(sub, displayName, email)));
    }

    [HttpPost]
    public async Task<ActionResult<UserProfileResponse>> Create(CreateUserProfileRequest request)
    {
        var userProfile = await userProfileService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = userProfile.Id }, userProfile);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserProfileResponse>> Update(Guid id, UpdateUserProfileRequest request)
    {
        if (id != request.Id) return BadRequest("The route ID does not match the request body ID.");

        var userProfile = await userProfileService.UpdateAsync(id, request);
        return userProfile is null ? NotFound() : Ok(userProfile);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await userProfileService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
