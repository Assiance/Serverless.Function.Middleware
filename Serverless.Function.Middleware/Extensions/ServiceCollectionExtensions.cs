using Microsoft.Extensions.DependencyInjection;
using Serverless.Function.Middleware.Abstractions;

namespace Serverless.Function.Middleware.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseServerlessMiddleware(this IServiceCollection services)
        {
            services.AddScoped<IFunctionApplicationBuilder, FunctionApplicationBuilder>();
            return services;
        }
    }
}
