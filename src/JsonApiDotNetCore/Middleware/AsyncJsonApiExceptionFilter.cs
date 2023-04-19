using System.Net;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApiDotNetCore.Middleware;

/// <inheritdoc />
[PublicAPI]
public sealed class AsyncJsonApiExceptionFilter : IAsyncJsonApiExceptionFilter
{
    private readonly IExceptionHandler _exceptionHandler;

    public AsyncJsonApiExceptionFilter(IExceptionHandler exceptionHandler)
    {
        ArgumentGuard.NotNull(exceptionHandler, nameof(exceptionHandler));

        _exceptionHandler = exceptionHandler;
    }

    /// <inheritdoc />
    public Task OnExceptionAsync(ExceptionContext context)
    {
        ArgumentGuard.NotNull(context, nameof(context));

        if (context.HttpContext.Request.Protocol == HttpProtocol.Http10)
        {
            //Return 500 because there are  errors.
            context.Result = new ObjectResult(new ErrorObject(HttpStatusCode.InternalServerError)
            {
                Title = "Http Version not supported",
                Detail = $"Http Version has to be HttpVersion/1.1 or above."
            });
        }
        else
        {
            if (context.HttpContext.IsJsonApiRequest())
            {

                IReadOnlyList<ErrorObject> errors = _exceptionHandler.HandleException(context.Exception);

                context.Result = new ObjectResult(errors)
                {
                    StatusCode = (int)ErrorObject.GetResponseStatusCode(errors)
                };
            }
        }
        return Task.CompletedTask;
    }
}
