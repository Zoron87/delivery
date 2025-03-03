using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application;
public class CreateOrderCommandShould
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IGeoClient _geoClient;
    public CreateOrderCommandShould(IGeoClient geoClient)
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _geoClient = geoClient;
    }

    private Order ExistedOrder()
    {
        return Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value).Value;
    }


    [Fact]
    public async Task ShouldReturnTrueWhenOrderExists()
    {
        //Arrange
        _orderRepositoryMock.GetByIdAsync(Arg.Any<Guid>()).Returns(await Task.FromResult(ExistedOrder()));
        _unitOfWorkMock.SaveChangesAsync().Returns(Task.FromResult(true));

        //Act
        var command = new CreateOrderCommand(Guid.NewGuid(), "Test Street");
        var handler = new CreateOrderCommandHandler(_orderRepositoryMock, _unitOfWorkMock, _geoClient);
        var result = await handler.Handle(command, new CancellationToken());

        // Assert
        _orderRepositoryMock.Received(1);
        _unitOfWorkMock.Received(1);
        result.IsSuccess.Should().BeTrue();
    }
}