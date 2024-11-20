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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Basic Authorization header using the Basic scheme."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

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

var externalApiOptions = (builder.Configuration.GetSection("ExternalApi"));
builder.Services.AddSingleton<IExternalApiOptions>(new ExternalApiOptions
{
    Url = externalApiOptions["url"] ?? string.Empty,
    ApiKey = externalApiOptions["key"] ?? string.Empty
});

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IExchangeRateRepository>(new ExchangeRateRepository(new ConnectionStrings
{
    Postgres = connectionStrings["Postgres"] ?? "",
    Redis = connectionStrings["Redis"] ?? ""
}));

builder.Services.AddSingleton<IExchangeRateService, ExchangeRateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
