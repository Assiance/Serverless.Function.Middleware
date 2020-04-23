using Moq;
using Serverless.Function.Middleware.Abstractions;
using Serverless.Function.Middleware.Tests.Mocks;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Serverless.Function.Middleware.Tests.FunctionApplicationBuilder
{
    public class UseMiddlewareTests : FunctionApplicationBuilderSpec
    {
        [Fact]
        public void UseMiddlewareShouldThrowArgumentExceptionWhenDoesntImplementIFunctionMiddleware()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            var result = Assert.Throws<ArgumentException>(() => sut.UseMiddleware<NotMiddleware>());

            // Assert
            result.Message.ShouldContain($"does not implement {nameof(IFunctionMiddleware)}");
        }

        [Fact]
        public void UseMiddlewareShouldAddToComponentsWhenCalled()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            sut.UseMiddleware<MockMiddleware>();

            // Assert
            var components = GetDelegateComponents(sut);
            components.Count.ShouldBe(1);
        }

        private static IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>> GetDelegateComponents(Middleware.FunctionApplicationBuilder sut)
        {
            var componentsField = sut.GetType().GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
            var components = componentsField.GetValue(sut) as IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>>;
            return components;
        }

        [Fact]
        public async Task UseMiddlewareShouldCreateADelegateThatThrowsInvalidOperationExceptionWhenMiddlewareNotRegistered()
        {
            // Arrange
            _serviceProvider.Setup(x => x.GetService(typeof(MockMiddleware))).Returns(null);

            // Act
            var sut = CreateSut();
            sut.UseMiddleware<MockMiddleware>();

            var components = GetDelegateComponents(sut);
            var middlewareDelegate = components[0](context =>
            {
                return Task.CompletedTask;
            });

            var result = await Assert.ThrowsAsync<InvalidOperationException>(() => middlewareDelegate(null));

            // Assert
            result.Message.Contains($"No service for type {typeof(MockMiddleware)}");
        }

        [Fact]
        public async Task UseMiddlewareShouldCreateADelegateThatCallsMiddlewareWhenCalled()
        {
            // Arrange
            _serviceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(new MockMiddleware());
            int timesMiddlewareCalled = 0;

            // Act
            var sut = CreateSut();
            sut.UseMiddleware<MockMiddleware>();

            var components = GetDelegateComponents(sut);
            var middlewareDelegate = components[0](context =>
            {
                timesMiddlewareCalled += 1;
                return Task.CompletedTask;
            });
            await middlewareDelegate(null);

            // Assert
            timesMiddlewareCalled.ShouldBe(1);
        }
    }
}
