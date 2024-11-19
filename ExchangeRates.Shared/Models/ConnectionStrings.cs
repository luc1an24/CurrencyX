using ExchangeRates.Shared.Interfaces;

namespace ExchangeRates.Shared.Models
{
    public class ConnectionStrings : IConnectionStrings
    {
        public required string Postgres { get; set; }
        public required string Redis { get; set; }
    }
}
