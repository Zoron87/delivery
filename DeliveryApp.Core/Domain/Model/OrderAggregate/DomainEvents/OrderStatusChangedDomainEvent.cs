using Primitives;
using System.Security;

namespace DeliveryApp.Core.Domain.Model.OrderAggregate.DomainEvents;

public sealed record OrderStatusChangedDomainEvent(Guid Id, string Status) : DomainEvent;
