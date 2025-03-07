using DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;
using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.Commands.DomainEventHandlers
{
    public sealed class OrderStatusChangedDomainEventHandler : INotificationHandler<OrderStatusChangedDomainEvent>
    {
        private readonly IMessageBusProducer _messageBusProducer;

        public OrderStatusChangedDomainEventHandler(IMessageBusProducer messageBusProducer) =>
            _messageBusProducer = messageBusProducer;

        public async Task Handle(OrderStatusChangedDomainEvent notification, CancellationToken cancellationToken) =>
            await _messageBusProducer.Publish(notification, cancellationToken);
    }
}
