﻿namespace ExchangeRates.Api.Interfaces
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }
}
