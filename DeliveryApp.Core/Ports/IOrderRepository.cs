using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository
    {
        Task<UnitResult<Error>> AddAsync(Order order);
        UnitResult<Error> Update(Order order);
        Task<Result<Order, Error>> GetByIdAsync(Guid orderId);
        Task<ICollection<Order>> GetAllCreatedAsync();
        Task<ICollection<Order>> GetAllAssignedAsync();
    }
}
