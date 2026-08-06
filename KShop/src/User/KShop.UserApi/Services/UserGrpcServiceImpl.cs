using Grpc.Core;
using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Application.Services.Interfaces;
using KShop.UserApi.Grpc;

namespace KShop.UserApi.Services;

public class UserGrpcServiceImpl(IUserProfileService userProfileService) : UserGrpcService.UserGrpcServiceBase
{
    public override async Task<UserReply> GetUser(GetUserRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user id"));
        }

        var userProfile = await userProfileService.GetByIdAsync(id);
        if (userProfile is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User '{request.Id}' not found"));
        }

        return ToReply(userProfile);
    }

    public override async Task<UserReply> GetUserBySub(GetUserBySubRequest request, ServerCallContext context)
    {
        var userProfile = await userProfileService.GetBySubAsync(request.Sub);
        if (userProfile is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"User with sub '{request.Sub}' not found"));
        }

        return ToReply(userProfile);
    }

    private static UserReply ToReply(UserProfileResponse userProfile) => new()
    {
        Id = userProfile.Id.ToString(),
        KeycloakSubjectId = userProfile.KeycloakSubjectId ?? string.Empty,
        DisplayName = userProfile.DisplayName ?? string.Empty,
        Email = userProfile.Email ?? string.Empty,
        Active = userProfile.Active,
    };
}
