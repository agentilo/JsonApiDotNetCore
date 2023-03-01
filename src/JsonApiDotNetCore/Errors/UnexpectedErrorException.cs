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
    public class UnexpectedErrorException : JsonApiException
    {
        public UnexpectedErrorException(Exception? ex)
            : base(new ErrorObject(HttpStatusCode.InternalServerError)
            {
                Title = "Internal Server Error",
                Detail = ex == null ? "Unexpected error." : ex.Message
            })
        {
        }
    }
}
