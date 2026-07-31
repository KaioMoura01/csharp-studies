FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankLedgerApi/BankLedgerApi.csproj", "BankLedgerApi/"]
COPY ["BankLedgerApi.Application/BankLedgerApi.Application.csproj", "BankLedgerApi.Application/"]
COPY ["BankLedgerApi.Domain/BankLedgerApi.Domain.csproj", "BankLedgerApi.Domain/"]
COPY ["BankLedgerApi.Infrastructure/BankLedgerApi.Infrastructure.csproj", "BankLedgerApi.Infrastructure/"]
RUN dotnet restore "BankLedgerApi/BankLedgerApi.csproj"
COPY . .
WORKDIR "/src/BankLedgerApi"
RUN dotnet build "./BankLedgerApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankLedgerApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankLedgerApi.dll"]
