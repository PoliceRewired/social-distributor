using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PoliceRewiredSocialDistributorLib.Social.Summary;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
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
            if (post.Image != null)
            {
                var response = await UploadMessageImageAsync(post.MessageFacebookIncLink, post.Image);
                return new FbPostSummary(response != null ? "Success: " + response : "Failed");
            }
            else
            {
                var response = await UploadMessageOnlyAsync(post.MessageFacebook, post.Link);
                return new FbPostSummary(response != null ? "Success: " + response : "Failed");
            }
        }

        public async Task<string> UploadMessageOnlyAsync(string message, Uri link)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(FB_BASE_ADDRESS);
                var parameters = new Dictionary<string, string>
                {
                    { "access_token", token },
                    { "message", message },
                    { "link", link.AbsoluteUri }
                };

                var encodedContent = new FormUrlEncodedContent(parameters);

                var result = await http.PostAsync($"{pageId}/feed", encodedContent);
                var msg = result.EnsureSuccessStatusCode();
                return await msg.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> UploadMessageImageAsync(string messageIncLink, Uri image)
        {
            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri(FB_BASE_ADDRESS);
                var postData = new Dictionary<string, string>
                {
                    { "access_token", token },
                    { "message", messageIncLink },
                    { "url", image.AbsoluteUri }
                };

                var encodedContent = new FormUrlEncodedContent(postData);
                var result = await http.PostAsync($"{pageId}/photos", encodedContent);
                var msg = result.EnsureSuccessStatusCode();
                var data = await msg.Content.ReadAsStringAsync();
                var json = JObject.Parse(data);
                var imagePostId = json["post_id"].Value<string>();
                return imagePostId;
            }
        }
    }
}
