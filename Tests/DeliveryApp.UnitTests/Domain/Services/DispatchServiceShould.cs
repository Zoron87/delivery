using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.OrderAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatchServiceShould
    {
        public static IEnumerable<object[]> GetCouriers()
        {
            yield return ["TestPedestrian", Transport.Pedestrian, Location.MaxPoint];
            yield return ["TestBicycle", Transport.Bicycle, Location.MaxPoint];
            yield return ["TestCar", Transport.Car, Location.MaxPoint];
        }

        [Theory]
        [MemberData(nameof(GetCouriers))]
        public void ShouldFastCourierWhenCorrectParams(string name, Transport transport, Location location)
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;

            var couriers = new List<Courier>()
            {
               Courier.Create("TestPedestrian", Transport.Pedestrian, Location.MaxPoint).Value,
               Courier.Create("TestBicycle", Transport.Bicycle, Location.MaxPoint).Value,
               Courier.Create("TestCar", Transport.Car, Location.MaxPoint).Value
            };

            //Act
            var fastCourier = new DispatchService().Dispatch(order, couriers);

            //Assert
            fastCourier.IsSuccess.Should().BeTrue();
            fastCourier.Value.Name.Should().Be("TestCar");
        }

        [Fact]
        public void ShoudReturnErrorWhenOrderNull()
        {
            //Arrange
            var couriers = new List<Courier>()
            {
               Courier.Create("TestPedestrian", Transport.Pedestrian, Location.MaxPoint).Value,
               Courier.Create("TestBicycle", Transport.Bicycle, Location.MaxPoint).Value,
               Courier.Create("TestCar", Transport.Car, Location.MaxPoint).Value
            };

            //Act
            var result = new DispatchService().Dispatch(null, couriers);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void ShoudReturnErrorWhenCouriersNull()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), Location.MinPoint).Value;

            //Act
            var result = new DispatchService().Dispatch(order, null);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
        }
    }
}
