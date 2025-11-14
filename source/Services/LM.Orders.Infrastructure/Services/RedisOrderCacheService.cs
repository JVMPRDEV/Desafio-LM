using LM.Orders.Contracts.Orders.Responses;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Infrastructure.Services
{
    public class RedisOrderCacheService(ICacheService cacheService) : IOrderCacheService
    {
        private readonly ICacheService _cacheService = cacheService;
        private const string CacheKeyPrefix = "order:";

        public async Task<OrderResponse?> GetAsync(Guid orderId)
        {
            var key = $"{CacheKeyPrefix}{orderId}";
            return await _cacheService.GetAsync<OrderResponse>(key);
        }

        public async Task SetAsync(OrderResponse response, TimeSpan? expiry = null)
        {
            var key = $"{CacheKeyPrefix}{response.Id}";
            await _cacheService.SetAsync(key, response, expiry);
        }
    }
}