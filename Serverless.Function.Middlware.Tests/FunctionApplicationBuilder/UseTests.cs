using System;
using System.Collections.Generic;
using System.Reflection;
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
            var componentsField = sut.GetType().GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
            var components = componentsField.GetValue(sut) as IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>>;
            components.Count.ShouldBe(1);
        }
    }
}
