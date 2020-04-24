using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Xunit;

namespace Serverless.Function.Middleware.Tests.FunctionApplicationBuilder
{
    public class UseFunctionTests : FunctionApplicationBuilderSpec
    {
        [Fact]
        public void UseFunctionShouldAddToMainFunctionDelegateWhenCalled()
        {
            // Arrange
            // Act
            var sut = CreateSut();
            sut.UseFunction(async () => new OkObjectResult(""));

            // Assert
            var mainFunctionDelegate = GetDelegateMainFunctionDelegate(sut);
            mainFunctionDelegate.ShouldNotBeNull();
        }
    }
}
