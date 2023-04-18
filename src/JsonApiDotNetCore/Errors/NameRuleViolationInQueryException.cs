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
    /// The error that is thrown when processing the request fails due to an error in the request query string.
    /// </summary>
    [PublicAPI]
    public sealed class NameRuleViolationInQueryException : JsonApiException
    {
        public string ParameterName { get; }

        public NameRuleViolationInQueryException(string parameterName, string genericMessage, string specificMessage, Exception? innerException = null)
            : base(new ErrorObject(HttpStatusCode.BadRequest)
            {
                Title = genericMessage ?? "Name rule violation",
                Detail = specificMessage ?? "A parameter contains unallowed characters",
                Source = new ErrorSource
                {
                    Parameter = parameterName

                }
            }, innerException)
        {
            ParameterName = parameterName;
        }
    }

}
