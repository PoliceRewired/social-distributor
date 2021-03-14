using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Social.Summary;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
    /// <summary>
    /// TODO: add image posting for facebook messages
    /// </summary>
    public class FbPoster : IPoster
    {
        private static readonly string FB_BASE_ADDRESS = "https://graph.facebook.com/";

        private long pageId;
        private string token;

        public FbPoster(long pageId, string token)
        {
            this.pageId = pageId;
            this.token = token;
        }

        public bool Accepts(SocialNetwork network)
        {
            return network == SocialNetwork.facebook;
        }

        public async Task InitAsync() { }

        public async Task<IPostSummary> PostAsync(Post post)
        {
            var outcome = await PublishMessage(post.Message);
            return new FbPostSummary(post, outcome);
        }

        private async Task<string> PublishMessage(string message)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(FB_BASE_ADDRESS);

                var parameters = new Dictionary<string, string>
                {
                    { "access_token", token },
                    { "message", message }
                };
                var encodedContent = new FormUrlEncodedContent(parameters);

                var result = await httpClient.PostAsync($"{pageId}/feed", encodedContent);
                var msg = result.EnsureSuccessStatusCode();
                return await msg.Content.ReadAsStringAsync();
            }

        }
    }
}
