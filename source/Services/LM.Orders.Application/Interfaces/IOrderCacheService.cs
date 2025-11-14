using LM.Orders.Contracts.Orders.Responses;

namespace LM.Orders.Domain.Interfaces
{
    public interface IOrderCacheService
    {
        Task<OrderResponse?> GetAsync(Guid orderId);
        Task SetAsync(OrderResponse response, TimeSpan? expiry = null);
    }
}