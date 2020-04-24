using Shouldly;
using Xunit;

namespace Serverless.Function.Middleware.Tests.FunctionApplicationBuilder
{
    public class UseTests : FunctionApplicationBuilderSpec
    {
        [Fact]
        public void UseShouldAddToComponentsWhenCalled()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            sut.Use((context) => null);

            // Assert
            var components = GetDelegateComponents(sut);
            components.Count.ShouldBe(1);
        }

        [Fact]
        public void UseShouldAddTwoComponentsWhenCalledTwice()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            sut.Use((context) => null);
            sut.Use((context) => null);

            // Assert
            var components = GetDelegateComponents(sut);
            components.Count.ShouldBe(2);
        }
    }
}
