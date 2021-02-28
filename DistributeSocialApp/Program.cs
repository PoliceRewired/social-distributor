using System;
using PoliceRewiredSocialDistributorLib.Social;

namespace DistributeSocialApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var networkName = GetArg(args, 0, "network").Trim().ToLower();
            var text = GetArg(args, 1, "text");
            var imagePath = GetArg(args, 2, "image path", false);

            DotNetEnv.Env.Load();
            var consumerKey = GetEnv("TWITTER-CONSUMER-KEY");
            var consumerKeySecret = GetEnv("TWITTER-CONSUMER-KEY-SECRET");
            var accessToken = GetEnv("TWITTER-ACCESS-TOKEN");
            var accessTokenSecret = GetEnv("TWITTER-ACCESS-TOKEN-SECRET");

            var network = Enum.Parse<SocialNetwork>(networkName);

            switch (network)
            {
                case SocialNetwork.twitter:
                    var tweeter = new Tweeter(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);

                    break;
                default:
                    var networks = string.Join(", ", Enum.GetNames(typeof(SocialNetwork)));
                    throw new ArgumentOutOfRangeException("network", "Please specify a known network. Choices are: " + networks);
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
