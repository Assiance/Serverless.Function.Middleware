using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SampleAzureFunction;
using Serilog;
using Serverless.Function.Middleware;
using Serverless.Function.Middleware.Abstractions;
using Serverless.Function.Middleware.Extensions;

[assembly: FunctionsStartup(typeof(Startup))]
namespace SampleAzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            // Registering a Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging(lb => lb.AddSerilog(Log.Logger));

            // Registering the FunctionApplicationBuilder and SampleMiddleware
            services.UseServerlessMiddleware();
            services.AddTransient<SampleMiddleware>();
        }
    }
}