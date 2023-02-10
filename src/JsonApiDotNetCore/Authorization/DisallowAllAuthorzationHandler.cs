using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiDotNetCore.Authorization
{
    public sealed class DisallowAllAuthorzationHandler : IAuthorizationHandler
    {
        public bool CanManage(string p_accessToken)
        {
            return false;
        }

        public bool CanRead(string p_accessToken)
        {
            return false;
        }

        public bool CanWrite(string p_accessToken)
        {
            return false;
        }

        public bool CanManage(string p_user, string p_password)
        {
            return false;
        }

        public bool CanRead(string p_user, string p_password)
        {
            return false;
        }

        public bool CanWrite(string p_user, string p_password)
        {
            return false;
        }
    }
}
