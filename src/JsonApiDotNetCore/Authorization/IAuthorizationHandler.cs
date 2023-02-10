using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiDotNetCore.Authorization
{
    public interface IAuthorizationHandler
    {
        /// <summary>
        /// Checks if the User is allowed to make GET Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <returns></returns>
        bool CanRead(string p_accessToken);
        /// <summary>
        /// Checks if the User is allowed to make PUT Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <returns></returns>
        bool CanWrite(string p_accessToken);
        /// <summary>
        /// Checks if the User is allowed to make DELETE|POST|PATCH Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <returns></returns>
        bool CanManage(string p_accessToken);

        /// <summary>
        /// Checks if the User is allowed to make GET Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <param name="p_password"></param>
        /// <returns></returns>
        bool CanRead(string p_user, string p_password);
        /// <summary>
        /// Checks if the User is allowed to make PUT Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <param name="p_password"></param>
        /// <returns></returns>
        bool CanWrite(string p_user, string p_password);
        /// <summary>
        /// Checks if the User is allowed to make DELETE|POST|PATCH Requests to the API
        /// </summary>
        /// <param name="p_accessToken"></param>
        /// <param name="p_password"></param>
        /// <returns></returns>
        bool CanManage(string p_user, string p_password);
    }
}
