using CSharpFunctionalExtensions;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;
public sealed class CreateOrderCommand : IRequest<UnitResult<Error>>
{
    public Guid BasketId { get; }
    public string Street { get; }

    public CreateOrderCommand(Guid basketId, string street)
    {
        if (basketId == Guid.Empty) throw new Exception(nameof(basketId));
        if (string.IsNullOrWhiteSpace(street)) throw new Exception(nameof(street));

        this.BasketId = basketId;
        this.Street = street;
    }
}
