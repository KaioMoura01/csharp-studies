using Grpc.Core;
using Grpc.Net.Client;
using OrderService.Application.Abstractions;
using UserService.Grpc;

namespace OrderService.Infrastructure.Grpc;

public sealed class UserServiceGrpcClient : IUserServiceClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly UserGrpcService.UserGrpcServiceClient _client;

    public UserServiceGrpcClient(UserServiceGrpcOptions options)
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        _channel = GrpcChannel.ForAddress(options.Address);
        _client = new UserGrpcService.UserGrpcServiceClient(_channel);
    }

    public async Task<RemoteUser?> GetUserAsync(string userId, CancellationToken cancellationToken)
    {
        try
        {
            var reply = await _client.GetUserAsync(
                new GetUserRequest { Id = userId },
                deadline: DateTime.UtcNow.AddSeconds(3),
                cancellationToken: cancellationToken);

            return new RemoteUser(reply.Id, reply.Name, reply.Email, reply.Active);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IReadOnlyCollection<RemoteUser>> ListUsersAsync(CancellationToken cancellationToken)
    {
        var result = new List<RemoteUser>();
        using var call = _client.ListUsers(new ListUsersRequest(), cancellationToken: cancellationToken);

        await foreach (var reply in call.ResponseStream.ReadAllAsync(cancellationToken))
        {
            result.Add(new RemoteUser(reply.Id, reply.Name, reply.Email, reply.Active));
        }

        return result;
    }

    public void Dispose() => _channel.Dispose();
}
