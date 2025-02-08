using DeliveryApp.Core.Domain.Model.OrderAggregate;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate
{
    public class OrderStatusShould
    {
        [Fact]
        public static void ShouldNotEqualWhenAllPropertiesIsEqual()
        {
            //Act
            var result = OrderStatus.Created == OrderStatus.Assigned && OrderStatus.Assigned == OrderStatus.Completed;

            //Arrange
            result.Should().BeFalse();
        }

        [Fact]
        public static void ShouldEqualWhenAllPropertiesIsEqual()
        {
            //Act
            var result = OrderStatus.Created == OrderStatus.Created;

            //Arrange
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("free")]
        public void ShouldReturnCorrectName(string name)
        {
            OrderStatus.Assigned.Name.ToLower().Should().Be("assigned");
            OrderStatus.Completed.Name.ToLower().Should().Be("completed");
            OrderStatus.Created.Name.ToLower().Should().Be("created");
        }
    }
}
