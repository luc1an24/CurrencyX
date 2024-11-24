using ExchangeRates.Api.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace ExchangeRates.Api.Services
{
    public class RedisCacheService : ICacheService
    {
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        private readonly IConnectionMultiplexer _redis;

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var json = await db.StringGetAsync(key);

            if (json.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(json!);
        }

        public async Task RemoveAsync(string key)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

            foreach (var key in keys)
            {
                await RemoveAsync(key.ToString());
            }
        }
    }
}
