using LM.Orders.Domain.Aggregates.OrderAggregate;

namespace LM.Orders.Domain.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddRangeAsync(IEnumerable<OrderItem> items);
        Task<List<OrderItem>> GetItemsByOrderIdAsync(Guid orderId);
    }
}