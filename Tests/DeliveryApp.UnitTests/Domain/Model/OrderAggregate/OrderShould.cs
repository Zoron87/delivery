using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate
{
    public class OrderShould
    {
        public static IEnumerable<object[]> GetIncorrectOrderParams()
        {
            yield return [Guid.Empty, Location.MinPoint];
            yield return [Guid.NewGuid(), null];
        }
        [Fact]
        public void ShouldCreateOrderWhenCorrectParams()
        {
            //Arrange
            var id = Guid.NewGuid();

            //Act
            var result = Order.Create(id, Location.MinPoint);

            //Arrange
            result.IsSuccess.Should().BeTrue();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Location.Should().Be(Location.MinPoint);
        }

        [Theory]
        [MemberData(nameof(GetIncorrectOrderParams))]
        public void ShouldReturnErrorWhenIncorrectParams(Guid id, Location location)
        {
            // Act
            var result = Order.Create(id, location);

            //Assert
            result.Error.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void ShouldAssignToCourierWhenParamsIsCorrect()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;
            var courier = Courier.Create("Test", Transport.Pedestrian, Location.MinPoint).Value;

            //Act
            var result = order.AssignCourier(courier);

            //Assert
            result.IsSuccess.Should().BeTrue();
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Assigned);
        }

        [Fact]
        public void ShouldCompleteOrderWhenCorrectParams()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;
            var courier = Courier.Create("Test", Transport.Pedestrian, Location.MinPoint).Value;
            order.AssignCourier(courier);

            //Act
            var result = order.Completed();

            //Assert
            result.IsSuccess.Should().BeTrue();
            order.CourierId.Should().Be(courier.Id);
            order.Status.Should().Be(OrderStatus.Completed);
        }
    }
}
