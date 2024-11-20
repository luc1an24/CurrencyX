# Stage 1: Build shared library
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS shared
WORKDIR /src
COPY ./ExchangeRates.Shared ./ExchangeRates.Shared
RUN dotnet build ./ExchangeRates.Shared/ExchangeRates.Shared.csproj -c Release

# Stage 2: Build and publish Fetcher
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./ExchangeRates.Fetcher ./ExchangeRates.Fetcher
COPY --from=shared /src/ExchangeRates.Shared ./ExchangeRates.Shared
RUN dotnet restore ./ExchangeRates.Fetcher/ExchangeRates.Fetcher.csproj
RUN dotnet publish ./ExchangeRates.Fetcher/ExchangeRates.Fetcher.csproj -c Release -o /out

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "ExchangeRates.Fetcher.dll"]

# Expose any necessary debugging ports
EXPOSE 5001