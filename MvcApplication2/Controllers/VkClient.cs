using DotNetOpenAuth.AspNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Text;
using System.Net;

namespace MvcApplication2.Controllers
{
    public class VkClient : OAuth2Client
    {
        private const string AccessTokenUrl = "https://oauth.vk.com/access_token";
        private const string AuthorizeUrl = "https://oauth.vk.com/authorize";
        private const string ProfileUrl = "https://api.vk.com/method/user";

        private readonly string AppId;
        private readonly string AppSecret;

        public VkClient(string AppId, string AppSecret)
            : base("Vk")
        {
            this.AppId = AppId;
            this.AppSecret = AppSecret;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var uriBuilder = new UriBuilder(AuthorizeUrl);
            uriBuilder.AppendQueryArgument("client_id", this.AppId);
            uriBuilder.AppendQueryArgument("display", "popup");
            uriBuilder.AppendQueryArgument("redirect_uri", returnUrl.ToString());
            uriBuilder.AppendQueryArgument("scope", "friends");
            uriBuilder.AppendQueryArgument("response_type", "code");

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
                           { "avatar_url", profile.avatar_url.ToString() }
                       };
            }
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            using (var webClient = CreateWebClient())
            {
                var parameters = new NameValueCollection
                                          {
                                              { "client_id", this.AppId },
                                              { "client_secret", this.AppSecret },
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