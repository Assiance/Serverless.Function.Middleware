using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Serverless.Function.Middleware.Abstractions
{
    public interface IFunctionApplicationBuilder
    {
        IServiceProvider ApplicationServices { get; }
        IFunctionApplicationBuilder Use(Func<FunctionRequestDelegate, FunctionRequestDelegate> middleware);
        IFunctionApplicationBuilder UseMiddleware<TMiddleware>();
        IFunctionApplicationBuilder UseEndpoint(Func<Task<IActionResult>> azureFunction);
        IFunctionApplicationBuilder Clear();
        Task<IActionResult> RunAsync(HttpContext httpContext);
    }
}