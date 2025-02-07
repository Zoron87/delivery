using DeliveryApp.Core.Domain.Model.CourierAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierStatusShould
    {
        [Fact]
        public static void ShouldNotEqualWhenAllPropertiesIsEqual()
        {
            //Act
            var result = CourierStatus.Busy == CourierStatus.Free;

            //Arrange
            result.Should().BeFalse();
        }

        [Fact]
        public static void ShouldEqualWhenAllPropertiesIsEqual()
        {
            //Act
            var result = CourierStatus.Free == CourierStatus.Free;

            //Arrange
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("free")]
        public void ShouldReturnCorrectName(string name)
        {
            CourierStatus.Free.Name.ToLower().Should().Be("free");
            CourierStatus.Busy.Name.ToLower().Should().Be("busy");
        }
    }
}
