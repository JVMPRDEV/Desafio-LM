using MediatR;
using LM.Orders.Domain.Events;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Application.EventHandlers
{
    public class OrderCreatedEventHandler(IEventBusPublisher eventBusPublisher) : INotificationHandler<OrderCreatedEvent>
    {
        private readonly IEventBusPublisher _eventBusPublisher = eventBusPublisher;

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _eventBusPublisher.PublishAsync(notification);
        }
    }
}