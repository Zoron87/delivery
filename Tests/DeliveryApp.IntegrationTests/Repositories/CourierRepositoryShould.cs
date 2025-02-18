using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _dbContext;

        private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
           .WithImage("postgres:14.7")
           .WithDatabase("courier")
           .WithUsername("postgres")
           .WithPassword("secret")
           .WithCleanUp(true)
           .Build();

        public async Task DisposeAsync()
        {
            await _postgreSqlContainer.DisposeAsync().AsTask();
        }

        public async Task InitializeAsync()
        {
            await _postgreSqlContainer.StartAsync();

            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
                _postgreSqlContainer.GetConnectionString(),
                sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); }).Options;
            _dbContext = new ApplicationDbContext(contextOptions);
            _dbContext.Database.Migrate();
        }

        [Fact]
        public async Task ShouldAddCourierAsync()
        {
            //Arrange
            var courier = Courier.Create("Иван", Transport.Pedestrian, Location.MinPoint).Value;

            //Act
            var courierRepository = new CourierRepository(_dbContext);
            var unitOfWork = new UnitOfWork(_dbContext);

            var result = await courierRepository.AddAsync(courier);
            await unitOfWork.SaveChangesAsync();

            //Assert
            result.IsSuccess.Should().BeTrue();

            var courierFromDB = await courierRepository.GetByIdAsync(courier.Id);
            courier.Should().BeEquivalentTo(courierFromDB.Value);
        }

        [Fact]
        public async Task ShouldGetFreeCouriers()
        {
            //Arrange
            var courier1 = Courier.Create("Петя", Transport.Bicycle, Location.MinPoint).Value;
            courier1.SetBusy();
            var courier2 = Courier.Create("Вася", Transport.Bicycle, Location.MinPoint).Value;

            var courierRepository = new CourierRepository(_dbContext);
            await courierRepository.AddAsync(courier1);
            await courierRepository.AddAsync(courier2);
            await new UnitOfWork(_dbContext).SaveChangesAsync();

            //Act
            var result = await courierRepository.GetAllFreeAsync();

            //Asset
            var courierFromDb = await courierRepository.GetByIdAsync(courier2.Id);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.First().Should().BeEquivalentTo(courierFromDb.Value);
        }

        [Fact]
        public async Task ShouldUpdateCourier()
        {
            //Arrange
            var courier = Courier.Create("Петя", Transport.Pedestrian, Location.MinPoint).Value;
            var courierRepository = new CourierRepository(_dbContext);
            var result = courierRepository.AddAsync(courier);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveChangesAsync();

            // Act
            courier.SetBusy();
            courierRepository.Update(courier);
            await unitOfWork.SaveChangesAsync();

            //Assert
            var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);
            courierFromDb.IsSuccess.Should().BeTrue();
            courier.Should().BeEquivalentTo(courierFromDb.Value);
        }

        [Fact]
        public async Task ShouldGetCourierById()
        {
            //Arrange
            var courier = Courier.Create("Петя", Transport.Pedestrian, Location.MinPoint).Value;
            var courierRepository = new CourierRepository(_dbContext);
            
            //Act
            var result = courierRepository.AddAsync(courier);
            var unitOfWork = new UnitOfWork(_dbContext);
            await unitOfWork.SaveChangesAsync();

            //Asset
            var courierFromDb = await courierRepository.GetByIdAsync(courier.Id);
            courierFromDb.IsSuccess.Should().BeTrue();
            courier.Should().BeEquivalentTo(courierFromDb.Value);
        }
    }
}
