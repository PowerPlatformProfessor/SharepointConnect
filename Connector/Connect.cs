using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Connector
{
    public static class Connect
    {
        private static string sharePointAPIUri = "https://yourdomain.sharepoint.com/_api/web/";
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri(sharePointAPIUri),
            Timeout = new TimeSpan(0, 10, 0)
        };

        private static AuthenticationResult authenticationResult = null;
        private static async Task SetAuthenticationResult(string siteUrl, string clientId, string tenant, X509Certificate2 certificate)
        {
            if (authenticationResult != null && authenticationResult.ExpiresOn.UtcDateTime > DateTime.UtcNow.AddMinutes(-10)) return;

            var authority = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/", "https://login.windows.net", tenant);

            AuthenticationContext authenticationContext = new AuthenticationContext(authority);
            authenticationContext.TokenCache.Clear();

            var clientAssertionCertificate = new ClientAssertionCertificate(clientId, certificate);
            var host = new Uri(siteUrl);

            authenticationResult = await authenticationContext.AcquireTokenAsync(host.Scheme + "://" + host.Host + "/", clientAssertionCertificate);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
        }

        public static async Task<string> CallSharePoint()
        {
            try
            {
                var tentantId = "11111111-1111-1111-1111-111111111112";
                var clientId = "11111111-1111-1111-1111-111111111111";
                var resource = "https://yourdomain.sharepoint.com/";

                await SetAuthenticationResult(
                resource,
                clientId,
                tentantId,
                new X509Certificate2(Properties.Resources.Sharepoint, "1234"));

                var response = await _client.GetAsync("lists");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                throw;
            }
            
        }
    }
}
