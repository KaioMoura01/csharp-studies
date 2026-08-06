# Build context for this Dockerfile must be the repository root (not src/User),
# because the csproj references the shared proto at src/Shared/Protos/user.proto.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/ src/Shared/
COPY src/User/KShop.UserApi.Domain/ src/User/KShop.UserApi.Domain/
COPY src/User/KShop.UserApi.Application/ src/User/KShop.UserApi.Application/
COPY src/User/KShop.UserApi.Infrastructure/ src/User/KShop.UserApi.Infrastructure/
COPY src/User/KShop.UserApi/ src/User/KShop.UserApi/
RUN dotnet publish src/User/KShop.UserApi/KShop.UserApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080 8081
ENTRYPOINT ["dotnet", "KShop.UserApi.dll"]
