using ExchangeRates.Api.Interfaces;
using ExchangeRates.Shared.Interfaces;
using ExchangeRates.Shared.Models;
using Npgsql;

namespace ExchangeRates.Api.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        public ExchangeRateRepository(IConnectionStrings connectionStrings)
        {
            _connectionString = connectionStrings.Postgres;
        }

        private readonly string _connectionString;

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync(int page, int pageSize)
        {
            var rates = new List<ExchangeRate>();

            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var offset = (page - 1) * pageSize;
            var command = new NpgsqlCommand(@"
                SELECT DISTINCT ON (Currency) Currency, Rate, Date 
                FROM ExchangeRates 
                ORDER BY Currency, Date DESC
                LIMIT @Limit OFFSET @Offset", connection);

            command.Parameters.AddWithValue("@Limit", pageSize);
            command.Parameters.AddWithValue("@Offset", offset);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rates.Add(new ExchangeRate
                {
                    CurrencyCode = reader.GetString(0),
                    Rate = reader.GetDouble(1),
                    Date = reader.GetDateTime(2)
                });
            }

            return rates;
        }

        public async Task<ExchangeRate?> GetLatestExchangeRateAsync(string currencyCode)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await using var command = new NpgsqlCommand(@"
                    SELECT Currency, Rate, Date
                    FROM ExchangeRates
                    WHERE Currency = @Currency
                    ORDER BY Date DESC
                    LIMIT 1", connection);

                command.Parameters.AddWithValue("@Currency", currencyCode.ToUpperInvariant());

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new ExchangeRate
                    {
                        CurrencyCode = reader.GetString(0),
                        Rate = reader.GetDouble(1),
                        Date = reader.GetDateTime(2)
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return null;
        }

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

        public async Task DeleteExchangeRateAsync(string currencyCode)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            try
            {
                await using var command = new NpgsqlCommand(@"
                    DELETE FROM ExchangeRates
                    WHERE Currency = @Currency
                    AND Date = (
                        SELECT MAX(Date)
                        FROM ExchangeRates
                        WHERE Currency = @Currency
                    )", connection);

                command.Parameters.AddWithValue("@Currency", currencyCode.ToUpperInvariant());

                var affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new KeyNotFoundException($"Exchange rate for currency '{currencyCode}' not found.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
