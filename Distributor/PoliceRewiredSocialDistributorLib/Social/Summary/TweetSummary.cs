using System;
using System.Linq;
using Tweetinvi.Models;

namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class TweetSummary : PostSummary
    {
        private ITweet tweet;

        public TweetSummary(Post post, ITweet tweet) : base(post)
        {
            this.tweet = tweet;
        }

        public ITweet Result => tweet;
    }
}
