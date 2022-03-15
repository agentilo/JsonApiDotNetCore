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
        public string FormatResourceName(Type resourceType, out string typeName)
        {
            Attribute? attr = resourceType.GetCustomAttribute(typeof(ResourceAttribute));
            typeName = null;
            if (attr != null && attr is ResourceAttribute attribute)
            {
                typeName = attribute.TypeName;
                return attribute.PublicName;
            }
            else
            {
               return _namingStrategy.GetPropertyName(resourceType.Name.Pluralize(), false);
            }
        }
    }
}
