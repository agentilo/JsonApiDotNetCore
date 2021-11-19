using System.Collections.Generic;
using JetBrains.Annotations;
using JsonApiDotNetCore.Resources.Annotations;

namespace JsonApiDotNetCoreTests.IntegrationTests.OptimisticConcurrency
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public sealed class PageFooter : PostgresVersionedIdentifiable<long>
    {
        [Attr]
        public string? Copyright { get; set; }

        [HasMany]
        public IList<WebLink> Links { get; set; } = new List<WebLink>();

        [HasMany]
        public IList<WebPage> UsedAt { get; set; } = new List<WebPage>();
    }
}
