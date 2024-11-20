using ExchangeRates.Api.Authentication;
using ExchangeRates.Api.Interfaces;
using ExchangeRates.Api.Repositories;
using ExchangeRates.Api.Services;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("BasicAuth").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuth", null);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");
builder.Services.AddSingleton<IConnectionStrings>(new ConnectionStrings
{
    Postgres = connectionStrings["Postgres"] ?? "",
    Redis = connectionStrings["Redis"] ?? ""
});
builder.Services.Configure<ExternalApiOptions>(builder.Configuration.GetSection("ExternalApi"));

builder.Services.AddSingleton<IExchangeRateRepository>(new ExchangeRateRepository(new ConnectionStrings
{
    Postgres = connectionStrings["Postgres"] ?? "",
    Redis = connectionStrings["Redis"] ?? ""
}));
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
