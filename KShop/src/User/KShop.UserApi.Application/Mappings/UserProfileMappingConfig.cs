using KShop.UserApi.Application.DTOs.UserProfiles;
using KShop.UserApi.Domain.Models;
using Mapster;

namespace KShop.UserApi.Application.Mappings;

public class UserProfileMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserProfileRequest, UserProfile>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt);

        config.NewConfig<UpdateUserProfileRequest, UserProfile>()
            .Ignore(dest => dest.KeycloakSubjectId)
            .Ignore(dest => dest.CreatedAt);
    }
}
