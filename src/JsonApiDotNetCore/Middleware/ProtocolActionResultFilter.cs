using JsonApiDotNetCore.Serialization.Objects;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JsonApiDotNetCore.Errors;

namespace JsonApiDotNetCore.Middleware
{
    public sealed class ProtocolActionResultFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Protocol == HttpProtocol.Http10)
            {
                if (context.Exception == null)
                    context.Exception = new HttpVersionNotSupportedException(true);
                else
                    context.Exception = new HttpVersionNotSupportedException(false);
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
