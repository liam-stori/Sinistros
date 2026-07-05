using Scalar.AspNetCore;
using SinistrosApi.Api;
using SinistrosApi.Api.Middleware;
using SinistrosApi.Application;
using SinistrosApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AdicionarInfrastructure(builder.Configuration);
builder.Services.AdicionarApplication();
builder.Services.AdicionarApi();

var app = builder.Build();

await app.Services.InicializarBancoAsync();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseTratadorDeExcecoes();

app.MapControllers();

app.Run();
