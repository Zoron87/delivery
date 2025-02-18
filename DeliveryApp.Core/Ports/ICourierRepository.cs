using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository
    {
        Task<UnitResult<Error>> AddAsync(Courier courier);
        UnitResult<Error> Update(Courier courier);
        Task<Result<Courier, Error>> GetByIdAsync(Guid courierId);
        Task<IEnumerable<Courier>> GetAllFreeAsync();
    }
}
