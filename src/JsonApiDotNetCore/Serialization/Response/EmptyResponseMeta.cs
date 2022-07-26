namespace JsonApiDotNetCore.Serialization.Response;

/// <inheritdoc />
public sealed class EmptyResponseMeta : IResponseMeta
{
    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?>? GetMeta(Queries.IPaginationContext paginationContext)
    {
        return null;
    }
}
