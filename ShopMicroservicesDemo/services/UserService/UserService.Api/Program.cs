using UserService.Api.Services;
using UserService.Application.Users;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<GetUserHandler>();
builder.Services.AddScoped<ListUsersHandler>();

var app = builder.Build();

app.MapGrpcService<UserGrpcServiceImpl>();
app.MapGet("/", () => "UserService gRPC está no ar. Use um cliente gRPC para se comunicar.");

app.Run();
