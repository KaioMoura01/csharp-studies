using Grpc.Core;
using UserService.Application.Users;
using UserService.Grpc;

namespace UserService.Api.Services;

public sealed class UserGrpcServiceImpl(GetUserHandler getUserHandler, ListUsersHandler listUsersHandler)
    : UserGrpcService.UserGrpcServiceBase
{
    public override async Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        try
        {
            var user = await getUserHandler.HandleAsync(request.Id, context.CancellationToken);
            return ToReply(user);
        }
        catch (UserNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
    }

    public override async Task ListUsers(ListUsersRequest request, IServerStreamWriter<UserReply> responseStream, ServerCallContext context)
    {
        var users = await listUsersHandler.HandleAsync(context.CancellationToken);
        foreach (var user in users)
        {
            await responseStream.WriteAsync(ToReply(user));
        }
    }

    private static UserReply ToReply(Application.Users.UserDto user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Active = user.Active,
    };
}
