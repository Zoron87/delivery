using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class TransportShould
    {
        public static IEnumerable<object[]> GetTransports()
        {
            yield return [Transport.Pedestrian, 1, nameof(Transport.Pedestrian).ToString().ToLowerInvariant(), 1];
            yield return [Transport.Bicycle, 2, nameof(Transport.Bicycle).ToString().ToLowerInvariant(), 2];
            yield return [Transport.Car, 3, nameof(Transport.Car).ToString().ToLowerInvariant(), 3];
        }

        [Theory]
        [MemberData(nameof(GetTransports))]
        public static void ShouldReturnCorrectTransportParams(Transport transportType, int id, string name, int speed)
        {
            //Assert
            transportType.Id.Should().Be(id);
            transportType.Name.Should().Be(name);
            transportType.Speed.Should().Be(speed);
        }

        [Theory]
        [MemberData(nameof(GetTransports))]
        public void ShouldFoundFromId(Transport transportType, int id, string name, int speed)
        {
            //Act
            var result = Transport.FromId(id).Value;

            //Arrange
            result.Should().Be(transportType);
            result.Id.Should().Be(id);
            result.Name.Should().Be(name);
            result.Speed.Should().Be(speed);
        }

        [Theory]
        [MemberData(nameof(GetTransports))]
        public void ShouldFoundByName(Transport transportType, int id, string name, int speed)
        {
            //Act
            var result = Transport.FromName(name).Value;

            //Arrange
            result.Should().Be(transportType);
            result.Id.Should().Be(id);
            result.Name.Should().Be(name);
            result.Speed.Should().Be(speed);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void ShouldReturnErrorWhenTransportNotFoundById(int id)
        {
            //Act
            var result = Transport.FromId(id);

            //Arrange
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Theory]
        [InlineData("TestTransport")]
        [InlineData("Bicycle1")]
        [InlineData("Bicycl")]
        public static void ShouldReturnErrorWhenTransportNotFoundByName(string name)
        {
            //Act
            var result = Transport.FromName(name);

            //Arrange
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public static void ShouldReturnListOfTransports()
        {
            //Act
            var result = Transport.List();

            //Arrange
            result.Should().NotBeNull();
        }
    }
}
