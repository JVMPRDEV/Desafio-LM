using Confluent.Kafka;
using System.Text.Json;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Infrastructure.MessageBus
{
    public class EventBusPublisher(ProducerConfig config) : IEventBusPublisher
    {
        private readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(config).Build();
        private const string TopicName = "order_created";

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            var message = JsonSerializer.Serialize(@event);
            await _producer.ProduceAsync(TopicName, new Message<Null, string> { Value = message });
        }
    }
}