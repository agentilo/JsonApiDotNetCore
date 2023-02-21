using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;

namespace JsonApiDotNetCore.Errors
{
    [PublicAPI]
    public sealed class BearerTokenExpiredException : JsonApiException
    {
        public BearerTokenExpiredException()
            : base(new ErrorObject(HttpStatusCode.Unauthorized)
            {
                Title = "Token expired",
                Detail = $"The bearer token is not valid."
            })
        {
        }
    }
}
