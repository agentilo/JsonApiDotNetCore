using System.Reflection;
using System.Text.Json;
using Humanizer;
using JsonApiDotNetCore.Resources.Annotations;

namespace JsonApiDotNetCore.Configuration;

internal sealed class ResourceNameFormatter
{
    private readonly JsonNamingPolicy? _namingPolicy;

    public ResourceNameFormatter(JsonNamingPolicy? namingPolicy)
    {
        _namingPolicy = namingPolicy;
    }

    /// <summary>
    /// Gets the publicly exposed resource name by applying the configured naming convention on the pluralized CLR type name.
    /// </summary>
    public (string resourceName, string? typeName) FormatResourceName(Type resourceClrType)
    {
        ArgumentGuard.NotNull(resourceClrType, nameof(resourceClrType));

        var resourceAttribute = resourceClrType.GetCustomAttribute<ResourceAttribute>(true);
        if (resourceAttribute != null && !string.IsNullOrWhiteSpace(resourceAttribute.PublicName))
        {
            string? typeName = resourceAttribute.TypeName;
            return (resourceAttribute.PublicName, typeName);
        }

        string publicName = resourceClrType.Name.Pluralize();
        publicName = _namingPolicy != null ? _namingPolicy.ConvertName(publicName) : publicName;
        return (publicName, null);
    }
}
