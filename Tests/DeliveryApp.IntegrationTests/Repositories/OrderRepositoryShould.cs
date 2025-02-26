using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class OrderRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _dbContext;

        private readonly PostgreSqlContainer _postreSqlContainer =
            new PostgreSqlBuilder()
                .WithImage("postgres:14.7")
                .WithDatabase("delivery")
                .WithUsername("postgres")
                .WithPassword("secret")
                .WithCleanUp(true)
                .Build();

        public async Task DisposeAsync()
        {
            await _postreSqlContainer.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _postreSqlContainer.StartAsync();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }).Options;
            _dbContext = new ApplicationDbContext(contextOptions);
            _dbContext.Database.Migrate();
        }

        [Fact]
        public async Task ShouldAddOrderAsync()
        {
            //Arrange
            var orderId = Guid.NewGuid();
            var order = Order.Create(orderId, Location.MinPoint).Value ;

            //Act
            var orderRepository = new OrderRepository(_dbContext);
            var unitOfWork = new UnitOfWork(_dbContext);

            var result = await orderRepository.AddAsync(order);
            await unitOfWork.SaveChangesAsync();

            //Assert
            result.IsSuccess.Should().BeTrue();

            var orderFromDB = await orderRepository.GetByIdAsync(orderId);
            order.Should().BeEquivalentTo(orderFromDB.Value);
        }

        [Fact]
        public async Task ShouldGetAssignedOrders()
        {
            //Arrange
            var courier1 = Courier.Create("Петя", Transport.Bicycle, Location.MinPoint).Value;
            var courier2 = Courier.Create("Вася", Transport.Bicycle, Location.MinPoint).Value;

            var order1 = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;
            var order2 = Order.Create(Guid.NewGuid(), Location.MaxPoint).Value;
            order2.AssignCourier(courier2);

            var orderRepository = new OrderRepository(_dbContext);
            await orderRepository.AddAsync(order1);
            await orderRepository.AddAsync(order2);
            await new UnitOfWork(_dbContext).SaveChangesAsync();

            //Act
            var result = await orderRepository.GetAllAssignedAsync();

            //Asset
            var orderFromDb = await orderRepository.GetByIdAsync(order2.Id);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Should().BeEquivalentTo(orderFromDb.Value);
        }

        [Fact]
        public async Task ShouldGetCreatedOrders()
        {
            //Arrange
            var courier1 = Courier.Create("Петя", Transport.Bicycle, Location.MinPoint).Value;
            var courier2 = Courier.Create("Вася", Transport.Bicycle, Location.MinPoint).Value;

            var order1 = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;
            var order2 = Order.Create(Guid.NewGuid(), Location.MaxPoint).Value;
            order2.AssignCourier(courier2);

            var orderRepository = new OrderRepository(_dbContext);
            await orderRepository.AddAsync(order1);
            await orderRepository.AddAsync(order2);
            await new UnitOfWork(_dbContext).SaveChangesAsync();

            //Act
            var result = await orderRepository.GetAllCreatedAsync();

            //Asset
            var orderFromDb = await orderRepository.GetByIdAsync(order1.Id);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Should().BeEquivalentTo(orderFromDb.Value);
        }

        [Fact]
        public async Task ShouldUpdateOrder()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value ;
            var orderRepository = new OrderRepository(_dbContext);
            var result = orderRepository.AddAsync(order);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveChangesAsync();

            // Act
            order.Completed();
            orderRepository.Update(order);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
            order.Should().BeEquivalentTo(orderFromDb.Value);
        }

        [Fact]
        public async Task ShouldGetCourierById()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;
            var orderRepository = new OrderRepository(_dbContext);

            //Act
            var result = orderRepository.AddAsync(order);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveChangesAsync();

            //Asset
            var orderFromDb = await orderRepository.GetByIdAsync(order.Id);
            order.Should().BeEquivalentTo(orderFromDb.Value);
        }
    }
}
