using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Npgsql;

namespace ExchangeRates.Fetcher.Repositories
{
    public class ExchangeRateRepository
    {
        public ExchangeRateRepository(IConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.Postgres;
        }

        private readonly string _connectionString;

        public async Task SaveExchangeRateAsync(ExchangeRate exchangeRate)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new NpgsqlCommand(
                "INSERT INTO ExchangeRates (Currency, Rate, Date) VALUES (@Currency, @Rate, @Date)", connection);

            command.Parameters.AddWithValue("Currency", exchangeRate.CurrencyCode);
            command.Parameters.AddWithValue("Rate", exchangeRate.Rate);
            command.Parameters.AddWithValue("Date", exchangeRate.Date);

            await command.ExecuteNonQueryAsync();
        }
    }
}
