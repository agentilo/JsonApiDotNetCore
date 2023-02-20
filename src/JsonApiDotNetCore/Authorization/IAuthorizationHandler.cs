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
        ICollection<TResource> FilterResourcesForRead<TResource>(AuthCredentials creds, IReadOnlyCollection<TResource> resources) where TResource : class, IIdentifiable<TId>;
        bool IsAllowedToRead(TId? id, AuthCredentials cred);
        bool IsAllowedToWrite(TId? id, AuthCredentials cred);
        bool IsAllowedToManage(TId? id, AuthCredentials cred);
        bool IsAllowedToWrite<TResource>(ICollection<TResource> tList, AuthCredentials cred) where TResource : class, IIdentifiable<TId>;
    }
}
