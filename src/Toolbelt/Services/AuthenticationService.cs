using System;
using System.Net;
using System.Net.Http;

namespace Vtex.Toolbelt.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient httpClient;

        public AuthenticationService()
        {
            this.httpClient = new HttpClient { BaseAddress = new Uri("https://vtexid.vtex.com.br/api/") };
        }

        public bool IsTokenValid(string login, string token)
        {
            var path = string.Format("vtexid/pub/authenticated/user?authToken={0}", WebUtility.UrlEncode(token));
            var response = this.httpClient.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();

            var user = response.Content.ReadAsAsync<VtexIdUser>().Result;
            return user != null && user.User == login;
        }

        public string GetAuthenticationToken(string login, string password)
        {
            var tempToken = this.GetTemporaryToken();
            var authenticationResponse = this.AuthenticateUser(login, password, tempToken);

            if (!authenticationResponse.AuthStatus.Equals("success", StringComparison.OrdinalIgnoreCase))
                throw new AuthenticationException(authenticationResponse.AuthStatus);

            return authenticationResponse.AuthCookie.Value;
        }

        private AuthenticateUserResponse AuthenticateUser(string login, string password, string token)
        {
            var path = string.Format(
                "vtexid/pub/authentication/classic/validate?authenticationToken={0}&login={1}&password={2}",
                Uri.EscapeDataString(token), Uri.EscapeDataString(login), Uri.EscapeDataString(password));
            var response = this.httpClient.GetAsync(path).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsAsync<AuthenticateUserResponse>().Result;
        }

        private string GetTemporaryToken()
        {
            var response = this.httpClient.GetAsync("vtexid/pub/authentication/start").Result;
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsAsync<TemporaryTokenResponse>().Result;
            return content.AuthenticationToken;
        }

        public class TemporaryTokenResponse
        {
            public string AuthenticationToken { get; set; }
        }

        public class AuthenticateUserResponse
        {
            public string AuthStatus { get; set; }
            public NameValuePair AuthCookie { get; set; }
        }

        public class NameValuePair
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class VtexIdUser
        {
            public string User { get; set; }
        }
    }
}