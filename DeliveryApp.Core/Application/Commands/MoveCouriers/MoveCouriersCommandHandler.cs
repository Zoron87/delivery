using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.Commands.MoveCouriers;
public class MoveCouriersCommandHandler : IRequestHandler<MoveCouriersCommand, UnitResult<Error>>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    public MoveCouriersCommandHandler(ICourierRepository courierRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _courierRepository = courierRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<UnitResult<Error>> Handle(MoveCouriersCommand request, CancellationToken cancellationToken)
    {
        var assignedOrders = await _orderRepository.GetAllAssignedAsync();

        foreach (var order in assignedOrders)
        {
            if (order.CourierId == null) return GeneralErrors.NotFound();

            var currentCourierResult = await _courierRepository.GetByIdAsync(order.Id);
            if (currentCourierResult == null) return GeneralErrors.NotFound();
            var currentCourier = currentCourierResult.Value;

            if (order.Location == currentCourier.Location)
            {
                order.Completed();
                currentCourier.SetFree();
            }
            else
            {
                currentCourier.MoveStep(order.Location);
            }

            _courierRepository.Update(currentCourier);
            _orderRepository.Update(order);
        }

        await _unitOfWork.SaveChangesAsync();
        return UnitResult.Success<Error>();
    }
}
