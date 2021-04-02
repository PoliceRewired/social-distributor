using System;
using System.Net;
using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Social.Summary;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace PoliceRewiredSocialDistributorLib.Social.Posters
{
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
            PublishTweetParameters parameters;
            if (post.Image == null)
            {
                parameters = new PublishTweetParameters()
                {
                    Text = post.MessageTwitter
                };
            }
            else
            {
                var media = await UploadImageAsync(post.Image);
                parameters = new PublishTweetParameters()
                {
                    Text = post.MessageTwitter,
                    Medias = { media }
                };
            }

            var tweet = await client.Tweets.PublishTweetAsync(parameters);
            return new TweetSummary(tweet);
        }

        private async Task<IMedia> UploadImageAsync(Uri image)
        {
            using (var http = new WebClient())
            {
                var imageBinary = await http.DownloadDataTaskAsync(image.AbsoluteUri);
                var uploadedImage = await client.Upload.UploadTweetImageAsync(imageBinary);
                return uploadedImage;
            }
        }
    }

}
