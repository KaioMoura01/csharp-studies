using LibraryApi.DTOs.Request;
using LibraryApi.DTOs.Response;
using LibraryApi.Enums;
using LibraryApi.Extensions;
using LibraryApi.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("admin/roles")]
[Authorize(Roles = "Admin")]
public class RolesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _uow = unitOfWork;

    [HttpGet]
    public ActionResult GetRoles()
        => Ok(Enum.GetValues<Role>().Select(r => new { Id = (int)r, Name = r.ToString() }));

    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<UserResponse>> SetRole(Guid userId, [FromBody] UpdateUserRoleRequest request)
    {
        var user = await _uow.Users.Get(u => u.Id == userId);
        if (user is null) return NotFound();

        user.Role = request.Role;
        await _uow.Commit();

        return Ok(user.ToResponse());
    }
}
