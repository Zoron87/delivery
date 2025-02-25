﻿using CSharpFunctionalExtensions;
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

        public async Task<ICollection<Order>> GetAllCreatedAsync() =>
            await _dbContext.Orders.Where(o => o.Status.Name == OrderStatus.Created.Name).ToListAsync();

        public async Task<ICollection<Order>> GetAllAssignedAsync() =>
            await _dbContext.Orders.Where(o => o.Status.Name == OrderStatus.Assigned.Name).ToListAsync();

        public async Task<Maybe<Order>> GetByIdAsync(Guid orderId) =>
            await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == orderId);

        public UnitResult<Error> Update(Order order)
        {
            _dbContext.Orders.Update(order);
            return Result.Success<Error>();
        }
    }
}
