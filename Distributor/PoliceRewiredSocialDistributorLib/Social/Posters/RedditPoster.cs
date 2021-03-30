using System;
using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Social.Summary;
using RedditSharp;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
    public class RedditPoster : IPoster
    {
        private string clientSecret;
        private string clientId;
        private string username;
        private string password;

        private Reddit reddit;
        private BotWebAgent agent;

        public RedditPoster(string clientId, string clientSecret, string username, string password)
        {
            this.clientSecret = clientSecret;
            this.clientId = clientId;
            this.username = username;
            this.password = password;
        }

        public bool Accepts(SocialNetwork network)
        {
            return network == SocialNetwork.reddit;
        }

        public async Task InitAsync()
        {
            agent = new BotWebAgent(username, password, clientId, clientSecret, null);
            reddit = new Reddit(agent, false);
        }

        public async Task<IPostSummary> PostAsync(Post post)
        {
            var subreddit = await reddit.GetSubredditAsync(post.Subreddit);
            var posted = await subreddit.SubmitPostAsync(post.TitleReddit, post.Link.AbsoluteUri);
            return new PostSummary(post, true);
        }
    }
}
