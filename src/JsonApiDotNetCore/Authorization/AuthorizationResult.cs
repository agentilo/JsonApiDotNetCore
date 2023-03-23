using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiDotNetCore.Authorization
{
    public enum AuthorizationResult
    {
        OK,
        Expired,
        Forbidden,
        Unauthorized,
        NotFound
    }
}
