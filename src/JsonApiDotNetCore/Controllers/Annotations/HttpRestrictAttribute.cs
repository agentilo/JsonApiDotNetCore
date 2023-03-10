using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using JsonApiDotNetCore.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApiDotNetCore.Controllers.Annotations
{
    public abstract class HttpRestrictAttribute : ActionFilterAttribute
    {
        protected abstract string[] Methods { get; }

        public virtual void CheckIfAbleToExecute(HttpContext context)
        {
            string method = context.Request.Method;

            if (!CanExecuteAction(method))
            {
                throw new RequestMethodNotAllowedException(new HttpMethod(method));
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ArgumentGuard.NotNull(context, nameof(context));
            ArgumentGuard.NotNull(next, nameof(next));

            string method = context.HttpContext.Request.Method;

            if (!CanExecuteAction(method))
            {
                throw new RequestMethodNotAllowedException(new HttpMethod(method));
            }

            await next();
        }

        protected bool CanExecuteAction(string requestMethod)
        {
            return !Methods.Contains(requestMethod);
        }
    }
}
