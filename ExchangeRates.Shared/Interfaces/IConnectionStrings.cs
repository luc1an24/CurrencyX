namespace ExchangeRates.Shared.Interfaces
{
    public interface IConnectionStrings
    {
        string Postgres { get; set; }
        string Redis { get; set; }
    }
}
