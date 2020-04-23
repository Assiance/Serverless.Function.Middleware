using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serverless.Function.Middleware.Abstractions;

namespace Serverless.Function.Middleware.Tests.Mocks
{
    public class MockMiddleware : IFunctionMiddleware
    {
        public async Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next)
        {
            await next(httpContext);
        }
    }
}
