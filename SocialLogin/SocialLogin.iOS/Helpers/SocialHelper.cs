using SocialLogin.Helpers;
using SimpleAuth.Providers;
using System.Threading.Tasks;
using SocialLogin.iOS.Helpers;
using SocialLogin.Models;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(SocialHelper))]
namespace SocialLogin.iOS.Helpers
{
    public class SocialHelper : ISocialHelper
    {
        private readonly string[] _facebookReadPermissions = { "public_profile", "user_friends", "email" };

        public async Task<AuthIdentity> SignIn(AuthProviderType providerType)
        {
            AuthIdentity authIdentity = null;

            switch (providerType)
            {
                case AuthProviderType.Facebook:
                    authIdentity = await FacebookSignIn();
                    break;
                case AuthProviderType.Google:
                    authIdentity = await GoogleSignIn();
                    break;
                default:
                    throw new NotImplementedException($"Login not implemented for {providerType}");
            }

            return authIdentity;
        }

        public void SignOut(AuthProviderType providerType)
        {
            switch (providerType)
            {
                case AuthProviderType.Facebook:
                    var manager = new Facebook.LoginKit.LoginManager();
                    manager.LogOut();
                    break;
                case AuthProviderType.Google:
                    var instance = Google.SignIn.SignIn.SharedInstance;
                    instance.SignOutUser();
                    break;
                default:
                    throw new NotImplementedException($"Login not implemented for {providerType}");
            }
        }

        private async Task<AuthIdentity> FacebookSignIn()
        {
            AuthIdentity authIdentity = null;

            var api = new FacebookApi("facebook", Config.FacebookAppId, Config.FacebookClientToken)
            {
                Scopes = _facebookReadPermissions,
            };

            var account = await api.Authenticate();

            if (account.IsValid() && account is SimpleAuth.OAuthAccount oauthAccount)
            {
                authIdentity = new AuthIdentity
                {
                    AuthProvider = AuthProviderType.Facebook,
                    AccessToken = oauthAccount.Token,
                    IdToken = oauthAccount.IdToken,
                    ClientId = oauthAccount.ClientId
                };
            }

            return authIdentity;
        }

        private async Task<AuthIdentity> GoogleSignIn()
        {
            AuthIdentity authIdentity = null;
            var googleScopes = new[]
                {
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/userinfo.profile",
                };

            var api = new GoogleApi("google", Config.GoogleIOSClientId)
            {
                ServerClientId = Config.GoogleServerClientId,
                Scopes = googleScopes,
            };

            var account = await api.Authenticate();

            if (account.UserData.ContainsKey("ServerToken"))
            {
                var serverToken = account.UserData["ServerToken"];

                if (account.IsValid() && account is SimpleAuth.OAuthAccount oauthAccount)
                {
                    authIdentity = new AuthIdentity
                    {
                        AuthProvider = AuthProviderType.Google,
                        AccessToken = serverToken,
                        IdToken = oauthAccount.IdToken,
                        ClientId = oauthAccount.ClientId
                    };
                }
            }

            return authIdentity;
        }

        private async Task<AuthIdentity> TwitterSignIn()
        {
            AuthIdentity authIdentity = null;

            var api = new TwitterApi("twitter", Config.TwitterClientId, Config.TwitterSecret);

            var account = await api.Authenticate();

            if (account.IsValid() && account is SimpleAuth.OAuthAccount oauthAccount)
            {
                authIdentity = new AuthIdentity
                {
                    AuthProvider = AuthProviderType.Twitter,
                    AccessToken = oauthAccount.Token,
                    IdToken = oauthAccount.IdToken,
                    ClientId = oauthAccount.ClientId
                };
            }

            return authIdentity;
        }
    }
}
