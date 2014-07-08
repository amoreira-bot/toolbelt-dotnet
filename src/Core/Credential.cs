namespace Vtex.Toolbelt.Core
{
    public class Credential
    {
        public string Email { get; set; }
        public string Token { get; set; }

        public Credential()
        {
        }

        public Credential(string email, string token)
        {
            this.Email = email;
            this.Token = token;
        }
    }
}