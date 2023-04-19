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
    /// <summary>
    /// The error that is thrown when an unexcepted error occured.
    /// </summary>
    [PublicAPI]
    public class NotImplementedOperationException : JsonApiException
    {
        public NotImplementedOperationException(string operationName)
            : base(new ErrorObject(HttpStatusCode.NotImplemented)
            {
                Title = "Not Implemented",
                Detail = $"{operationName} not implemented."
            })
        {
        }
    }
}
