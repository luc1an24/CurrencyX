using ExchangeRates.Fetcher.Interfaces;
using ExchangeRates.Fetcher.Repositories;
using ExchangeRates.Fetcher.Services;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var connectionStrings = context.Configuration.GetSection("ConnectionStrings");
        services.AddSingleton<IConnectionStrings>(new ConnectionStrings
        {
            Postgres = connectionStrings["Postgres"] ?? string.Empty,
            Redis = connectionStrings["Redis"] ?? string.Empty
        });

        services.AddDbContext<ExchangeRatesDbContext>(options => options.UseNpgsql(connectionStrings["Postgres"]));

        var externalApiOptions = context.Configuration.GetSection("ExternalApi");
        services.AddSingleton<IExternalApiOptions>(new ExternalApiOptions
        {
            Url = externalApiOptions["url"] ?? string.Empty,
            ApiKey = externalApiOptions["key"] ?? string.Empty
        });

        services.AddHttpClient();

        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddTransient<IFetcherService, FetcherService>();
        services.AddTransient<IExchangeRateRepository, ExchangeRateRepository>();

        services.AddHostedService<FetcherService>();
    })
    .Build();

await builder.RunAsync();