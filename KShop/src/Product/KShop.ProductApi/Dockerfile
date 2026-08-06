# Build context for this Dockerfile must be the repository root (not src/Product),
# because the csproj references the shared proto at src/Shared/Protos/user.proto.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Shared/ src/Shared/
COPY src/Product/KShop.ProductApi.Domain/ src/Product/KShop.ProductApi.Domain/
COPY src/Product/KShop.ProductApi.Application/ src/Product/KShop.ProductApi.Application/
COPY src/Product/KShop.ProductApi.Infrastructure/ src/Product/KShop.ProductApi.Infrastructure/
COPY src/Product/KShop.ProductApi/ src/Product/KShop.ProductApi/
RUN dotnet publish src/Product/KShop.ProductApi/KShop.ProductApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "KShop.ProductApi.dll"]
