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
        IFunctionApplicationBuilder UseMiddleware(Type middlewareType);
        IFunctionApplicationBuilder UseFunction(Func<Task<IActionResult>> function);
        IFunctionApplicationBuilder Clear();
        Task<IActionResult> RunAsync(HttpContext httpContext);
    }
}