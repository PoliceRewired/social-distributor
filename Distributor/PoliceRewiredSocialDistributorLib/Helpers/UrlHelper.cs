using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PoliceRewiredSocialDistributorLib.Helpers
{
    public class UrlHelper
    {
        public static async Task<Uri> ResolveUrlAsync(Uri uri)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                return response.RequestMessage.RequestUri;
            }
        }

    }
}
