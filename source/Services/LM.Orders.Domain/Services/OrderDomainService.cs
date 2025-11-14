using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.Orders.Domain.Interfaces;
using LM.SharedKernel.Enums;

namespace LM.Orders.Domain.Services
{
    public class OrderDomainService(IOrderRepository orderRepository)
    {
        private readonly IOrderRepository _orderRepository = orderRepository;

        public void EnsureOrderIsValid(Order order)
        {
            const decimal HighValueThreshold = 5000.00m;
            const int MaxDistinctItems = 5;

            var isLargeOrder = order.Items.Count > MaxDistinctItems || order.TotalAmount > HighValueThreshold;

            if (isLargeOrder)
            {
                order.UpdateStatus(OrderStatus.Processing, order.CreatedByUserId);
            }
        }

        public async Task CancelOrder(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return;

            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Processing)
            {
                order.UpdateStatus(OrderStatus.Cancelled, userId);
                await _orderRepository.UpdateAsync(order);
            }
        }
    }
}