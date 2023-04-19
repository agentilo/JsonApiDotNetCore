using JsonApiDotNetCore.Serialization.Objects;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApiDotNetCore.Middleware
{
    public sealed class ProtocolActionResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.HttpContext.Request.Protocol == HttpProtocol.Http10)
            {
                context.Result = context.Result = new ObjectResult(new ErrorObject(HttpStatusCode.HttpVersionNotSupported)
                {
                    Title = "Http Version not supported",
                    Detail = $"Http Version has to be HttpVersion/1.1 or above."
                });
            }
        }
    }
}
