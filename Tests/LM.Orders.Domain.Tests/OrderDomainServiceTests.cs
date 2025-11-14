using Moq;
using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.Orders.Domain.Interfaces;
using LM.Orders.Domain.Services;
using LM.SharedKernel.Enums;

namespace LM.Orders.Domain.Tests
{
    public class OrderDomainServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly OrderDomainService _domainService;
        private readonly Guid _userId = new("3fa85f64-5717-4562-b3fc-2c963f66afa6");
        private readonly Guid _customerId = Guid.NewGuid();

        public OrderDomainServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _domainService = new OrderDomainService(_orderRepositoryMock.Object);
        }

        [Fact]
        public void EnsureOrderIsValid_ShouldSetStatusToProcessing_WhenTotalAmountIsHigh()
        {
            var itemsData = new List<(Guid ProductId, int Quantity, decimal UnitPrice)>
            {
                (Guid.NewGuid(), 1, 5000.01m)
            };

            var order = new Order(_customerId, itemsData, _userId);

            order.UpdateStatus(OrderStatus.Pending, _userId);

            _domainService.EnsureOrderIsValid(order);

            Assert.Equal(OrderStatus.Processing, order.Status);
        }

        [Fact]
        public void EnsureOrderIsValid_ShouldKeepStatusAsPending_WhenOrderIsNormal()
        {
            var itemsData = new List<(Guid ProductId, int Quantity, decimal UnitPrice)>
            {
                (Guid.NewGuid(), 1, 100.00m),
                (Guid.NewGuid(), 2, 50.00m)
            };

            var order = new Order(_customerId, itemsData, _userId);

            order.UpdateStatus(OrderStatus.Pending, _userId);

            _domainService.EnsureOrderIsValid(order);

            Assert.Equal(OrderStatus.Pending, order.Status);
        }

        [Fact]
        public async Task CancelOrder_ShouldChangeStatusToCancelled_WhenStatusIsPending()
        {
            var orderId = Guid.NewGuid();
            var itemsData = new List<(Guid ProductId, int Quantity, decimal UnitPrice)>
            {
                (Guid.NewGuid(), 1, 10.00m)
            };
            var order = new Order(_customerId, itemsData, _userId);
            order.UpdateStatus(OrderStatus.Pending, _userId);

            _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

            await _domainService.CancelOrder(orderId, _userId);

            _orderRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Cancelled)), Times.Once);
        }
    }
}