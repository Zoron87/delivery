using DeliveryApp.Core.Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationShould
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(10, 10)]
        [InlineData(7, 7)]
        public void BeCorrectWhenParamsIsCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeTrue();
            location.Value.X.Should().Be(x);
            location.Value.Y.Should().Be(y);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, -1)]
        [InlineData(11, 11)]
        public void ReturnErrorWhenParamsIsInCorrectOnCreated(int x, int y)
        {
            //Arrange

            //Act
            var location = Location.Create(x, y);

            //Assert
            location.IsSuccess.Should().BeFalse();
            location.Error.Should().NotBeNull();
        }

        [Fact]
        public void BeEqualWhenAllPropertiesIsEqual()
        {
            //Arrange
            var location1 = Location.Create(5, 5);
            var location2 = Location.Create(5, 5);

            //Act
            var result = location1.Value.X == location2.Value.X && location1.Value.Y == location2.Value.Y;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeNotEqualWhenAllPropertiesIsNotEqual()
        {
            //Arrange
            var location1 = Location.Create(5, 5);
            var location2 = Location.Create(6, 6);

            //Act
            var result = location1.Value.X != location2.Value.X && location1.Value.Y != location2.Value.Y;

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeCorreсtCalculateDistanceX()
        {
            //Arrange
            var currentLocation = Location.Create(5, 5);
            var otherLocation = Location.Create(6, 5);

            //Act
            var result = currentLocation.Value.CalculateDistance(otherLocation.Value);

            //Assert
            result.Should().Be(1);
        }

        [Fact]
        public void BeCorreсtCalculateDistanceY()
        {
            //Arrange
            var currentLocation = Location.Create(5, 5);
            var otherLocation = Location.Create(5, 7);

            //Act
            var result = currentLocation.Value.CalculateDistance(otherLocation.Value);

            //Assert
            result.Should().Be(2);
        }

        [Fact]
        public void BeCorreсtCalculateDistanceXY()
        {
            //Arrange
            var currentLocation = Location.Create(2, 6);
            var otherLocation = Location.Create(4, 9);

            //Act
            var result = currentLocation.Value.CalculateDistance(otherLocation.Value);

            //Assert
            result.Should().Be(5);
        }

        [Fact]
        public void CanCreateRandomLocation()
        {
            //Arrange

            //Act
            var location = Location.CreateRandomPoint();

            //Assert
            location.X.Should().BeGreaterThanOrEqualTo(1).And.BeLessThanOrEqualTo(10);
            location.Y.Should().BeGreaterThanOrEqualTo(1).And.BeLessThanOrEqualTo(10);
        }
    }
}
