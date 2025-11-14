// C:\Desafios\LM\LM.Orders\source\Services\LM.Orders.Infrastructure\Database\Repositories\OrderItemRepository.cs

using MongoDB.Driver;
using LM.Orders.Domain.Interfaces;
using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.Orders.Infrastructure.Projections; // Novo namespace

namespace LM.Orders.Infrastructure.Repositories
{
    // A coleção agora armazena a Projeção
    public class OrderItemRepository(IMongoDatabase database) : IOrderItemRepository
    {
        private readonly IMongoCollection<OrderItemProjection> _collection = database.GetCollection<OrderItemProjection>("OrderItems");

        public async Task AddRangeAsync(IEnumerable<OrderItem> items)
        {
            var projections = items.Select(i => new OrderItemProjection(i));
            await _collection.InsertManyAsync(projections);
        }

        public async Task<List<OrderItem>> GetItemsByOrderIdAsync(Guid orderId)
        {
            var filter = Builders<OrderItemProjection>.Filter.Eq(i => i.OrderId, orderId);

            var projections = await _collection.Find(filter).ToListAsync();

            return projections.Select(p => p.ToDomain()).ToList();
        }
    }
}