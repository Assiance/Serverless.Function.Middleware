using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Serverless.Function.Middleware
{
    public delegate Task FunctionRequestDelegate(HttpContext context);
}