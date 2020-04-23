using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serverless.Function.Middleware.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Serverless.Function.Middleware
{
    public class FunctionApplicationBuilder : IFunctionApplicationBuilder
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IList<Func<FunctionRequestDelegate, FunctionRequestDelegate>> _components = new List<Func<FunctionRequestDelegate, FunctionRequestDelegate>>();

        private IActionResult _endpointResult;
        private FunctionRequestDelegate _endPointDelegate;

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

            return this;
        }

        public IFunctionApplicationBuilder UseEndpoint(Func<Task<IActionResult>> azureFunction)
        {
            FunctionRequestDelegate app = async (context) =>
            {
                _endpointResult = await azureFunction();
            };

            _endPointDelegate = app;

            return this;
        }

        public IFunctionApplicationBuilder Clear()
        {
            _components.Clear();
            _endpointResult = null;
            _endPointDelegate = null;

            return this;
        }

        public async Task<IActionResult> RunAsync(HttpContext httpContext)
        {
            FunctionRequestDelegate app = _endPointDelegate;
            foreach (var component in _components.Reverse())
            {
                app = component(app);
            }

            await app(httpContext);

            return _endpointResult;
        }
    }
}
