using System;
using System.Reflection;
using Humanizer;
using JsonApiDotNetCore.Resources.Annotations;
using Newtonsoft.Json.Serialization;

namespace JsonApiDotNetCore.Configuration
{
    internal sealed class ResourceNameFormatter
    {
        private readonly NamingStrategy _namingStrategy;

        public ResourceNameFormatter(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy;
        }

        /// <summary>
        /// Gets the publicly visible resource name for the internal type name using the configured naming convention.
        /// </summary>
        /// <returns>Tuple. First string is public name, secont typename</returns>
        public (string resourceName, string typeName) FormatResourceName(Type resourceType)
        {
            Attribute attr = resourceType.GetCustomAttribute(typeof(ResourceAttribute));
            string typeName = null;
            if (attr != null && attr is ResourceAttribute attribute)
            {
                typeName = attribute.TypeName;
                return (attribute.PublicName, typeName);
            }
            else
            {
               return (_namingStrategy.GetPropertyName(resourceType.Name.Pluralize(), false), null);
            }
        }
    }
}
