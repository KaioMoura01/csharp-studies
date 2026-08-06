using Grpc.Core;
using Grpc.Net.Client;
using KShop.ProductApi.Application.Abstractions;
using KShop.UserApi.Grpc;

namespace KShop.ProductApi.Infrastructure.Grpc;

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

    public async Task<RemoteUserProfile?> GetUserBySubAsync(string sub, CancellationToken cancellationToken)
    {
        try
        {
            var reply = await _client.GetUserBySubAsync(
                new GetUserBySubRequest { Sub = sub },
                deadline: DateTime.UtcNow.AddSeconds(3),
                cancellationToken: cancellationToken);

            return new RemoteUserProfile(Guid.Parse(reply.Id), reply.KeycloakSubjectId, reply.DisplayName, reply.Email, reply.Active);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public void Dispose() => _channel.Dispose();
}
