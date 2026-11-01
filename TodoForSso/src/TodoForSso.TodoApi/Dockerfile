# Build context deve ser a raiz do repositório (TodoForSso/), não src/TodoForSso.TodoApi.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/TodoForSso.TodoApi.Domain/ src/TodoForSso.TodoApi.Domain/
COPY src/TodoForSso.TodoApi.Application/ src/TodoForSso.TodoApi.Application/
COPY src/TodoForSso.TodoApi.Infrastructure/ src/TodoForSso.TodoApi.Infrastructure/
COPY src/TodoForSso.TodoApi/ src/TodoForSso.TodoApi/
RUN dotnet publish src/TodoForSso.TodoApi/TodoForSso.TodoApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoForSso.TodoApi.dll"]
