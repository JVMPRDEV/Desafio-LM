using MediatR;

namespace LM.Orders.Domain.Events
{
    public class OrderCreatedEvent(Guid orderId, Guid customerId, decimal totalAmount, DateTime createdDate, Guid createdByUserId) : INotification
    {
        public Guid OrderId { get; private set; } = orderId;
        public Guid CustomerId { get; private set; } = customerId;
        public decimal TotalAmount { get; private set; } = totalAmount;
        public DateTime CreatedDate { get; private set; } = createdDate;
        public Guid CreatedByUserId { get; private set; } = createdByUserId;
    }
}