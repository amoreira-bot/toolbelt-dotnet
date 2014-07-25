using System;

namespace Vtex.Toolbelt.Services
{
    public class AuthenticationException : Exception
    {
        public string Status { get; private set; }

        public AuthenticationException(string authStatus)
            : base("Authentication has failed with status " + authStatus)
        {
            this.Status = authStatus;
        }
    }
}