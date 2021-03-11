using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Tweetinvi;
using Tweetinvi.Models;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
    public class TwitterPoster : IPoster
    {
        private TwitterClient client;

        public TwitterPoster(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            client = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public bool Accepts(SocialNetwork network)
        {
            return network == SocialNetwork.twitter;
        }

        public async Task<IPostSummary> PostAsync(Post post)
        {
            var tweet = await client.Tweets.PublishTweetAsync(post.Message);
            return new TweetSummary(post, tweet);
        }
    }

}
