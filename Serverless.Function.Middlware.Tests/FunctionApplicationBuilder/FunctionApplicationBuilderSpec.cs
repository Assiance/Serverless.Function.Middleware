using System;
using System.Collections.Generic;
using System.Reflection;
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

        protected static IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>> GetDelegateComponents(Middleware.FunctionApplicationBuilder sut)
        {
            var componentsField = sut.GetType().GetField("_components", BindingFlags.NonPublic | BindingFlags.Instance);
            var components = componentsField.GetValue(sut) as IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>>;
            return components;
        }

        protected static FunctionRequestDelegate GetDelegateMainFunctionDelegate(Middleware.FunctionApplicationBuilder sut)
        {
            var mainFunctionDelegateField = sut.GetType().GetField("_mainFunctionDelegate", BindingFlags.NonPublic | BindingFlags.Instance);
            var mainFunctionDelegate = mainFunctionDelegateField.GetValue(sut) as FunctionRequestDelegate;
            return mainFunctionDelegate;
        }
    }
}
