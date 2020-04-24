using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serverless.Function.Middleware.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Serverless.Function.Middleware
{
    public class FunctionApplicationBuilder : IFunctionApplicationBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>> _components = new List<Func<FunctionRequestDelegate, FunctionRequestDelegate>>();

        private IActionResult _mainFunctionResult;
        private FunctionRequestDelegate _mainFunctionDelegate;

        public FunctionApplicationBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IServiceProvider ApplicationServices => _serviceProvider;

        public IFunctionApplicationBuilder Use(Func<FunctionRequestDelegate, FunctionRequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        public IFunctionApplicationBuilder UseMiddleware<TMiddleware>()
        {
            var middlewareType = typeof(TMiddleware);
            return UseMiddleware(middlewareType);
        }

        public IFunctionApplicationBuilder UseMiddleware(Type middlewareType)
        {
            if (!typeof(IFunctionMiddleware).GetTypeInfo().IsAssignableFrom(middlewareType.GetTypeInfo()))
            {
                throw new ArgumentException($"{middlewareType.GetTypeInfo().Name} does not implement {nameof(IFunctionMiddleware)}");
            }

            return this.Use(next =>
            {
                return async context =>
                {
                    var middleware = _serviceProvider.GetRequiredService(middlewareType) as IFunctionMiddleware;

                    try
                    {
                        await middleware.InvokeAsync(context, next);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                };
            });
        }

        public IFunctionApplicationBuilder UseFunction(Func<Task<IActionResult>> function)
        {
            FunctionRequestDelegate app = async (context) =>
            {
                _mainFunctionResult = await function();
            };

            _mainFunctionDelegate = app;

            return this;
        }

        public IFunctionApplicationBuilder Clear()
        {
            _components.Clear();
            _mainFunctionResult = null;
            _mainFunctionDelegate = null;

            return this;
        }

        public async Task<IActionResult> RunAsync(HttpContext httpContext)
        {
            FunctionRequestDelegate app = _mainFunctionDelegate;
            if (app == null)
            {
                throw new ArgumentNullException("Main function delegate can not be null. Call UseFunction method.");
            }

            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            await app(httpContext);

            return _mainFunctionResult;
        }
    }
}
