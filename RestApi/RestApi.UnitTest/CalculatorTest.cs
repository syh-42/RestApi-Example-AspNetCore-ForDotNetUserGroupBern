using Moq;
using RestApi.Business;
using Xunit;
using FluentAssertions;

namespace RestApi.UnitTest
{
    public class CalculatorTest
    {
        [Theory]
        [InlineData("ping", 4, 6, -2)]
        [InlineData("pong", 8, 9, 1)]
        public void Calculator_WithSomeInput_ReturnsCorrectValue(string input, int x, int y, int expected)
        {
            var mock = new Mock<IFoo>();
            mock.Setup(foo => foo.DoSomething("ping")).Returns(true);

            var calculator = new Calculator(mock.Object);
            var value = calculator.CalculateBecauseOfSomething(x, y, input);

            value.Should().Be(expected);
        }
    }
}
