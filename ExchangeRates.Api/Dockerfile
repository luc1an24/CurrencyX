# Stage 1: Build shared library
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS shared
WORKDIR /src
COPY ./ExchangeRates.Shared ./ExchangeRates.Shared
RUN dotnet build ./ExchangeRates.Shared/ExchangeRates.Shared.csproj -c Release

# Stage 2: Build and publish API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./ExchangeRates.Api ./ExchangeRates.Api
COPY --from=shared /src/ExchangeRates.Shared ./ExchangeRates.Shared
RUN dotnet restore ./ExchangeRates.Api/ExchangeRates.Api.csproj
RUN dotnet publish ./ExchangeRates.Api/ExchangeRates.Api.csproj -c Release -o /out

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "ExchangeRates.Api.dll"]
