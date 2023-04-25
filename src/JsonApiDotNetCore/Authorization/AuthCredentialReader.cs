using Microsoft.AspNetCore.Http;
using System.Text;
using System;

namespace JsonApiDotNetCore.Authorization
{
    public class AuthCredentialReader
    {
        public AuthCredentials? GetAuthCredentials(HttpContext context)
        {
            string? authHeader = context?.Request?.Headers["Authorization"];
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                string token = authHeader.Substring("Bearer ".Length).Trim();
                return new AuthCredentials(token);
            }

            if (authHeader?.StartsWith("Basic ") == true)
            {
                string encoded = authHeader.Substring("Basic ".Length).Trim();
                var credentials = _GetUserCredentials(encoded);
                if (credentials.user != null && credentials.pw != null)
                    return new AuthCredentials(credentials.user, credentials.pw);
            }
            return null;
        }

        private (string? user, string? pw) _GetUserCredentials(string encodedUser)
        {

            string? user = null;
            string? pw = null;
            try
            {
                string usernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUser));
                string[] splitted = usernamePassword.Split(":");
                user = splitted[0];
                pw = splitted[1];
            }catch
            {

            }
            finally
            {
            }
            return (user, pw);

        }

    }
}
