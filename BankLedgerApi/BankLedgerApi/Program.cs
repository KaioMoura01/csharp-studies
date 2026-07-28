using BankLedgerApi.Configurations;
using BankLedgerApi.Context;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

DependencyInjection.ConfigureServices(builder);

var app = builder.Build();

AppConfiguration.ApplyConfiguration(app);
