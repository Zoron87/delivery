using Confluent.Kafka;
using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using OrderStatusChanged;
using Primitives.Extensions;
using System.Text.Json;

namespace DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged
{
    /// <summary>
    ///  Producer для Kafka
    /// </summary>
    public sealed class Producer : IMessageBusProducer
    {
        private readonly ProducerConfig _config;
        private readonly string _topicName;
        public Producer(IOptions<Settings> options)
        {
            if (string.IsNullOrWhiteSpace(options.Value.MessageBrokerHost)) throw new ArgumentException(nameof(options.Value.MessageBrokerHost));

            _config = new ProducerConfig
            {
                BootstrapServers = options.Value.MessageBrokerHost
            };
            _topicName = options.Value.OrderStatusChangedTopic;
        }

        public async Task Publish(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            // Перекладываем данные из DomainEvent в IntergrationEvent
            var orderStatusConfirmedIntegrationEvent = new OrderStatusChangedIntegrationEvent
            {
                OrderId = notification.Id.ToString(),
                OrderStatus = notification.Status.ToEnum<OrderStatus>()
            };

            // Создаем сообщение для Kafka
            var message = new Message<string, string>
            {
                Key = notification.EventId.ToString(),
                Value = JsonSerializer.Serialize(orderStatusConfirmedIntegrationEvent)
            };

            try
            {
                // Отправляем сообщение в Kafka
                using var producer = new ProducerBuilder<string, string>(_config).Build();
                var dr = await producer.ProduceAsync(_topicName, message, cancellationToken).ConfigureAwait(false);
                Console.WriteLine($"Delivered '{dr.Value}' to {dr.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        
    }
}
