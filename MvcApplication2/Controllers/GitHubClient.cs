using DotNetOpenAuth.AspNet.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetOpenAuth.Messaging;

using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Web.Mvc;

namespace MvcApplication2.Controllers
{
    public class GitHubClient : OAuth2Client
    {
        private const string AccessTokenUrl = "https://github.com/login/oauth/access_token";
        private const string AuthorizeUrl = "https://github.com/login/oauth/authorize";
        private const string ProfileUrl = "https://api.github.com/user";

        private readonly string clientId;
        private readonly string clientSecret;

        public GitHubClient(string clientId, string clientSecret)
            : base("GitHub")
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(AuthorizeUrl);
            uriBuilder.AppendQueryArgument("client_id", this.clientId);
            uriBuilder.AppendQueryArgument("redirect_uri", returnUrl.ToString());

            return uriBuilder.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            using (var webClient = CreateWebClient())
            {
                var uriBuilder = new UriBuilder(ProfileUrl);
                uriBuilder.AppendQueryArgument("access_token", accessToken);

                var profileResponse = webClient.DownloadString(uriBuilder.Uri);
                var profile = JsonConvert.DeserializeObject<dynamic>(profileResponse);

                return new Dictionary<string, string>
                       {
                           { "login", profile.login.ToString() }, 
                           { "id", profile.id.ToString() }, 
                           { "avatar_url", profile.avatar_url.ToString() }, 
                           { "gravatar_id", profile.gravatar_id.ToString() }
                       };
            }
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            using (var webClient = CreateWebClient())
            {
                var parameters = new NameValueCollection
                                          {
                                              { "client_id", this.clientId },
                                              { "client_secret", this.clientSecret },
                                              { "redirect_uri", returnUrl.ToString() },
                                              { "code", authorizationCode },
                                          };

                var accessTokenResponse = Encoding.UTF8.GetString(webClient.UploadValues(AccessTokenUrl, parameters));
                var parsedAccessTokenResponse = HttpUtility.ParseQueryString(accessTokenResponse);
                return parsedAccessTokenResponse["access_token"];
            }
        }

        private static WebClient CreateWebClient()
        {
            var webClient = new WebClient();
            return webClient;
        }
    }
}