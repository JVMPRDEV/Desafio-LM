using LM.Orders.Domain.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace LM.Orders.Infrastructure.Services
{
    public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer) : ICacheService
    {
        private readonly IDatabase _cache = connectionMultiplexer.GetDatabase();

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _cache.StringGetAsync(key);

            if (value.IsNull)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value.ToString()!, JsonSerializerOptions);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = JsonSerializer.Serialize(value, JsonSerializerOptions);
            await _cache.StringSetAsync(key, serializedValue, expiry);
        }
    }
}