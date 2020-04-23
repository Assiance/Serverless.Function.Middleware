using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serverless.Function.Middleware;
using Serverless.Function.Middleware.Abstractions;
using System.Threading.Tasks;

namespace SampleAzureFunction
{
    public class SampleMiddleware : IFunctionMiddleware
    {
        private readonly ILogger<SampleMiddleware> _logger;

        public SampleMiddleware(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SampleMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next)
        {
            _logger.LogInformation("Before the function endpoint is executed");

            await next(httpContext);

            _logger.LogInformation("After the function endpoint is executed");
        }
    }
}
