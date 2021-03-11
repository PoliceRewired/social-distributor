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
            if (!File.Exists(envFile)) { throw new FileNotFoundException(envFile + " not found."); }
            DotNetEnv.Env.Load(envFile);

            var consumerKey = GetEnv("TWITTER_CONSUMER_KEY");
            var consumerKeySecret = GetEnv("TWITTER_CONSUMER_KEY_SECRET");
            var accessToken = GetEnv("TWITTER_ACCESS_TOKEN");
            var accessTokenSecret = GetEnv("TWITTER_ACCESS_TOKEN_SECRET");

            var network = Enum.Parse<SocialNetwork>(networkName);

            Console.WriteLine("Environment: " + environment);
            Console.WriteLine("Network:     " + network.ToString());
            Console.WriteLine("Text:        " + text);
            Console.WriteLine("Image:       " + imagePath);

            switch (network)
            {
                case SocialNetwork.twitter:
                    Console.WriteLine("Tweeting...");
                    var tweeter = new TwitterPoster(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
                    var result = await tweeter.PostAsync(Post.FromText(text));
                    Console.WriteLine("Result: " + result.Message);
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
            if (required && string.IsNullOrWhiteSpace(result))
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
