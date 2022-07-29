using System.Net;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;

namespace JsonApiDotNetCore.Errors;

/// <summary>
/// The error that is thrown when processing the request fails due to an error in the request query string.
/// </summary>
[PublicAPI]
public sealed class InvalidQueryStringParameterException : JsonApiException
{
    public string ParameterName { get; }

    public InvalidQueryStringParameterException(string parameterName, string genericMessage, string specificMessage, Exception? innerException = null)
        : base(new ErrorObject(HttpStatusCode.UnprocessableEntity)
        {
            Title = "Unprocessable Entity",
            Detail = "Syntax error. Unable to process the contained instructions.",
            Source = new ErrorSource
            {
                Parameter = parameterName
            }
        }, innerException)
    {
        ParameterName = parameterName;
    }
}
