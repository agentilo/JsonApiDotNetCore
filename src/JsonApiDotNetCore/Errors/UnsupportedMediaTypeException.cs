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
    [PublicAPI]
    public class UnsupportedMediaTypeException : JsonApiException
    {
        public UnsupportedMediaTypeException(MediaTypeHeaderValue allowedMediaTypeValue)
            : base(new ErrorObject(HttpStatusCode.UnsupportedMediaType)
            {
                Title = "The specified Content-type value is not supported.",
                Detail = $"Please include '{allowedMediaTypeValue}' in the Content-type.",
                Source = new ErrorSource
                {
                    Header = "Content-Type"
                }
            })
        {
        }
    }
}
