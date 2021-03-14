using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using PoliceRewiredSocialDistributorLib.Social;
using PoliceRewiredSocialDistributorLib.Social.Posters;
using PoliceRewiredSocialDistributorLib.Social.Summary;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistributeSocialLambda
{
    public class Function
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<DistributeSocialResponse> FunctionHandler(DistributeSocialCommand input, ILambdaContext context)
        {
            var post = new Post(input.message);
            var response = new DistributeSocialResponse();
            response.input = input;
            var started = DateTime.Now;

            try
            {
                switch (input.command)
                {
                    case "dry-run":
                        response.results = await PostMessagesAsync(context, post, input.networks, false);
                        break;

                    case "post":
                        response.results = await PostMessagesAsync(context, post, input.networks, true);
                        break;

                    default:
                        throw new InvalidOperationException("Unrecognised command: " + input.command);
                }
            }
            catch (Exception e)
            {
                context.Logger.LogLine("Unexpected exception: " + e.Message);
                context.Logger.LogLine(e.StackTrace);
            }
            finally
            {
                response.duration_secs = (DateTime.Now - started).TotalSeconds;
            }

            return response;
        }

        private async Task<Dictionary<string, IPostSummary>> PostMessagesAsync(ILambdaContext context, Post post, IEnumerable<string> networks, bool send)
        {
            var results = new Dictionary<string, IPostSummary>();

            foreach (var networkName in networks)
            {
                SocialNetwork network;
                var networkFound = Enum.TryParse(networkName, out network);
                if (networkFound)
                {
                    try
                    {
                        var summary = await PostMessageAsync(context, network, post, send);
                        results[networkName] = summary;
                    }
                    catch (Exception e)
                    {
                        context.Logger.LogLine(e.Message);
                        context.Logger.LogLine(e.StackTrace);
                        results[networkName] = new PostSummary(post, e);
                    }
                }
                else
                {
                    results[network.ToString()] = new PostSummary(post, "Network " + networkName + " not recognised.");
                    context.Logger.LogLine("Network " + networkName + " not recognised.");
                }
            }

            return results;
        }

        private async Task<IPostSummary> PostMessageAsync(ILambdaContext context, SocialNetwork network, Post post, bool send = false)
        {
            context.Logger.LogLine("Network: " + network.ToString());

            switch (network)
            {
                case SocialNetwork.twitter:
                    var consumerKey = GetEnv("TWITTER_CONSUMER_KEY");
                    var consumerKeySecret = GetEnv("TWITTER_CONSUMER_KEY_SECRET");
                    var accessToken = GetEnv("TWITTER_ACCESS_TOKEN");
                    var accessTokenSecret = GetEnv("TWITTER_ACCESS_TOKEN_SECRET");
                    if (send)
                    {
                        context.Logger.LogLine("Tweeting...");
                        var tweeter = new TwitterPoster(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
                        var twSummary = await tweeter.PostAsync(post);
                        context.Logger.LogLine(twSummary.Summarise());
                        return twSummary;
                    }
                    else
                    {
                        context.Logger.LogLine("Not tweeting...");
                        return new PostSummary(post, "Not attempted.");
                    }

                case SocialNetwork.facebook:
                    var pageId = long.Parse(GetEnv("FACEBOOK_PAGE_ID"));
                    var fbToken = GetEnv("FACEBOOK_ACCESS_TOKEN");
                    if (send)
                    {
                        context.Logger.LogLine("Facebooking...");
                        var facebooker = new FbPoster(pageId, fbToken);
                        var fbSummary = await facebooker.PostAsync(post);
                        context.Logger.LogLine(fbSummary.Summarise());
                        return fbSummary;
                    }
                    else
                    {
                        context.Logger.LogLine("Not facebooking...");
                        return new PostSummary(post, "Not attempted.");
                    }

                case SocialNetwork.discord:
                    var discordToken = GetEnv("DISCORD_TOKEN");
                    var discordServer = ulong.Parse(GetEnv("DISCORD_SERVER_ID"));
                    var discordChannel = GetEnv("DISCORD_CHANNEL");
                    if (send)
                    {
                        context.Logger.LogLine("Discording...");
                        var discorder = new DiscordPoster(discordToken);
                        await discorder.InitAsync();
                        var discordSummary = await discorder.PostAsync(new Post(discordServer, discordChannel, post.Message, post.ImagePath));
                        context.Logger.LogLine(discordSummary.Summarise());
                        return discordSummary;
                    }
                    else
                    {
                        context.Logger.LogLine("Not discording...");
                        return new PostSummary(post, "Not attempted.");
                    }

                default:
                    var networks = string.Join(", ", Enum.GetNames(typeof(SocialNetwork)));
                    throw new ArgumentOutOfRangeException("network", network.ToString() + " is not supported. Choices are: " + networks);
            }
        }

        private string GetEnv(string key, bool required = true)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (required && string.IsNullOrWhiteSpace(value))
            {
                throw new KeyNotFoundException("Environment variable " + key + " not found.");
            }
            else
            {
                return value;
            }
        }
    }
}
