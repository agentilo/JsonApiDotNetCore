namespace JsonApiDotNetCore.Serialization.Response;

/// <inheritdoc />
public sealed class EmptyResponseMeta : IResponseMeta
{
    /// <inheritdoc />
    public IDictionary<string, object?>? GetMeta(Queries.IPaginationContext paginationContext)
    {
        return null;
    }
}
