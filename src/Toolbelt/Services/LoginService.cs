using System;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class LoginService
    {
        private readonly IConsole _console;
        private readonly CredentialStore _credentialStore;
        private readonly AuthenticationService _authentication;

        public LoginService(IConsole console, CredentialStore credentialStore, AuthenticationService authentication)
        {
            this._console = console;
            this._credentialStore = credentialStore;
            this._authentication = authentication;
        }

        public Credential GetValidCredential()
        {
            var credential = _credentialStore.GetCurrent();

            if (credential == null || !this.Validate(credential))
                credential = this.Login();

            Console.WriteLine("Logged in as {0}", credential.Email);
            _credentialStore.Save(credential);
            return credential;
        }

        private bool Validate(Credential credential)
        {
            return _authentication.IsTokenValid(credential.Email, credential.Token);
        }

        private Credential Login()
        {
            _console.WriteLine("Please log in with your VTEX credential.");
            while (true)
            {
                var login = _console.ReadLine("login: ");
                var password = _console.ReadPassword("password: ");

                try
                {
                    var token = _authentication.GetAuthenticationToken(login, password);
                    _console.WriteLine();
                    return new Credential(login, token);
                }
                catch (AuthenticationException exception)
                {
                    _console.WriteLine();
                    _console.WriteLine("Login failed with status {0}. Please try again.", exception.Status);
                }
            }
        }
    }
}
