using System.Net;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;

namespace JsonApiDotNetCore.Errors;

/// <summary>
/// The error that is thrown when a resource does not exist.
/// </summary>
[PublicAPI]
public sealed class UnauthorizedOperationException : JsonApiException
{
    public UnauthorizedOperationException(string? operation)
        : base(new ErrorObject(HttpStatusCode.Unauthorized)
        {
            Title = "Unauthorized",
            Detail = $"No authorization for { operation} operations"
        })
    {
    }
}
