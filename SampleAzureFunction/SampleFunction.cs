using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Serverless.Function.Middleware.Abstractions;
using System.Threading.Tasks;

namespace SampleAzureFunction
{
    public class SampleFunction
    {
        private readonly IFunctionApplicationBuilder _builder;

        public SampleFunction(IFunctionApplicationBuilder builder)
        {
            _builder = builder;

            // Loading the middleware into the builder. You can load multiple middleware.
            _builder.UseMiddleware<SampleMiddleware>();
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Loading the endpoint to be called. You can only load a single endpoint
            // This will return a pipeline complete with middleware and endpoint delegates.
            var pipeline = _builder.UseEndpoint(async () =>
            {
                log.LogInformation("FUNCTION EXECUTING!!!!");

                return new OkObjectResult("Everything went well");
            });

            // Execute the pipeline.
            return await pipeline.RunAsync(req.HttpContext);
        }
    }
}
