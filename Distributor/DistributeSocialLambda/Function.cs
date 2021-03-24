using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using DistributeSocialLambda.DTO;
using PoliceRewiredSocialDistributorLib.Instruction;
using PoliceRewiredSocialDistributorLib.Social;
using PoliceRewiredSocialDistributorLib.Social.Posters;
using PoliceRewiredSocialDistributorLib.Social.Summary;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DistributeSocialLambda
{
    public class Function
    {
        ILambdaContext context;

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<DistributeSocialResponse> FunctionHandler(DistributeSocialCommand input, ILambdaContext context)
        {
            this.context = context;

            var response = new DistributeSocialResponse();
            response.input = input;
            var started = DateTime.Now;

            try
            {
                switch (input.command)
                {
                    case "dry-run":
                        var postDryRun = new Post(input.message);
                        response.results = await PostMessagesAsync(context, postDryRun, input.networks, false);
                        break;

                    case "post":
                        var post = new Post(input.message);
                        response.results = await PostMessagesAsync(context, post, input.networks, true);
                        break;

                    case "auto":
                        response.results = await RunAutoPostAsync(context, input.postsCsvUrl, input.rulesCsvUrl, input.networks, true);
                        break;

                    case "auto-dry-run":
                        response.results = await RunAutoPostAsync(context, input.postsCsvUrl, input.rulesCsvUrl, input.networks, false);
                        break;

                    default:
                        throw new InvalidOperationException("Unrecognised command: " + input.command);
                }
            }
            catch (Exception e)
            {
                Log("Unexpected exception: " + e.Message);
                Log(e.StackTrace);
            }
            finally
            {
                response.duration_secs = (DateTime.Now - started).TotalSeconds;
            }

            return response;
        }

        private async Task<NetworkPostDTO> RunAutoPostAsync(ILambdaContext context, string postsCsvUrl, string rulesCsvUrl, IEnumerable<string> networks, bool send)
        {
            var results = new NetworkPostDTO();

            var stateBucket = GetEnv("S3_STATE_BUCKET");
            var stateKey = GetEnv("S3_STATE_KEY");

            var manager = new ContentManager(postsCsvUrl, rulesCsvUrl, stateBucket, stateKey, Log);
            var listContent = await manager.RunAsync();

            Log("Next post comes from list: " + listContent.List);
            Log("Next post: " + listContent.Content);

            var post = new Post(listContent.Content);
            return await PostMessagesAsync(context, post, networks, send);
        }

        private async Task<NetworkPostDTO> PostMessagesAsync(ILambdaContext context, Post post, IEnumerable<string> networks, bool send)
        {
            var results = new NetworkPostDTO();

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
                        Log(e.Message);
                        Log(e.StackTrace);
                        results[networkName] = new PostSummary(post, e);
                    }
                }
                else
                {
                    results[network.ToString()] = new PostSummary(post, "Network " + networkName + " not recognised.");
                    Log("Network " + networkName + " not recognised.");
                }
            }

            return results;
        }

        private async Task<IPostSummary> PostMessageAsync(ILambdaContext context, SocialNetwork network, Post post, bool send = false)
        {
            Log("Network: " + network.ToString());

            switch (network)
            {
                case SocialNetwork.twitter:
                    var consumerKey = GetEnv("TWITTER_CONSUMER_KEY");
                    var consumerKeySecret = GetEnv("TWITTER_CONSUMER_KEY_SECRET");
                    var accessToken = GetEnv("TWITTER_ACCESS_TOKEN");
                    var accessTokenSecret = GetEnv("TWITTER_ACCESS_TOKEN_SECRET");
                    if (send)
                    {
                        Log("Tweeting...");
                        var tweeter = new TwitterPoster(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
                        var twSummary = await tweeter.PostAsync(post);
                        Log(twSummary.Summarise());
                        return twSummary;
                    }
                    else
                    {
                        Log("Not tweeting...");
                        return new PostSummary(post, "Not attempted.");
                    }

                case SocialNetwork.facebook:
                    var pageId = long.Parse(GetEnv("FACEBOOK_PAGE_ID"));
                    var fbToken = GetEnv("FACEBOOK_ACCESS_TOKEN");
                    if (send)
                    {
                        Log("Facebooking...");
                        var facebooker = new FbPoster(pageId, fbToken);
                        var fbSummary = await facebooker.PostAsync(post);
                        Log(fbSummary.Summarise());
                        return fbSummary;
                    }
                    else
                    {
                        Log("Not facebooking...");
                        return new PostSummary(post, "Not attempted.");
                    }

                case SocialNetwork.discord:
                    var discordToken = GetEnv("DISCORD_TOKEN");
                    var discordServer = ulong.Parse(GetEnv("DISCORD_SERVER_ID"));
                    var discordChannel = GetEnv("DISCORD_CHANNEL");
                    if (send)
                    {
                        Log("Discording...");
                        var discorder = new DiscordPoster(discordToken);
                        await discorder.InitAsync();
                        var discordSummary = await discorder.PostAsync(new Post(discordServer, discordChannel, post.Message, post.ImagePath));
                        Log(discordSummary.Summarise());
                        return discordSummary;
                    }
                    else
                    {
                        Log("Not discording...");
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

        private void Log(string msg)
        {
            context.Logger.LogLine(msg);
        }
    }
}
