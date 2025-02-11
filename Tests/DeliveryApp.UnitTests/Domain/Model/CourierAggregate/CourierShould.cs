using DeliveryApp.Core.Domain.Model.CourierAggregate;
using DeliveryApp.Core.Domain.Model.SharedKernel;
using FluentAssertions;
using Primitives;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierShould
    {
        [Fact]
        public void ShouldBeCorrentWhenParamsCorrect()
        {
            //Act
            var result = Courier.Create("Иван", Transport.Pedestrian, Location.MinPoint);

            //Arrange
            result.Should().NotBeNull();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Name.Should().Be("Иван");
            result.Value.Transport.Should().Be(Transport.Pedestrian);
        }

        [Fact]
        public void ShouldErrorWhenParamIsNotCorrect()
        {
            //Assert
            Transport transport = null;

            //Act
            var result = Courier.Create("Иван", transport, Location.MinPoint);

            //Arrange
            result.Error.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeEquivalentTo(GeneralErrors.ValueIsRequired(nameof(transport)));
        }

        public void ShouldNotMoveIncorrectLocation()
        {
            //Assert
            Location finalLocation = null;
            var courier = Courier.Create("Test", Transport.Pedestrian, finalLocation).Value;

            //Act
            var result = courier.MoveStep(finalLocation);

            //Arrange
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().BeEquivalentTo(GeneralErrors.ValueIsRequired(nameof(finalLocation).ToLowerInvariant()));
        }

        [Theory]
        [InlineData(1, 1, 1, 1, 0)]
        [InlineData(1, 1, 1, 2, 1)]
        [InlineData(2, 2, 3, 2, 1)]
        [InlineData(2, 2, 1, 2, 1)]
        [InlineData(1, 1, 5, 10, 13)]
        public void ShoudCorrectCalculateNumberSteps(int currentX, int currentY, int finalX, int finalY, int expected)
        {
            //Asset
            var currentLocation = Location.Create(currentX, currentY);
            var finalLocation = Location.Create(finalX, finalY);
            var courier = Courier.Create("Test", Transport.Pedestrian, currentLocation.Value).Value;

            //Act
            var result = courier.GetNumberOfSteps(finalLocation.Value);

            //Arrange
            result.Value.Should().Be(expected);
        }

        public static IEnumerable<object[]> GetTransports()
        {
            // Пешеход, заказ X:совпадает, Y: совпадает
            yield return
            [
                Transport.Pedestrian, Location.Create(1, 1).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(5, 5).Value, Location.Create(5, 5).Value, Location.Create(5, 5).Value
            ];

            // Пешеход, заказ X:совпадает, Y: выше
            yield return
            [
                Transport.Pedestrian, Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(1, 1).Value, Location.Create(1, 5).Value, Location.Create(1, 2).Value
            ];

            // Пешеход, заказ X:правее, Y: совпадает
            yield return
            [
                Transport.Pedestrian, Location.Create(2, 2).Value, Location.Create(3, 2).Value, Location.Create(3, 2).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(5, 5).Value, Location.Create(6, 5).Value, Location.Create(6, 5).Value
            ];

            // Пешеход, заказ X:правее, Y: выше
            yield return
            [
                Transport.Pedestrian, Location.Create(2, 2).Value, Location.Create(3, 3).Value, Location.Create(3, 2).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(1, 1).Value, Location.Create(5, 5).Value, Location.Create(2, 1).Value
            ];

            // Пешеход, заказ X:совпадает, Y: ниже
            yield return
            [
                Transport.Pedestrian, Location.Create(1, 2).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(5, 5).Value, Location.Create(5, 1).Value, Location.Create(5, 4).Value
            ];

            // Пешеход, заказ X:левее, Y: совпадает
            yield return
            [
                Transport.Pedestrian, Location.Create(2, 2).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(5, 5).Value, Location.Create(1, 5).Value, Location.Create(4, 5).Value
            ];

            // Пешеход, заказ X:левее, Y: ниже
            yield return
            [
                Transport.Pedestrian, Location.Create(2, 2).Value, Location.Create(1, 1).Value, Location.Create(1, 2).Value
            ];
            yield return
            [
                Transport.Pedestrian, Location.Create(5, 5).Value, Location.Create(1, 1).Value, Location.Create(4, 5).Value
            ];


            // Велосипедист, заказ X:совпадает, Y: совпадает
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(5, 5).Value, Location.Create(5, 5).Value];

            // Велосипедист, заказ X:совпадает, Y: выше
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(1, 3).Value, Location.Create(1, 3).Value];
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(1, 5).Value, Location.Create(1, 3).Value];

            // Велосипедист, заказ X:правее, Y: совпадает
            yield return
                [Transport.Bicycle, Location.Create(2, 2).Value, Location.Create(4, 2).Value, Location.Create(4, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(8, 5).Value, Location.Create(7, 5).Value];

            // Велосипедист, заказ X:правее, Y: выше
            yield return
                [Transport.Bicycle, Location.Create(2, 2).Value, Location.Create(4, 4).Value, Location.Create(4, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(5, 5).Value, Location.Create(3, 1).Value];

            // Велосипедист, заказ X:совпадает, Y: ниже
            yield return
                [Transport.Bicycle, Location.Create(1, 3).Value, Location.Create(1, 1).Value, Location.Create(1, 1).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(5, 1).Value, Location.Create(5, 3).Value];

            // Велосипедист, заказ X:левее, Y: совпадает
            yield return
                [Transport.Bicycle, Location.Create(3, 2).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(1, 5).Value, Location.Create(3, 5).Value];

            // Велосипедист, заказ X:левее, Y: ниже
            yield return
                [Transport.Bicycle, Location.Create(3, 3).Value, Location.Create(1, 1).Value, Location.Create(1, 3).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(1, 1).Value, Location.Create(3, 5).Value];

            // Велосипедист, заказ ближе чем скорость
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(2, 1).Value, Location.Create(2, 1).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(5, 4).Value, Location.Create(5, 4).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(4, 5).Value, Location.Create(4, 5).Value];

            // Велосипедист, заказ с шагами по 2 осям
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(2, 2).Value, Location.Create(2, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(4, 4).Value, Location.Create(4, 4).Value];
            yield return
                [Transport.Bicycle, Location.Create(1, 1).Value, Location.Create(1, 2).Value, Location.Create(1, 2).Value];
            yield return
                [Transport.Bicycle, Location.Create(5, 5).Value, Location.Create(5, 4).Value, Location.Create(5, 4).Value];
        }


        [Theory]
        [MemberData(nameof(GetTransports))]
        public void CanMoveStep(Transport transport, Location currentLocation, Location targetLocation,
        Location locationAfterMove)
        {
            //Arrange
            var courier = Courier.Create("Ваня", transport, currentLocation).Value;

            //Act
            var result = courier.MoveStep(targetLocation);

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Location.Should().Be(locationAfterMove);
        }
    }
}
