using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JsonApiDotNetCore.Controllers.Annotations
{
    /// <summary>
    /// Blocks PATCH except on the named relationship endpoints, i.e. it permits
    /// PATCH {resource}/{id}/relationships/{name} while still rejecting PATCH {resource}/{id}.
    ///
    /// NoHttpPatch cannot express this: HttpRestrictAttribute keys purely on the HTTP method, so it
    /// disables the resource update and the relationship update together. This is the same shape as
    /// NoHttpPutExceptValues, generalized over the relationship name.
    ///
    /// Pass every spelling a client may use - JsonApiDotNetCore serves a relationship under both its
    /// public name and its relationship name.
    /// </summary>
    [PublicAPI]
    public sealed class NoHttpPatchExceptRelationshipAttribute : HttpRestrictAttribute
    {
        private readonly string[] _allowedRelationships;

        protected override string[] Methods { get; } =
        {
            "PATCH"
        };

        public NoHttpPatchExceptRelationshipAttribute(params string[] allowedRelationships)
        {
            _allowedRelationships = allowedRelationships ?? Array.Empty<string>();
        }

        private bool IsAllowedPath(HttpContext context)
        {
            string path = context?.Request?.Path.Value;

            if (path == null)
            {
                return false;
            }

            return _allowedRelationships.Any(relationship =>
                path.EndsWith($"/relationships/{relationship}", StringComparison.OrdinalIgnoreCase));
        }

        public override void CheckIfAbleToExecute(HttpContext context)
        {
            string method = context.Request.Method;

            if (!CanExecuteAction(method) && !IsAllowedPath(context))
            {
                throw new RequestMethodNotAllowedException(new HttpMethod(method));
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ArgumentGuard.NotNull(context, nameof(context));
            ArgumentGuard.NotNull(next, nameof(next));

            string method = context.HttpContext.Request.Method;

            if (!CanExecuteAction(method) && !IsAllowedPath(context.HttpContext))
            {
                throw new RequestMethodNotAllowedException(new HttpMethod(method));
            }

            await next();
        }
    }
}
