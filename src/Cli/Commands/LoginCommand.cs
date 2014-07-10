using System;
using Vtex.Toolbelt.Core;
using Vtex.Toolbelt.Core.Services;

namespace Vtex.Toolbelt.Cli.Commands
{
    public class LoginCommand : Command
    {
        private readonly string accountName;
        private readonly CredentialStore credentialStore;
        private readonly AuthenticationService authentication;

        public LoginCommand(string accountName)
            : this(accountName, new CredentialStore(), new AuthenticationService())
        {
        }

        protected LoginCommand(string accountName, CredentialStore credentialStore, AuthenticationService authentication)
        {
            this.accountName = accountName;
            this.credentialStore = credentialStore;
            this.authentication = authentication;
        }

        public override void Run()
        {
            GetValidCredential();
        }

        public Credential GetValidCredential()
        {
            var credential = this.credentialStore.GetCurrent();

            if (credential == null || !this.Validate(credential))
                credential = this.Login();

            Console.WriteLine("Logged in as {0}", credential.Email);
            this.credentialStore.Save(credential);
            return credential;
        }

        private bool Validate(Credential credential)
        {
            return this.authentication.IsTokenValid(credential.Email, credential.Token);
        }

        private Credential Login()
        {
            Console.WriteLine("Please log in with your VTEX credential.");
            while (true)
            {
                var login = ReadLine("login: ");
                var password = ReadPassword("password: ");

                try
                {
                    var token = authentication.GetAuthenticationToken(login, password);
                    Console.WriteLine();
                    return new Credential(login, token);
                }
                catch (AuthenticationException exception)
                {
                    Console.WriteLine();
                    Console.WriteLine("Login failed with status {0}. Please try again.", exception.Status);
                }
            }
        }
    }
}
