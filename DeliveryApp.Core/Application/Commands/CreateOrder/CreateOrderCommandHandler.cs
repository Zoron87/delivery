using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.CreateOrder;
public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGeoClient _geoClient;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IGeoClient geoClient)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _geoClient = geoClient;
    }
    public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.BasketId);
        if (order != null) return UnitResult.Success<Error>();

        var streetOrder = await _geoClient.GetGeolocationAsync(request.Street);
        var currentOrder = Order.Create(request.BasketId, streetOrder);
        if (currentOrder.IsFailure) return currentOrder.Error;

        await _orderRepository.AddAsync(currentOrder.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return UnitResult.Success<Error>();
    }
}
