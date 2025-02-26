using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.AssignCourier;
public sealed class AssignCourierCommandHandler : IRequestHandler<AssignCourierCommand, UnitResult<Error>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly IDispatchService _dispatchService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignCourierCommandHandler(IOrderRepository orderRepository, ICourierRepository courierRepository, IUnitOfWork unitOfWork, IDispatchService dispatchService)
    {
        _orderRepository = orderRepository;
        _courierRepository = courierRepository;
        _unitOfWork = unitOfWork;
        _dispatchService = dispatchService;
    }

    public async Task<UnitResult<Error>> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var freeOrder = (await _orderRepository.GetAllCreatedAsync()).FirstOrDefault();
        if (freeOrder == null) return GeneralErrors.NotFound();

        var couriers = (await _courierRepository.GetAllFreeAsync()).ToList();

        var suitCourier = _dispatchService.Dispatch(freeOrder, couriers);
        if (suitCourier.IsFailure) return suitCourier.Error;

        var assignCourier = freeOrder.AssignCourier(suitCourier.Value);
        if (assignCourier.IsFailure) return assignCourier.Error;

        _courierRepository.Update(suitCourier.Value);
        _orderRepository.Update(freeOrder);
        await _unitOfWork.SaveChangesAsync();
        return UnitResult.Success<Error>();
    }
}
