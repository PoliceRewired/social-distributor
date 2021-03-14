using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Social.Summary;
using Tweetinvi;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
    /// <summary>
    /// TODO: add image posting for Tweets
    /// </summary>
    public class TwitterPoster : IPoster
    {
        private TwitterClient client;

        public TwitterPoster(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            client = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
        }

        public async Task InitAsync() { }

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
