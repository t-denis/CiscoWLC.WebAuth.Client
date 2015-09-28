using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CiscoWLC.WebAuth.Client.Logging;
using CiscoWLC.WebAuth.Client.Settings;

namespace CiscoWLC.WebAuth.Client.Core
{
    public static class CiscoWebAuthManager
    {
        public static async Task LoginAsync(LoginPageSettings loginPageSettings, AuthSettings authSettings)
        {
            Logger.Verbose("Posting login form");

            var data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("buttonClicked", "4"),
                new KeyValuePair<string, string>("redirect_url", "www.google.com"),
                new KeyValuePair<string, string>("err_flag", "0"),
                new KeyValuePair<string, string>("username", authSettings.Username),
                new KeyValuePair<string, string>("password", authSettings.Password),
            };

            using (var client = new HttpClient())
            {
                var httpResult = await client.PostAsync(loginPageSettings.LoginPageUrl, new FormUrlEncodedContent(data));
                Logger.Verbose($"{httpResult.StatusCode}: {httpResult.ReasonPhrase}");
                httpResult.EnsureSuccessStatusCode();
            }
        }
    }
}