using System;
using System.Linq;
using Tweetinvi.Models;

namespace PoliceRewiredSocialDistributorLib.Social.Summary
{
    public class TweetSummary : PostSummary
    {
        private ITweet tweet;

        public TweetSummary(ITweet tweet) : base()
        {
            this.tweet = tweet;
        }

        public ITweet Result => tweet;
    }
}
