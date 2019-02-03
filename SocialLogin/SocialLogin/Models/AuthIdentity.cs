using System;
namespace SocialLogin.Models
{
    public class AuthIdentity
    {
        public AuthProviderType AuthProvider { get; set; }

        public string AccessToken { get; set; }

        public string IdToken { get; set; }

        public string ClientId { get; set; }
    }
}
