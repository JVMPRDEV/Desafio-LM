using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LM.Orders.Domain.Aggregates.OrderAggregate;

namespace LM.Orders.Infrastructure.Projections
{
    public class OrderItemProjection
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId InternalId { get; set; }

        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public OrderItemProjection(OrderItem item)
        {
            OrderId = item.OrderId;
            ProductId = item.ProductId;
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
        }

        public OrderItemProjection() { }

        public OrderItem ToDomain()
        {
            return new OrderItem(OrderId, ProductId, Quantity, UnitPrice);
        }
    }
}