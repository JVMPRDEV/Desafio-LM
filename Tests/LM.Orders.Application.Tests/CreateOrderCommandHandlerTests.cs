using Moq;
using MediatR;
using LM.Orders.Application.CommandHandlers;
using LM.Orders.Contracts.Orders.Commands;
using LM.Orders.Domain.Interfaces;
using LM.Orders.Domain.Services;
using LM.SharedKernel.Dtos;
using LM.Orders.Domain.Events;
using LM.Orders.Domain.Aggregates.OrderAggregate;

namespace LM.Orders.Application.Tests
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IEventBusPublisher> _eventBusPublisherMock;
        private readonly Mock<OrderDomainService> _domainServiceMock;
        private readonly Mock<IOrderItemRepository> _orderItemRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _eventBusPublisherMock = new Mock<IEventBusPublisher>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _domainServiceMock = new Mock<OrderDomainService>(_orderRepositoryMock.Object);
            _orderItemRepositoryMock = new Mock<IOrderItemRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new CreateOrderCommandHandler(
                _eventBusPublisherMock.Object,
                _domainServiceMock.Object,
                _orderItemRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _mediatorMock.Object 
            );
        }

        [Fact]
        public async Task Handle_ShouldCompleteTransactionAndPublishEvent_WhenOrderIsCreated()
        {
            var userId = new Guid("1b3d1b82-9e9f-4b0d-b8e7-0a4a8d0f8d0f");
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items =
                [
                    new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = 2, UnitPrice = 100.00m }
                ]
            };
            command.SetCreatedByUserId(userId);

            _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
            _orderItemRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<OrderItem>>())).Returns(Task.CompletedTask);

            var response = await _handler.Handle(command, CancellationToken.None);

            Assert.NotNull(response);
            Assert.NotEqual(Guid.Empty, response.Id);

            _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
            _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
            _orderItemRepositoryMock.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<OrderItem>>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldRollbackTransaction_WhenMongoPersistenceFails()
        {
            var userId = new Guid("1b3d1b82-9e9f-4b0d-b8e7-0a4a8d0f8d0f");
            var command = new CreateOrderCommand
            {
                CustomerId = Guid.NewGuid(),
                Items =
                [
                    new CreateOrderItemDto { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 50.00m }
                ]
            };
            command.SetCreatedByUserId(userId);

            _orderItemRepositoryMock.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<OrderItem>>()))
                .ThrowsAsync(new Exception("Mongo connection failed"));

            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));

            _unitOfWorkMock.Verify(u => u.BeginTransaction(), Times.Once);
            _unitOfWorkMock.Verify(u => u.RollbackAsync(), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never); 
        }
    }
}