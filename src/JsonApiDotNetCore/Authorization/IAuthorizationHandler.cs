using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiDotNetCore.Resources;

namespace JsonApiDotNetCore.Authorization
{
    public interface IAuthorizationHandler<TId>
    {
        FilterResourceResult<TResource> FilterResourcesForRead<TResource>(AuthCredentials creds, IReadOnlyCollection<TResource> resources) where TResource : class, IIdentifiable<TId>;
        AuthorizationResult IsAllowedToRead(TId? id, AuthCredentials cred);
        AuthorizationResult IsAllowedToWrite(TId? id, AuthCredentials cred);
        AuthorizationResult IsAllowedToManage(TId? id, AuthCredentials cred);
        AuthorizationResult IsAllowedToWrite<TResource>(ICollection<TResource> tList, AuthCredentials cred) where TResource : class, IIdentifiable<TId>;

        //Is generally allowed to access resources without scopes
        AuthorizationResult IsAllowed(AuthCredentials cred);
        AuthorizationResult IsAllowedToManage<TId>(IIdentifiable<TId> resource, AuthCredentials cred);
    }
}
