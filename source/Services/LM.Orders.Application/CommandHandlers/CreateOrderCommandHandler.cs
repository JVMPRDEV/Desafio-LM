using MediatR;
using LM.Orders.Contracts.Orders.Commands;
using LM.Orders.Domain.Aggregates.OrderAggregate;
using LM.Orders.Domain.Interfaces;
using LM.Orders.Domain.Services;
using LM.Orders.Contracts.Orders.Responses;

namespace LM.Orders.Application.CommandHandlers
{
    public class CreateOrderCommandHandler(IEventBusPublisher eventBusPublisher, OrderDomainService domainService, IOrderItemRepository orderItemRepository, IUnitOfWork unitOfWork, IOrderRepository orderRepository, IMediator mediator) : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
    {
        private readonly IEventBusPublisher _eventBusPublisher = eventBusPublisher;
        private readonly OrderDomainService _domainService = domainService;
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMediator _mediator = mediator;

        public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            _unitOfWork.BeginTransaction();

            try
            {
                var itemsData = request.Items.Select(i => (i.ProductId, i.Quantity, i.UnitPrice));

                var order = new Order(request.CustomerId, itemsData, request.CreatedByUserId);

                _domainService.EnsureOrderIsValid(order);

                await _orderRepository.AddAsync(order);

                await _orderItemRepository.AddRangeAsync(order.Items);

                var domainEvents = order.DomainEvents.ToList();
                order.ClearDomainEvents();
                foreach (var domainEvent in domainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                await _unitOfWork.CommitAsync();

                return new CreateOrderResponse(order.Id);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}