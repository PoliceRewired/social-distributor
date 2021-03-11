using System;
using System.Linq;
using Tweetinvi.Models;

namespace PoliceRewiredSocialDistributorLib.Social
{
    public class TweetSummary : IPostSummary
    {
        private ITweet tweet;
        private Post post;

        public TweetSummary(Post post, ITweet tweet)
        {
            this.post = post;
            this.tweet = tweet;
        }

        public string Message => tweet.Text;

        public DateTime Posted => tweet.CreatedAt.DateTime;

        public Post Request => post;

        public ITweet Result => tweet;
    }
}
