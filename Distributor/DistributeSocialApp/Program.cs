using System;
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
            var imagePath = GetArg(args, 3, "image path", false);

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
            Console.WriteLine("Image:       " + imagePath);

            switch (network)
            {
                case SocialNetwork.twitter:
                    var consumerKey = GetEnv("TWITTER_CONSUMER_KEY");
                    var consumerKeySecret = GetEnv("TWITTER_CONSUMER_KEY_SECRET");
                    var accessToken = GetEnv("TWITTER_ACCESS_TOKEN");
                    var accessTokenSecret = GetEnv("TWITTER_ACCESS_TOKEN_SECRET");
                    Console.WriteLine("Tweeting...");
                    var tweeter = new TwitterPoster(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
                    var twSummary = await tweeter.PostAsync(new Post(text, imagePath));
                    Console.WriteLine(twSummary.Summarise());
                    break;
                case SocialNetwork.facebook:
                    var pageId = long.Parse(GetEnv("FACEBOOK_PAGE_ID"));
                    var fbToken = GetEnv("FACEBOOK_ACCESS_TOKEN");
                    Console.WriteLine("Facebooking...");
                    var facebooker = new FbPoster(pageId, fbToken);
                    var fbSummary = await facebooker.PostAsync(new Post(text, imagePath));
                    Console.WriteLine(fbSummary.Summarise());
                    break;
                case SocialNetwork.discord:
                    var discordToken = GetEnv("DISCORD_TOKEN");
                    var discordServer = ulong.Parse(GetEnv("DISCORD_SERVER_ID"));
                    var discordChannel = GetEnv("DISCORD_CHANNEL");
                    Console.WriteLine("Discording...");
                    var discorder = new DiscordPoster(discordToken);
                    await discorder.InitAsync();
                    var discordSummary = await discorder.PostAsync(new Post(discordServer, discordChannel, text, imagePath));
                    Console.WriteLine(discordSummary.Summarise());
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
