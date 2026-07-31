using BankLedgerApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

DependencyInjection.ConfigureServices(builder);

var app = builder.Build();

AppConfiguration.ApplyConfiguration(app);
