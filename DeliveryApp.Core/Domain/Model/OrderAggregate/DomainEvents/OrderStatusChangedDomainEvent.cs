using Primitives;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public sealed record OrderStatusChangedDomainEvent(Order order) : DomainEvent;
