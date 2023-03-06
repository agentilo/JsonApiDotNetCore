using System.Net;
using JetBrains.Annotations;
using JsonApiDotNetCore.Serialization.Objects;

namespace JsonApiDotNetCore.Errors;

/// <summary>
/// The error that is thrown when a resource does not exist.
/// </summary>
[PublicAPI]
public sealed class ForbiddenOperationException : JsonApiException
{
    public ForbiddenOperationException()
        : base(new ErrorObject(HttpStatusCode.Forbidden)
        {
            Title = "Forbidden",
            Detail = $"No permissions for this resources"
        })
    {
    }
}
