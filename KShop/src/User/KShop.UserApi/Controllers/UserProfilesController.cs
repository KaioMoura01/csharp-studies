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

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMe()
    {
        var sub = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub)) return Unauthorized();

        var userProfile = await userProfileService.GetBySubAsync(sub);
        return userProfile is null ? NotFound() : Ok(userProfile);
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
