using TodoForSso.TodoApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.ConfigureServices();

var app = builder.ConfigureApp();
app.Run();
