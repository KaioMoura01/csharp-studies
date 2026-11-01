FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JcmSoft.Console/JcmSoft.Console.csproj", "JcmSoft.Console/"]
COPY ["JcmSoft.Domain/JcmSoft.Domain.csproj", "JcmSoft.Domain/"]
COPY ["JcmSoft.EFCore/JcmSoft.EFCore.csproj", "JcmSoft.EFCore/"]
RUN dotnet restore "JcmSoft.Console/JcmSoft.Console.csproj"
COPY . .
WORKDIR "/src/JcmSoft.Console"
RUN dotnet build "./JcmSoft.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JcmSoft.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JcmSoft.Console.dll"]
