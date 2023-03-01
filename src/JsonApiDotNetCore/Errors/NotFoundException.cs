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
    public class NotFoundException : JsonApiException
    {
        public NotFoundException(string details = "")
            : base(new ErrorObject(HttpStatusCode.NotFound)
            {
                Title = "Not Found",
                Detail = String.IsNullOrEmpty(details) ? "Some elements where not found." : details
            })
        {
        }
    }
}
