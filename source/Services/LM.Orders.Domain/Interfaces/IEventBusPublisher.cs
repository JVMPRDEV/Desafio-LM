namespace LM.Orders.Domain.Interfaces
{
    public interface IEventBusPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}