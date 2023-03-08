using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;
using Microsoft.Net.Http.Headers;

namespace JsonApiDotNetCore.Errors
{
    /// <summary>
    /// The error that is thrown when an unexcepted error occured.
    /// </summary>
    [PublicAPI]
    public class NotAcceptableRequestException : JsonApiException
    {
        public NotAcceptableRequestException(MediaTypeHeaderValue allowedMediaTypeValue)
            : base(new ErrorObject(HttpStatusCode.NotAcceptable)
            {
                Title = "The specified Accept header value does not contain any supported media types.",
                Detail = $"Please include '{allowedMediaTypeValue}' in the Accept header values.",
                Source = new ErrorSource
                {
                    Header = "Accept"
                }
            })
        {
        }
    }
}
