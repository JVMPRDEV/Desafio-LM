using LM.Orders.Domain.Entities;
using LM.Orders.Domain.Events;
using LM.SharedKernel.Enums;

namespace LM.Orders.Domain.Aggregates.OrderAggregate
{
    public class Order : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateTime OrderDate { get; private set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        protected Order() { }

        public Order(Guid customerId, IEnumerable<(Guid ProductId, int Quantity, decimal UnitPrice)> itemsData, Guid createdByUserId) : base(createdByUserId)
        {
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            OrderDate = DateTime.Now;

            foreach (var (ProductId, Quantity, UnitPrice) in itemsData)
            {
                _items.Add(new OrderItem(Id, ProductId, Quantity, UnitPrice));
            }

            CalculateTotalAmount();
            AddOrderCreatedEvent();
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = Items.Sum(item => item.Subtotal);
        }

        public void UpdateStatus(OrderStatus newStatus, Guid userId)
        {
            Status = newStatus;
            SetUpdateDate(userId);
        }

        private void AddOrderCreatedEvent()
        {
            AddDomainEvent(new OrderCreatedEvent(Id, CustomerId, TotalAmount, CreatedAt, CreatedByUserId));
        }
    }
}