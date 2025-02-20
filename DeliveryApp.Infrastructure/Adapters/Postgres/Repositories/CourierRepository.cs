using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository : ICourierRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CourierRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<UnitResult<Error>> AddAsync(Courier courier)
        {
            if (courier.Transport != null) _dbContext.Attach(courier.Transport);

            await _dbContext.Couriers.AddAsync(courier);
            return UnitResult.Success<Error>(); 
        }

        public async Task<ICollection<Courier>> GetAllFreeAsync() =>
            await _dbContext.Couriers.Include(c => c.Transport).Where(c => c.Status.Name == CourierStatus.Free.Name).ToListAsync();

        public async Task<Maybe<Courier>> GetByIdAsync(Guid courierId) =>
            await _dbContext.Couriers.Include(c => c.Transport).Where(c => c.Id == courierId).SingleOrDefaultAsync(); 

        public UnitResult<Error> Update(Courier courier)
        {
            if (courier.Transport != null) _dbContext.Attach(courier.Transport);

            _dbContext.Couriers.Update(courier);
            return Result.Success<Error>();
        }
    }
}
