using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JsonApiDotNetCore.Controllers.Annotations;
using JsonApiDotNetCore.Serialization.Objects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace JsonApiDotNetCore.Middleware
{
    internal class HttpRequestRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionHandler _exHandler;

        public HttpRequestRestrictionMiddleware(RequestDelegate next, IExceptionHandler handler)
        {
            _next = next;
            _exHandler = handler;
        }

        public async Task Invoke(HttpContext context, IAsyncJsonApiExceptionFilter exceptionFilter)
        {
            bool restricted = false;
            var endpoint = context.GetEndpoint();
            var httpRestrictAttributes = endpoint?.Metadata.Where(m => m is HttpRestrictAttribute);

            if (httpRestrictAttributes != null)
            {
                try
                {
                    foreach (var httpRestrictAttribute in httpRestrictAttributes)
                        ((HttpRestrictAttribute)httpRestrictAttribute).CheckIfAbleToExecute(context);
                }catch(Exception ex)
                {
                    ActionResult result = new StatusCodeResult(405);
                    if (context.IsJsonApiRequest())
                    {
                        IReadOnlyList<ErrorObject> errors = _exHandler.HandleException(ex);

                        result = new ObjectResult(errors)
                        {
                            StatusCode = (int)ErrorObject.GetResponseStatusCode(errors)
                        };
                    }
                    await result.ExecuteResultAsync(new ActionContext
                    {
                        HttpContext = context
                    });
                    restricted = true;
                }
                
            }
            if (_next != null && !restricted) await _next(context);
        }
    }
}
