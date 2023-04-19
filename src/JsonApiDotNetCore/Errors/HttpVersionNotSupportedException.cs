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
    public sealed class HttpVersionNotSupportedException : JsonApiException
    {
        public HttpVersionNotSupportedException(bool onlyError)
            : base(new ErrorObject( onlyError ? HttpStatusCode.HttpVersionNotSupported : HttpStatusCode.InternalServerError)
            {
                Title = "Http Version not supported",
                Detail = $"Http Version has to be HttpVersion/1.1 or above."
            })
        {
        }
    }
}
