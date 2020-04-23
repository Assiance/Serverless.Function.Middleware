using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Serverless.Function.Middleware.Abstractions
{
    public interface IFunctionMiddleware
    {
        Task InvokeAsync(HttpContext httpContext, FunctionRequestDelegate next);
    }
}