using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using PoliceRewiredSocialDistributorLib.Social;

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
        public DistributeSocialResponse FunctionHandler(DistributeSocialCommand input, ILambdaContext context)
        {
            switch (input.command)
            {
                case "dry-run":
                    var started = DateTime.Now;
                    var response = new DistributeSocialResponse();
                    response.input = input;
                    response.results = new Dictionary<string, string>();

                    foreach (var networkName in input.networks)
                    {
                        SocialNetwork network;
                        var networkFound = Enum.TryParse(networkName, out network);
                        if (networkFound)
                        {
                            context.Logger.LogLine("Posting to network: " + network.ToString());
                            try
                            {
                                context.Logger.LogLine("Not attempted.");
                                response.results[network.ToString()] = "Not attempted.";
                            }
                            catch (Exception e)
                            {
                                context.Logger.LogLine(e.Message);
                                context.Logger.LogLine(e.StackTrace);
                                response.results[network.ToString()] = e.Message;
                            }
                        }
                        else
                        {
                            context.Logger.LogLine("Network " + networkName + " not recognised.");
                        }
                    }

                    response.duration_secs = (DateTime.Now - started).TotalSeconds;
                    return response;
                default:
                    throw new InvalidOperationException("Unrecognised command: " + input.command);
            }
        }
    }
}
