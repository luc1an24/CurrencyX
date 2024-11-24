using ExchangeRates.Api.Interfaces;
using ExchangeRates.Api.Repositories;
using ExchangeRates.Api.Services;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGci...\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowKnownOriginsOn4200",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",
                "http://188.230.164.132:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? "");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));

var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");
builder.Services.AddSingleton<IConnectionStrings>(new ConnectionStrings
{
    Postgres = connectionStrings["Postgres"] ?? "",
    Redis = connectionStrings["Redis"] ?? ""
});

builder.Services.AddDbContext<ExchangeRatesDbContext>(options => options.UseNpgsql(connectionStrings["Postgres"]));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(connectionStrings["Redis"] ?? "");
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

var externalApiOptions = (builder.Configuration.GetSection("ExternalApi"));
builder.Services.AddSingleton<IExternalApiOptions>(new ExternalApiOptions
{
    Url = externalApiOptions["url"] ?? string.Empty,
    ApiKey = externalApiOptions["key"] ?? string.Empty
});

builder.Services.AddHttpClient();

builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowKnownOriginsOn4200");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
