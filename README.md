# Serverless.Function.Middleware
This package provides serverless functions with middleware functionality similar to AspNetCore

### Create Middleware that implements IFunctionMiddleware instead of IMiddleware
```
public class SampleMiddleware : IFunctionMiddleware
{
    public SampleMiddleware()
    {
    }

    public async Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next)
    {
        // Do something before function is executed
        
        await next(httpContext);
        
        // Do something after function is executed
    }
}
```

### Startup File using Microsoft.Azure.Functions.Extensions Package
```
[assembly: FunctionsStartup(typeof(Startup))]
namespace SampleAzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.UseServerlessMiddleware();
            services.AddTransient<SampleMiddleware>();
        }
    }
}
```

### Implement Function, load middleware into the functionBuilder and execute pipeline
```
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
```

# IFunctionApplicationBuilder Methods

### UseMiddleware<TMiddleware>() - Add Middleware to the pipeline (Can add multiple)
`builder.UseMiddleware<SampleMiddleware>();`
    
### UseFunction(Func<Task<IActionResult>> function) - Add or Replace function that returns IActionResult to the pipeline. (Can only have once) 
```
var pipeline = _builder.UseEndpoint(async () =>
{
    return new OkObjectResult("Everything went well");
});
```
    
### RunAsync(HttpContext httpContext) - Executes the request pipeline
`await pipeline.RunAsync(req.HttpContext);`

### Clear() - Clears middleware and endpoint from the request pipeline
`builder.Clear();`
