using System;
using Moq;

namespace Serverless.Function.Middleware.Tests.FunctionApplicationBuilder
{
    public class FunctionApplicationBuilderSpec
    {
        protected Mock<IServiceProvider> _serviceProvider;

        public FunctionApplicationBuilderSpec()
        {
            _serviceProvider = new Mock<IServiceProvider>();
        }

        public Middleware.FunctionApplicationBuilder CreateSut()
        {
            return new Middleware.FunctionApplicationBuilder(_serviceProvider.Object);
        }
    }
}
