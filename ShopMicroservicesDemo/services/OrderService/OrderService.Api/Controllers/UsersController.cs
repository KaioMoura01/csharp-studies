using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Abstractions;

namespace OrderService.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserServiceClient userServiceClient) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RemoteUser>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userServiceClient.ListUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RemoteUser>> GetById(string id, CancellationToken cancellationToken)
    {
        var user = await userServiceClient.GetUserAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }
}
