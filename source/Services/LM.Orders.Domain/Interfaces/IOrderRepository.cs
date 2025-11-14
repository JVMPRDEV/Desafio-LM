using LM.Orders.Domain.Aggregates.OrderAggregate;

namespace LM.Orders.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(Guid id);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
    }
}