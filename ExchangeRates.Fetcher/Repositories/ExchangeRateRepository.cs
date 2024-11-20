using ExchangeRates.Fetcher.Interfaces;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Npgsql;

namespace ExchangeRates.Fetcher.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
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
                "INSERT INTO ExchangeRates (Currency, Rate, Date) " +
                "SELECT @Currency, @Rate, @Date " +
                "WHERE NOT EXISTS (" +
                "SELECT 1 FROM ExchangeRates WHERE Currency = @Currency AND Date = @Date" +
                ")", connection);

            command.Parameters.AddWithValue("Currency", exchangeRate.CurrencyCode);
            command.Parameters.AddWithValue("Rate", exchangeRate.Rate);
            command.Parameters.AddWithValue("Date", exchangeRate.Date);

            await command.ExecuteNonQueryAsync();
        }
    }
}
