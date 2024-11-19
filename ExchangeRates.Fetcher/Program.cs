using ExchangeRates.Fetcher.Interfaces;
using ExchangeRates.Fetcher.Repositories;
using ExchangeRates.Fetcher.Services;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;

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
            Postgres = connectionStrings["Postgres"] ?? "",
            Redis = connectionStrings["Redis"] ?? ""
        });
        services.Configure<ExternalApiOptions>(context.Configuration.GetSection("ExternalApi"));

        services.AddTransient<IHttpClientService, HttpClientService>();
        services.AddTransient<IFetcherService, FetcherService>();
        services.AddTransient<IExchangeRateRepository, ExchangeRateRepository>(); 
    })
    .Build();

await builder.RunAsync();