using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiDotNetCore.Authorization
{
    public class AuthCredentials
    {
        public string? AccessToken { get; }
        public string? User { get; }
        public string? Password { get; }
        public AuthCredentialsType CredentialsType { get; }

        public AuthCredentials(string accessToken)
        {
            AccessToken = accessToken;
            CredentialsType = AuthCredentialsType.Bearer;
        }

        public AuthCredentials(string user, string password)
        {
            User = user;
            Password = password;
            CredentialsType= AuthCredentialsType.Basic;
        }

        public AuthCredentials(AuthCredentials creds)
        {
            AccessToken = creds?.AccessToken;
            User = creds?.User;
            Password = creds?.Password;
            CredentialsType = creds?.CredentialsType ?? AuthCredentialsType.Basic;
        }

    }
}
