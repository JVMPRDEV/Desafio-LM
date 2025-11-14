using Confluent.Kafka;
using System.Text.Json;
using LM.Orders.Domain.Interfaces;

namespace LM.Orders.Infrastructure.Services
{
    public class KafkaEventBusPublisher(ProducerConfig config) : IEventBusPublisher
    {
        private readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(config).Build();

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            var topic = typeof(TEvent).Name;
            var message = JsonSerializer.Serialize(@event);

            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        }
    }
}