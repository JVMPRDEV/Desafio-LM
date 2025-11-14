using NHibernate;
using LM.Orders.Domain.Interfaces;
using DomainOrder = LM.Orders.Domain.Aggregates.OrderAggregate.Order;

namespace LM.Orders.Infrastructure.Repositories
{
    public class OrderRepository(ISession session) : IOrderRepository
    {
        private readonly ISession _session = session;

        public async Task AddAsync(DomainOrder order)
        {
            await _session.SaveAsync(order);
        }

        public async Task<DomainOrder> GetByIdAsync(Guid id)
        {
            return await _session.GetAsync<DomainOrder>(id);
        }

        public async Task UpdateAsync(DomainOrder order)
        {
            await _session.UpdateAsync(order);
        }
    }
}