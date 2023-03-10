using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Errors;
using Microsoft.AspNetCore.Http;

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
    }
}
