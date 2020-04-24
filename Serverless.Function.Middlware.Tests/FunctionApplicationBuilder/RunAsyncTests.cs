using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serverless.Function.Middleware.Abstractions;
using Shouldly;
using Xunit;

namespace Serverless.Function.Middleware.Tests.FunctionApplicationBuilder
{
    public class RunAsyncTests : FunctionApplicationBuilderSpec
    {
        [Fact]
        public async Task RunAsyncShouldThrowArgumentNullExceptionWhenUseFunctionNotCalled()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => sut.RunAsync(null));

            // Assert
            result.Message.ShouldContain("Call UseFunction method");
        }

        [Fact]
        public async Task RunAsyncShouldExecuteMiddlewareInOrderWhenCalled()
        {
            // Arrange
            var objectList = new List<object>();
            _serviceProvider.Setup(x => x.GetService(typeof(AddDateTimeMiddleware)))
                .Returns(new AddDateTimeMiddleware(objectList));
            _serviceProvider.Setup(x => x.GetService(typeof(AddNumberMiddleware)))
                .Returns(new AddNumberMiddleware(objectList));

            // Act
            var sut = CreateSut();
            sut.UseMiddleware<AddDateTimeMiddleware>();
            sut.UseMiddleware<AddNumberMiddleware>();
            sut.UseFunction(async () => new OkObjectResult("success"));
            await sut.RunAsync(null);

            // Assert
            objectList.First().ShouldBeOfType<DateTime>();
            objectList.Skip(1).First().ShouldBeOfType<int>();
        }

        [Fact]
        public async Task RunAsyncShouldReturnIActionResultWhenCalled()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            sut.UseFunction(async () => new OkObjectResult("success"));
            var result = await sut.RunAsync(null);

            // Assert
            result.ShouldBeOfType<IActionResult>();
        }

        private class AddDateTimeMiddleware : IFunctionMiddleware
        {
            private readonly IList<object> _theList;

            public AddDateTimeMiddleware(IList<object> theList)
            {
                _theList = theList;
            }

            public async Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next)
            {
                _theList.Add(DateTime.Now);
                await next(httpContext);
            }
        }

        private class AddNumberMiddleware : IFunctionMiddleware
        {
            private readonly IList<object> _theList;

            public AddNumberMiddleware(IList<object> theList)
            {
                _theList = theList;
            }

            public async Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next)
            {
                _theList.Add(1);
                await next(httpContext);
            }
        }
    }
}
