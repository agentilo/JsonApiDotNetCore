using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApiDotNetCore.Controllers.Annotations
{
    [PublicAPI]
    public sealed class NoHttpPutExceptValuesAttribute : HttpRestrictAttribute
    {
        protected override string[] Methods { get; } =
        {
            "PUT"
        };

        public override void CheckIfAbleToExecute(HttpContext context)
        {
            string method = context.Request.Method;

            if (!CanExecuteAction(method))
            {
                if (context?.Request?.Path.Value == null || !context.Request.Path.Value.EndsWith("values"))
                {
                    throw new RequestMethodNotAllowedException(new HttpMethod(method));
                }
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ArgumentGuard.NotNull(context, nameof(context));
            ArgumentGuard.NotNull(next, nameof(next));

            string method = context.HttpContext.Request.Method;

            if (!CanExecuteAction(method))
            {
                if (context?.HttpContext?.Request?.Path.Value == null || !context.HttpContext.Request.Path.Value.EndsWith("values"))
                {
                    throw new RequestMethodNotAllowedException(new HttpMethod(method));
                }
            }

            await next();
        }
    }
}
