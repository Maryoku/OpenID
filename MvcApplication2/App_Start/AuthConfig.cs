using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using MvcApplication2.Models;
using MvcApplication2.Controllers;

namespace MvcApplication2
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // следует обновить сайт. Дополнительные сведения: http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");

            //OAuthWebSecurity.RegisterGoogleClient();

            var vkAuth = new VkClient(
                AppId: "AppId",
                AppSecret: "AppSecret");

            OAuthWebSecurity.RegisterClient(vkAuth, displayName: "ВТентакле", extraData: null);

            var gitHubClient = new GitHubClient(
                clientId: "clientId",
                clientSecret: "clientSecret");
            OAuthWebSecurity.RegisterClient(gitHubClient, "GitHub", null);
        }
    }
}
