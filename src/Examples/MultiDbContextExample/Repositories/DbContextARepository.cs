using System.Collections.Generic;
using JetBrains.Annotations;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Queries;
using JsonApiDotNetCore.Repositories;
using JsonApiDotNetCore.Resources;
using Microsoft.Extensions.Logging;
using MultiDbContextExample.Data;

namespace MultiDbContextExample.Repositories
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class DbContextARepository<TResource> : EntityFrameworkCoreRepository<TResource, int>
        where TResource : class, IIdentifiable<int>
    {
        public DbContextARepository(ITargetedFields targetedFields, DbContextResolver<DbContextA> dbContextResolver, IResourceGraph resourceGraph,
            IResourceFactory resourceFactory, IEnumerable<IQueryConstraintProvider> constraintProviders, ILoggerFactory loggerFactory,
            IResourceDefinitionAccessor resourceDefinitionAccessor)
            : base(targetedFields, dbContextResolver, resourceGraph, resourceFactory, constraintProviders, loggerFactory, resourceDefinitionAccessor)
        {
        }
    }
}
