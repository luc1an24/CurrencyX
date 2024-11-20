namespace ExchangeRates.Shared.Interfaces
{
    public interface IExternalApiOptions
    {
        string Url { get; set; }
        string ApiKey { get; set; }
    }
}
