﻿using System;
using System.IO;
using System.Threading.Tasks;
using PoliceRewiredSocialDistributorLib.Social;
using PoliceRewiredSocialDistributorLib.Social.Posters;

namespace DistributeSocialApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var environment = GetArg(args, 0, "environment").Trim().ToLower();
            var networkName = GetArg(args, 1, "network").Trim().ToLower();
            var text = GetArg(args, 2, "text");
            var tags = GetArg(args, 3, "tags");
            var link = GetArg(args, 4, "link");
            var image = GetArg(args, 5, "image", false);

            var envFile = ".env." + environment;
            if (!File.Exists(envFile)) { throw new FileNotFoundException("Environment file not found.", envFile); }

            var vars = DotNetEnv.Env.Load(envFile);

            foreach (var pair in vars)
            {
                Console.WriteLine("Key = " + pair.Key + "; Value = " + pair.Value);
            }

            var network = Enum.Parse<SocialNetwork>(networkName);

            Console.WriteLine("Environment: " + environment);
            Console.WriteLine("Network:     " + network.ToString());
            Console.WriteLine("Text:        " + text);
            Console.WriteLine("Tags:        " + tags);
            Console.WriteLine("Link:        " + link);
            Console.WriteLine("Image:       " + image);

            var linkUri = string.IsNullOrWhiteSpace(link) ? null : new Uri(link);
            var imageUri = string.IsNullOrWhiteSpace(image) ? null : new Uri(image);
            var post = new Post(text, tags, linkUri, imageUri);

            switch (network)
            {
                case SocialNetwork.twitter:
                    var consumerKey = GetEnv("TWITTER_CONSUMER_KEY");
                    var consumerKeySecret = GetEnv("TWITTER_CONSUMER_KEY_SECRET");
                    var accessToken = GetEnv("TWITTER_ACCESS_TOKEN");
                    var accessTokenSecret = GetEnv("TWITTER_ACCESS_TOKEN_SECRET");
                    Console.WriteLine("Tweeting...");
                    var tweeter = new TwitterPoster(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
                    var twSummary = await tweeter.PostAsync(post);
                    Console.WriteLine(twSummary.Summarise());
                    break;
                case SocialNetwork.facebook:
                    var pageId = long.Parse(GetEnv("FACEBOOK_PAGE_ID"));
                    var fbToken = GetEnv("FACEBOOK_ACCESS_TOKEN");
                    Console.WriteLine("Facebooking...");
                    var facebooker = new FbPoster(pageId, fbToken);
                    var fbSummary = await facebooker.PostAsync(post);
                    Console.WriteLine(fbSummary.Summarise());
                    break;
                case SocialNetwork.discord:
                    var discordToken = GetEnv("DISCORD_TOKEN");
                    var discordServer = ulong.Parse(GetEnv("DISCORD_SERVER_ID"));
                    var discordChannel = GetEnv("DISCORD_CHANNEL");
                    post.SetDiscordChannel(discordServer, discordChannel);
                    Console.WriteLine("Discording...");
                    var discorder = new DiscordPoster(discordToken);
                    await discorder.InitAsync();
                    var discordSummary = await discorder.PostAsync(post);
                    Console.WriteLine(discordSummary.Summarise());
                    break;
                case SocialNetwork.reddit:
                    var redditClientId = GetEnv("REDDIT_CLIENT_ID");
                    var redditClientSecret = GetEnv("REDDIT_CLIENT_SECRET");
                    var redditUsername = GetEnv("REDDIT_USERNAME");
                    var redditPassword = GetEnv("REDDIT_PASSWORD");
                    var redditSubreddit = GetEnv("REDDIT_SUBREDDIT");
                    post.SetSubreddit(redditSubreddit);
                    var redditer = new RedditPoster(redditClientId, redditClientSecret, redditUsername, redditPassword);
                    await redditer.InitAsync();
                    var redditSummary = await redditer.PostAsync(post);
                    Console.WriteLine(redditSummary.Summarise());
                    break;
                default:
                    var networks = string.Join(", ", Enum.GetNames(typeof(SocialNetwork)));
                    throw new ArgumentOutOfRangeException("network", "Please specify a supported network. Choices are: " + networks);
            }
        }

        public static string GetArg(string[] args, int index, string name, bool required = true)
        {
            if (args.Length < index+1)
            {
                if (required)
                {
                    throw new ArgumentNullException(name, "Argument " + index + " missing. Should contain a value for: " + name);
                }
                else
                {
                    return null;
                }    
            }

            var result = args[index];

            if (required && string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentNullException(name, "Argument " + index + " is empty. Should contain a value for: " + name);
            }

            return result;
        }

        public static string GetEnv(string key, bool required = true)
        {
            var result = DotNetEnv.Env.GetString(key);
            Console.WriteLine("Environment variable: " + key + " = " + result);
            if (required && string.IsNullOrWhiteSpace(result))
            {
                throw new ArgumentNullException(key, "The " + key + " environment variable is not set.");
            }
            else
            {
                return result;
            }
        }

        public static int GetEnvInt(string key, bool required = true)
        {
            var result = DotNetEnv.Env.GetInt(key, -1);
            Console.WriteLine("Environment variable: " + key + " = " + result);
            if (required && result == -1)
            {
                throw new ArgumentNullException(key, "The " + key + " environment variable is not set.");
            }
            else
            {
                return result;
            }
        }

    }
}
