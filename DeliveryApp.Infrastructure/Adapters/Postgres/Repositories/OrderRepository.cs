using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext)); ;
        }

        public async Task<UnitResult<Error>> AddAsync(Order order)
        {
            if (order == null) return GeneralErrors.ValueIsRequired(nameof(order));

            await _dbContext.Orders.AddAsync(order);
            return Result.Success<Error>();
        }

        public async Task<IEnumerable<Order>> GetAllCreatedAsync()
        {
            return await _dbContext.Orders.Where(o => o.Status.Name == OrderStatus.Created.Name).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllAssignedAsync()
        {
            var test = await _dbContext.Orders.Where(o => o.Status.Name == OrderStatus.Assigned.Name).ToListAsync();
            return test;
        }

        public async Task<Result<Order, Error>> GetByIdAsync(Guid orderId)
        {
            return await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == orderId);
        }

        public UnitResult<Error> Update(Order order)
        {
            _dbContext.Orders.Update(order);
            return Result.Success<Error>();
        }
    }
}
